using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace IndieMarc.EnemyVision
{
    public enum EnemyState2D
    {
        None=0,
        Patrol = 2,
        Alert = 5,
        Chase = 10,
        Confused = 15,
        Wait = 20,
    }

    /// <summary>
    /// Handles enemy movement and the different enemy behaviors (2D)
    /// </summary>

    public class Enemy2D : MonoBehaviour
    {
        public float move_speed = 2f;
        public float run_speed = 4f;
        public float rotate_speed = 120f;
        public float fall_speed = 5f;
        public float fall_accel = 10f;
        public LayerMask obstacle_mask = ~(0); //All bit 1
        public float ground_raycast_dist = 0.1f;

        [Header("State")]
        public EnemyState2D state = EnemyState2D.Patrol;

        [Header("Patrol")]
        public float wait_time = 1f;
        public GameObject[] patrol_targets;

        [Header("Patrol Facing")]
        public float angle_min = -20f;
        public float angle_max = 20f;
        public float angle_speed = 50f;
        public float angle_pause = 0f;

        [Header("Alert")]
        public float alert_wait_time = 10f;
        public float alert_walk_time = 10f;

        [Header("Follow")]
        public GameObject follow_target;
        public float memory_duration = 4f;
        
        public UnityAction onDeath;

        private Rigidbody2D rigid;
        private CapsuleCollider2D capsule_coll;
        private ContactFilter2D contact_filter;
        private Vector3 start_pos;
        private Vector3 start_scale;
        private Vector3 move_vect;
        private Vector3 face_vect;
        private Quaternion face_rot;
        private bool paused = false;

        private Vector2 move_target;
        private Vector3 alert_target;
        private float current_speed = 1f;
        private Vector3 current_rot_target;
        private float current_rot_mult = 1f;
        private float fall_value = 0f;
        private float state_timer = 0f;

        private int current_path = 0;
        private bool path_rewind = false;

        private Vector3 move_dir;
        private Vector3 face_dir;
        private float current_angle = 0f;
        private bool angle_rewind = false;
        private float pause_timer = 0f;
        private bool waiting = false;
        private float wait_timer = 0f;

        private Vector3 last_seen_pos;
        private GameObject last_target;
        private float memory_timer = 0f;

        private List<Vector3> path_list = new List<Vector3>();

        private static List<Enemy2D> enemy_list = new List<Enemy2D>();

        private void Awake()
        {
            enemy_list.Add(this);
            rigid = GetComponent<Rigidbody2D>();
            capsule_coll = GetComponentInChildren<CapsuleCollider2D>();
            move_vect = Vector3.zero;
            start_pos = transform.position;
            move_target = transform.position;
            current_rot_target = transform.position + transform.forward;
            alert_target = follow_target ? follow_target.transform.position : transform.position;
            start_scale = transform.localScale;
            face_rot = Quaternion.identity;
            move_vect = Mathf.Sign(start_scale.x) > 0f ? Vector3.right : Vector3.left;
            current_speed = move_speed;

            contact_filter = new ContactFilter2D();
            contact_filter.layerMask = obstacle_mask;
            contact_filter.useLayerMask = true;
            contact_filter.useTriggers = false;

            move_dir = Vector3.right * Mathf.Sign(transform.localScale.x);
            face_dir = move_dir;

            RefreshPatrol();

            current_path = 0;
            if (path_list.Count >= 2)
                current_path = 1; //Dont start at start pos

            if (path_list.Count <= 1)
                path_rewind = Mathf.Sign(transform.localScale.x) < 0f;
        }

        private void OnDestroy()
        {
            enemy_list.Remove(this);
        }

        void Start()
        {
            
        }

        private void RefreshPatrol()
        {
            path_list.Clear();
            path_list.Add(transform.position);
            foreach (GameObject patrol in patrol_targets)
            {
                if (patrol)
                    path_list.Add(patrol.transform.position);
            }
        }

        private void FixedUpdate()
        {
            if (paused)
                return;

            Vector2 dist_vect = (move_target - rigid.position);
            move_vect = dist_vect.normalized * current_speed * Mathf.Min(dist_vect.magnitude, 1f);
            move_vect.z = 0f;

            bool grounded = DetectObstacle(Vector3.down);

            if (fall_speed > 0.1f)
            {
                if (grounded)
                    fall_value = 0f;
                else
                    fall_value += fall_accel * Time.deltaTime;
                fall_value = Mathf.Clamp(fall_value, 0f, fall_speed);
                move_vect.y = -fall_value;
            }

            rigid.velocity = move_vect;
        }

        private void Update()
        {
            if (paused)
                return;

            state_timer += Time.deltaTime;
            wait_timer += Time.deltaTime;

            if (state == EnemyState2D.Alert)
            {
                UpdateAlert();
            }

            if (state == EnemyState2D.Patrol)
            {
                UpdatePatrol();
            }

            if (state == EnemyState2D.Chase)
            {
                UpdateFollow();
            }

            if (state == EnemyState2D.Confused)
            {
                UpdateConfused();
            }

            Vector3 dir = current_rot_target - transform.position;
            dir.z = 0f;
            Debug.DrawRay(transform.position, dir);

            //Side
            if (Mathf.Abs(dir.x) > 0.1f)
            {
                float side = (dir.x < 0f) ? -1f : 1f;
                transform.localScale = new Vector3(Mathf.Abs(start_scale.x) * side, start_scale.y, start_scale.z);
            }
            
            //Vision angle
            if (dir.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x * GetSide()) * Mathf.Rad2Deg;
                Quaternion target = Quaternion.AngleAxis(angle, Vector3.forward);
                face_rot = Quaternion.RotateTowards(face_rot, target, rotate_speed * current_rot_mult * Time.deltaTime);
                face_vect = face_rot * Vector3.right;
                face_vect.x = face_vect.x * GetSide();
                face_vect.Normalize();
            }
        }

        private void UpdateAlert()
        {
            if (state_timer < alert_wait_time)
            {
                FaceToward(alert_target);
            }
            else if (state_timer < alert_wait_time + alert_walk_time)
            {
                MoveTo(alert_target, move_speed);
            }
        }

        private void UpdateConfused()
        {
            //Just wait
        }

        private void UpdateFollow()
        {
            Vector3 targ = follow_target ? follow_target.transform.position : last_seen_pos;

            //Use memory if no more target
            if (follow_target == null && last_target != null && memory_duration > 0.1f)
            {
                memory_timer += Time.deltaTime;
                if (memory_timer < memory_duration)
                {
                    last_seen_pos = last_target.transform.position;
                    targ = last_seen_pos;
                }
            }

            //Move to target
            MoveTo(targ, run_speed);
            FaceToward(GetNextTarget(), 2f);

            if (follow_target != null)
            {
                last_target = follow_target;
                last_seen_pos = follow_target.transform.position;
                memory_timer = 0f;
            }
        }

        void UpdatePatrol()
        {
            move_dir = Vector3.right * Mathf.Sign(transform.localScale.x);

            //If still in starting path
            if (!waiting && !HasFallen() && path_list.Count > 1)
            {
                //Move
                Vector3 targ = path_list[current_path];
                MoveTo(targ, move_speed);
                move_dir = Vector3.right * Mathf.Sign((targ - transform.position).x);

                //Check if reached target
                Vector3 dist_vect = (targ - transform.position);
                dist_vect.z = 0f;
                if (dist_vect.magnitude < 0.1f)
                {
                    waiting = true;
                    wait_timer = 0f;
                }

                //Check if obstacle ahead
                bool fronted = CheckFronted(dist_vect.normalized);
                if (fronted && wait_timer > 2f)
                {
                    RewindPath();
                    wait_timer = 0f;
                }
            }

            //If can't reach starting path anymore
            if (!waiting && HasFallen())
            {
                //Move
                Vector3 mdir = Vector3.right * (path_rewind ? -2f : 2f);
                Vector3 targ = transform.position + mdir;
                MoveTo(targ, move_speed);
                FaceToward(targ);
                move_dir = Vector3.right * Mathf.Sign((targ - transform.position).x);

                //Check if obstacle ahead
                Vector3 dist_vect = (targ - transform.position);
                bool fronted = CheckFronted(dist_vect.normalized);
                if (fronted && wait_timer > 2f)
                {
                    path_rewind = !path_rewind;
                    wait_timer = 0f;
                }
            }

            if (waiting)
            {
                //Wait a bit
                if (wait_timer > wait_time)
                {
                    GoToNextPath();
                    waiting = false;
                    wait_timer = 0f;
                }
            }

            //Angle
            pause_timer += Time.deltaTime;
            if (pause_timer > angle_pause)
            {
                float angle_target = angle_rewind ? angle_min : angle_max;
                current_angle = Mathf.MoveTowards(current_angle, angle_target, angle_speed * Time.deltaTime);
                face_dir = new Vector3(Mathf.Cos(current_angle * Mathf.Deg2Rad), Mathf.Sin(current_angle * Mathf.Deg2Rad), 0f);
                face_dir = new Vector3(Mathf.Sign(move_dir.x) * face_dir.x, face_dir.y, 0f);
            }

            FaceToward(transform.position + face_dir * 2f);

            if (!angle_rewind && GetFacingAngle() >= angle_max - 0.02f)
            {
                angle_rewind = true;
                pause_timer = 0f;
            }
            if (angle_rewind && GetFacingAngle() <= angle_min + 0.02f)
            {
                angle_rewind = false;
                pause_timer = 0f;
            }
        }

        //---- Patrol ------

        private void RewindPath()
        {
            path_rewind = !path_rewind;
            current_path += path_rewind ? -1 : 1;
            current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
        }

        private void GoToNextPath()
        {
            if (current_path <= 0 || current_path >= path_list.Count - 1)
                path_rewind = !path_rewind;
            current_path += path_rewind ? -1 : 1;
            current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
        }

        //------  Chase ------

        public void SetAlertTarget(Vector3 target)
        {
            alert_target = target;
        }

        public void SetFollowTarget(GameObject atarget)
        {
            follow_target = atarget;
            if (follow_target != null)
            {
                last_seen_pos = follow_target.transform.position;
                memory_timer = 0f;
            }
        }

        //------  Actions ------

        public void Alert(Vector3 target)
        {
            if (state != EnemyState2D.Chase)
            {
                ChangeState(EnemyState2D.Alert);
                SetAlertTarget(target);
                StopMove();
            }
        }

        public void Follow(GameObject target)
        {
            ChangeState(EnemyState2D.Chase);
            SetFollowTarget(target);
        }

        public void MoveTo(Vector3 pos, float speed_mult = 1f)
        {
            move_target = pos;
            current_speed = speed_mult;
        }

        public void StopMove()
        {
            move_target = rigid.position;
            rigid.velocity = Vector3.zero;
        }

        public void FaceToward(Vector3 pos, float speed_mult = 1f)
        {
            current_rot_target = pos;
            current_rot_mult = speed_mult;
        }

        public void Kill()
        {
            if (onDeath != null)
                onDeath.Invoke();

            Destroy(gameObject);
        }

        public void ChangeState(EnemyState2D state)
        {
            this.state = state;
            state_timer = 0f;
            wait_timer = 0f;
            waiting = false;
        }

        public void Pause()
        {
            paused = true;
        }

        public void UnPause()
        {
            paused = false;
        }

        //---- Check state ------ 

        private bool DetectObstacle(Vector3 dir)
        {
            bool grounded = false;
            Vector2[] raycastPositions = new Vector2[3];

            Vector2 raycast_start = rigid.position;
            Vector2 orientation = dir.normalized;
            bool is_up_down = Mathf.Abs(orientation.y) > Mathf.Abs(orientation.x);
            Vector2 perp_ori = is_up_down ? Vector2.right : Vector2.up;
            float radius = GetSize().x * 0.5f;

            if (capsule_coll != null && is_up_down)
            {
                //Adapt raycast to collider
                Vector2 raycast_offset = capsule_coll.offset + orientation * Mathf.Abs(capsule_coll.size.y * 0.5f - capsule_coll.size.x * 0.5f);
                raycast_start = rigid.position + raycast_offset * capsule_coll.transform.lossyScale.y;
            }

            float ray_size = radius + ground_raycast_dist;
            raycastPositions[0] = raycast_start - perp_ori * radius / 2f;
            raycastPositions[1] = raycast_start;
            raycastPositions[2] = raycast_start + perp_ori * radius / 2f;


            for (int i = 0; i < raycastPositions.Length; i++)
            {
                Debug.DrawRay(raycastPositions[i], orientation * ray_size, Color.green);
                if (RaycastObstacle(raycastPositions[i], orientation * ray_size))
                    grounded = true;
            }
            return grounded;
        }

        public bool RaycastObstacle(Vector2 pos, Vector2 dir)
        {
            RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
            Physics2D.Raycast(pos, dir.normalized, contact_filter, hitBuffer, dir.magnitude);
            for (int j = 0; j < hitBuffer.Length; j++)
            {
                if (hitBuffer[j].collider != null && hitBuffer[j].collider != capsule_coll && !hitBuffer[j].collider.isTrigger)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckFronted(Vector3 dir)
        {
            return RaycastObstacle(transform.position, dir);
        }

        private void CheckIfFacingReachedTarget(Vector3 targ)
        {
            //Check if reached target
            Vector3 dist_vect = (targ - transform.position);
            dist_vect.z = 0f;
            float dot = Vector3.Dot(face_vect.normalized, dist_vect.normalized);
            if (dot > 0.99f)
            {
                waiting = true;
                wait_timer = 0f;
            }
        }

        //------  Getters ------

        public EnemyState2D GetState()
        {
            return state;
        }

        public float GetStateTimer()
        {
            return state_timer;
        }

        public Vector3 GetMove()
        {
            return move_vect;
        }

        public Vector3 GetFacing()
        {
            return face_vect;
        }

        public float GetFacingAngle()
        {
            return Mathf.Atan2(face_vect.y, face_vect.x * GetSide()) * Mathf.Rad2Deg;
        }

        public float GetSide()
        {
            return Mathf.Sign(transform.localScale.x);
        }

        public Vector2 GetSize()
        {
            if (capsule_coll != null)
                return new Vector2(Mathf.Abs(capsule_coll.transform.lossyScale.x) * capsule_coll.size.x, Mathf.Abs(capsule_coll.transform.lossyScale.y) * capsule_coll.size.y);
            return new Vector2(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));
        }

        public bool IsRunning()
        {
            return state == EnemyState2D.Chase;
        }

        public Vector3 GetNextTarget()
        {
            return move_target;
        }

        public bool HasReachedTarget()
        {
            Vector3 targ = follow_target ? follow_target.transform.position : last_seen_pos;
            if (state == EnemyState2D.Alert)
                targ = alert_target;
            return (targ - transform.position).magnitude < 0.5f;
        }

        private Vector3 GetRandomLookTarget()
        {
            Vector3 center = transform.position;
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            return center + offset;
        }

        public float GetVisionAngle()
        {
            return current_angle;
        }

        public bool HasFallen()
        {
            float distY = Mathf.Abs(transform.position.y - start_pos.y);
            return distY > 0.5f;
        }

        public bool IsPaused()
        {
            return paused;
        }

        public static Enemy2D GetNearest(Vector3 pos, float range = 999f)
        {
            float min_dist = range;
            Enemy2D nearest = null;
            foreach (Enemy2D point in enemy_list)
            {
                float dist = (point.transform.position - pos).magnitude;
                if (dist < min_dist)
                {
                    min_dist = dist;
                    nearest = point;
                }
            }
            return nearest;
        }

        public static List<Enemy2D> GetAllInRange(Vector3 pos, float range)
        {
            List<Enemy2D> range_list = new List<Enemy2D>();
            foreach (Enemy2D enemy in enemy_list)
            {
                float dist = (enemy.transform.position - pos).magnitude;
                if (dist < range)
                {
                    range_list.Add(enemy);
                }
            }
            return range_list;
        }

        public static List<Enemy2D> GetAll()
        {
            return enemy_list;
        }

        //--- Debug Gizmos -----

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 prev_pos = transform.position;

            foreach (GameObject patrol in patrol_targets)
            {
                if (patrol)
                {
                    Gizmos.DrawLine(prev_pos, patrol.transform.position);
                    prev_pos = patrol.transform.position;
                }
            }
        }
    }

}