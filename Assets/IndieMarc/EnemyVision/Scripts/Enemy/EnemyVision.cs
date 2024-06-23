using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndieMarc.EnemyVision
{
    /// <summary>
    /// Manages enemy detection of the player, will also spawn a VisionCone and change the state of Enemy.cs based on what is seen
    /// </summary>

    public class EnemyVision : MonoBehaviour
    {
        [Header("Detection")]
        public float vision_angle = 30f;
        public float vision_range = 10f;
        public float vision_near_range = 5f;
        public float vision_height_above = 1f; //How far above can they detect
        public float vision_height_below = 10f; //How far below can they detect
        public float touch_range = 1f;
        public LayerMask vision_mask = ~(0);
        public bool group_detect = false; //Group detect will make an enemy follow another enemy chasing even if didnt see player

        [Header("Duration")]
        public float detect_time = 1f;
        public float confused_time = 10f;

        [Header("Chase")]
        public float follow_time = 10f;
        public bool dont_return = false;

        [Header("Ref")]
        public Transform eye;
        public GameObject vision_prefab;
        public GameObject death_fx_prefab;

        public UnityAction<VisionTarget, int> onSeeTarget; //As soon as seen (Patrol->Alert)  int:0=touch, 1=near, 2=far, 3:other
        public UnityAction<VisionTarget, int> onDetectTarget; //detect_time seconds after seen (Alert->Chase)  int:0=touch, 1=near, 2=far
        public UnityAction<VisionTarget> onTouchTarget;
        public UnityAction<Vector3> onAlert;
        public UnityAction onDeath;

        private Enemy enemy;

        private VisionTarget seen_character = null;
        private VisionCone vision;

        private float detect_timer = 0f;
        private float vision_timer = 0f;
        private float wait_timer = 0f;

        private static List<EnemyVision> enemy_list = new List<EnemyVision>();

        private void Awake()
        {
            enemy_list.Add(this);
            enemy = GetComponent<Enemy>();
            if(enemy != null)
                enemy.onDeath += OnDeath;
        }

        private void OnDestroy()
        {
            enemy_list.Remove(this);
        }

        void Start()
        {
            if (vision_prefab)
            {
                GameObject vis = Instantiate(vision_prefab, GetEye(), Quaternion.identity);
                vis.transform.parent = transform;
                vision = vis.GetComponent<VisionCone>();
                vision.target = this;
                vision.vision_angle = vision_angle;
                vision.vision_range = vision_range;
                vision.vision_near_range = vision_near_range;
            }

            if (enemy != null)
                enemy.ChangeState(EnemyState.Patrol);
            seen_character = null;
        }

        void Update()
        {
            //In case using vision without enemy behavior
            if (enemy == null)
                DetectVisionTargetOnly();

            if(enemy == null || enemy.IsPaused())
                return;

            wait_timer -= Time.deltaTime;

            //While patroling, detect targets
            if (enemy.GetState() == EnemyState.Patrol)
            {
                DetectVisionTarget();

                if(group_detect)
                    DetectOtherEnemies();
            }

            //When just seen the VisionTarget, enemy alerted
            if (enemy.GetState() == EnemyState.Alert)
            {
                VisionTarget target_seen = CanSeeAnyVisionTarget();

                vision_timer += target_seen ? Time.deltaTime : -Time.deltaTime;

                if (target_seen != null && vision_timer < -0.5f)
                {
                    Alert(target_seen);

                    int distance = GetSeenDistance(target_seen.gameObject);
                    if (onSeeTarget != null)
                        onSeeTarget.Invoke(target_seen, distance);
                }

                if (target_seen != null)
                    enemy.SetAlertTarget(target_seen.transform.position);

                if (target_seen != null && vision_timer > detect_time)
                {
                    Chase(target_seen);

                    if (onDetectTarget != null)
                        onDetectTarget.Invoke(target_seen, 2);
                }

                if (target_seen != null && enemy.GetStateTimer() > 0.2f && CanSeeVisionTargetNear(target_seen))
                {
                    Chase(target_seen);

                    bool is_touch = CanTouchObject(target_seen.gameObject);
                    if (onDetectTarget != null)
                        onDetectTarget.Invoke(target_seen, is_touch ? 0 : 1);
                }

                if (enemy.HasReachedTarget() || enemy.GetStateTimer() > (detect_time + 1f))
                {
                    ResumeDefault();
                }

                if (group_detect)
                    DetectOtherEnemies();
            }

            //If seen long enough (detect time), will go into a chase
            if (enemy.GetState() == EnemyState.Chase)
            {
                bool can_see_target = CanSeeVisionTarget(seen_character);

                vision_timer += can_see_target ? -Time.deltaTime : Time.deltaTime;
                vision_timer = Mathf.Max(vision_timer, 0f);

                if (enemy.GetStateTimer() > 0.5f)
                {
                    enemy.SetFollowTarget(can_see_target ? seen_character.gameObject : null);
                }

                if (vision_timer > follow_time)
                {
                    ResumeDefault();
                }

                if (enemy.HasReachedTarget() && !can_see_target)
                    enemy.ChangeState(EnemyState.Confused);

                if(seen_character == null)
                    enemy.ChangeState(EnemyState.Confused);

                DetectTouchTarget();
            }

            //After the chase, if VisionTarget is unseen, enemy will be confused
            if (enemy.GetState() == EnemyState.Confused)
            {
                VisionTarget target_seen = CanSeeAnyVisionTarget();
                if (target_seen != null && target_seen == seen_character)
                {
                    Chase(target_seen);

                    int distance = GetSeenDistance(target_seen.gameObject);
                    if (onDetectTarget != null)
                        onDetectTarget.Invoke(target_seen, distance);
                }

                if (target_seen != null && target_seen != seen_character)
                {
                    Alert(target_seen);

                    int distance = GetSeenDistance(target_seen.gameObject);
                    if (onSeeTarget != null)
                        onSeeTarget.Invoke(target_seen, distance);
                }

                if (!dont_return && enemy.GetStateTimer() > confused_time)
                {
                    ResumeDefault();
                }

                if (group_detect)
                    DetectOtherEnemies();
            }

            if (enemy.GetState() == EnemyState.Wait)
            {
                if (enemy.GetStateTimer() > 0.5f && wait_timer < 0f)
                {
                    ResumeDefault();
                }
            }
        }

        //In case using vision without enemy behavior, detect without changing state
        private void DetectVisionTargetOnly()
        {
            detect_timer += Time.deltaTime;
            VisionTarget target = CanSeeAnyVisionTarget();
            if (target != null && detect_timer > detect_time)
            {
                int dist = GetSeenDistance(target.gameObject);
                onSeeTarget.Invoke(target, dist);
                vision_timer = 0f;
            }
        }

        //Look for possible seen targets
        private void DetectVisionTarget()
        {
            if (wait_timer > 0f)
                return;

            //Detect character
            foreach (VisionTarget character in VisionTarget.GetAll())
            {
                if (character == seen_character)
                    continue;

                if (CanSeeVisionTarget(character))
                {
                    Alert(character);

                    int distance = GetSeenDistance(character.gameObject);
                    if (onSeeTarget != null)
                        onSeeTarget.Invoke(character, distance);
                }
            }
        }

        //Check if the enemy is in touch range of a target
        private void DetectTouchTarget()
        {
            if (wait_timer > 0f)
                return;

            //Detect character touch
            foreach (VisionTarget character in VisionTarget.GetAll())
            {
                if (CanTouchObject(character.gameObject))
                {
                    if (onTouchTarget != null)
                        onTouchTarget.Invoke(character);
                }
            }
        }

        //Check if allies running
        private void DetectOtherEnemies()
        {
            if (wait_timer > 0f)
                return;

            foreach (Enemy oenemy in Enemy.GetAll())
            {
                if (oenemy != enemy && oenemy.GetState() == EnemyState.Chase)
                {
                    if (oenemy.follow_target != null && CanSeeObject(oenemy.gameObject, vision_range, vision_angle))
                    {
                        VisionTarget target = oenemy.follow_target.GetComponent<VisionTarget>();
                        Chase(target);

                        if (target != null && onDetectTarget != null)
                            onDetectTarget.Invoke(target, 3);
                    }
                }
            }
        }

        //Can see any vision target
        public VisionTarget CanSeeAnyVisionTarget()
        {
            foreach (VisionTarget character in VisionTarget.GetAll())
            {
                if(CanSeeVisionTarget(character))
                {
                    return character;
                }
            }
            return null;
        }

        //Can the enemy see a vision target?
        public bool CanSeeVisionTarget(VisionTarget target)
        {
            return target != null && target.CanBeSeen() 
                && (CanSeeObject(target.gameObject, vision_range, vision_angle) || CanTouchObject(target.gameObject));
        }

        public bool CanSeeVisionTargetNear(VisionTarget target)
        {
            return target != null && target.CanBeSeen()
                && (CanSeeObject(target.gameObject, vision_near_range, vision_angle) || CanTouchObject(target.gameObject));
        }

        //Can the enemy see an object ?
        public bool CanSeeObject(GameObject obj, float see_range, float see_angle)
        {
            Vector3 forward = transform.forward;
            Vector3 dir = obj.transform.position - GetEye();
            Vector3 dir_touch = dir; //Preserve Y for touch
            dir.y = 0f; //Remove Y for cone vision range

            float vis_range = see_range;
            float vis_angle = see_angle;
            float losangle = Vector3.Angle(forward, dir);
            float losheight = obj.transform.position.y - GetEye().y;
            bool can_see_cone = losangle < vis_angle / 2f && dir.magnitude < vis_range && losheight < vision_height_above && losheight > -vision_height_below;
            bool can_see_touch = dir_touch.magnitude < touch_range;
            if (obj.activeSelf && (can_see_cone || can_see_touch)) //In range and in angle
            {
                RaycastHit hit;
                bool raycast = Physics.Raycast(new Ray(GetEye(), dir.normalized), out hit, dir.magnitude, vision_mask.value);
                if (!raycast)
                    return true; //No obstacles in the way (in case character not in layer)
                if (raycast && (hit.collider.gameObject == obj || hit.collider.transform.IsChildOf(obj.transform))) //See character
                    return true; //The only obstacles is the character
            }
            return false;
        }

        //Is the enemy right next to the object ?
        public bool CanTouchObject(GameObject obj)
        {
            Vector3 dir = obj.transform.position - transform.position;
            if (dir.magnitude < touch_range) //In range and in angle
            {
                return true;
            }
            return false;
        }

        //Return seen distance of target: 0:touch,  1:near,  2:far,  3:other
        public int GetSeenDistance(GameObject target)
        {
            bool is_near = CanSeeObject(target, vision_near_range, vision_angle);
            bool is_touch = CanTouchObject(target);
            int distance = is_touch ? 0 : (is_near ? 1 : 2);
            return distance;
        }

        //Call this function from another script to manually alert of the target presense
        public void Alert(VisionTarget target)
        {
            if (target != null)
            {
                Alert(target.transform.position);
            }
        }

        //Alert with a position instead of object (such as noise)
        public void Alert(Vector3 target)
        {
            if(enemy != null)
                enemy.Alert(target);
            vision_timer = 0f;

            if (onAlert != null)
                onAlert.Invoke(target);
        }

        //Call this function from another script to manually start chasing the target
        public void Chase(VisionTarget target)
        {
            if (target != null)
            {
                seen_character = target;
                vision_timer = 0f;
                if(enemy != null)
                    enemy.Follow(seen_character.gameObject);
            }
        }

        //Call this function from another script to stop chasing, may not work if the target is still in vision because it will just start chasing again
        public void Stop()
        {
            ResumeDefault();
            WaitFor(2f);
        }

        public void ResumeDefault()
        {
            seen_character = null;
            wait_timer = 0f;
            if (enemy != null)
            {
                if (dont_return)
                    enemy.ChangeState(EnemyState.Confused);
                else
                    enemy.ChangeState(EnemyState.Patrol);
            }
        }

        //Stop detecting player for X seconds
        public void PauseVisionFor(float time)
        {
            wait_timer = time;
        }

        //Do nothing for X seconds
        public void WaitFor(float time)
        {
            wait_timer = time;
            if (enemy != null)
            {
                enemy.ChangeState(EnemyState.Wait);
                enemy.StopMove();
            }
        }

        public Vector3 GetEye()
        {
            return eye ? eye.position : transform.position;
        }

        public Enemy GetEnemy()
        {
            return enemy;
        }

        private void OnDeath()
        {
            if(vision)
                vision.gameObject.SetActive(false);

            if (onDeath != null)
                onDeath.Invoke();
        }

        public static EnemyVision GetNearest(Vector3 pos, float range = 999f)
        {
            float min_dist = range;
            EnemyVision nearest = null;
            foreach (EnemyVision enemy in enemy_list)
            {
                float dist = (enemy.transform.position - pos).magnitude;
                if (dist < min_dist)
                {
                    min_dist = dist;
                    nearest = enemy;
                }
            }
            return nearest;
        }

        public static List<EnemyVision> GetAllInRange(Vector3 pos, float range)
        {
            List<EnemyVision> range_list = new List<EnemyVision>();
            foreach (EnemyVision enemy in enemy_list)
            {
                float dist = (enemy.transform.position - pos).magnitude;
                if (dist < range)
                {
                    range_list.Add(enemy);
                }
            }
            return range_list;
        }

        public static List<EnemyVision> GetAll()
        {
            return enemy_list;
        }
    }

}
