using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace IndieMarc.EnemyVision
{
    public enum EnemyState
    {
        None = 0,
        Patrol = 2,
        Alert = 5,
        Chase = 10,
        Confused = 15, //After lost track of target
        Wait=20,
    }

    public enum EnemyPatrolType
    {
        Rewind = 0,
        Loop = 2,
        FacingOnly = 5,
    }

    /// <summary>
    /// Handles enemy movement and the different enemy behaviors
    /// </summary>

    [RequireComponent(typeof(Rigidbody))]
    public class Enemy : MonoBehaviour
    {
        public float move_speed = 2f;
        public float run_speed = 4f;
        public float rotate_speed = 120f;
        public float fall_speed = 5f;
        public LayerMask obstacle_mask = ~(0);
        public bool use_pathfind = false;

        [Header("State")]
        public EnemyState state = EnemyState.Patrol;

        [Header("Patrol")]
        public EnemyPatrolType type;
        public float wait_time = 1f;
        public GameObject[] patrol_path;

        [Header("Alert")]
        public float alert_wait_time = 3f;
        public float alert_walk_time = 10f;

        [Header("Follow")]
        public GameObject follow_target;
        public float memory_duration = 4f;
        
        public UnityAction onDeath;

        private Rigidbody rigid;
        private NavMeshAgent nav_agent;

        private Vector3 start_pos;
        private Vector3 move_vect;
        private Vector3 face_vect;
        private float rotate_val;
        private float state_timer = 0f;
        private bool paused = false;

        private Vector3 move_target;
        private Vector3 alert_target;
        private float current_speed = 1f;
        private Vector3 current_move;
        private Vector3 current_rot_target;
        private float current_rot_mult = 1f;
        private bool waiting = false;
        private float wait_timer = 0f;

        private int current_path = 0;
        private bool path_rewind = false;
        private bool using_navmesh = false;

        private Vector3 last_seen_pos;
        private GameObject last_target;
        private float memory_timer = 0f;

        private List<Vector3> path_list = new List<Vector3>();

        private static List<Enemy> enemy_list = new List<Enemy>();

        private void Awake()
        {
            enemy_list.Add(this);
            rigid = GetComponent<Rigidbody>();
            nav_agent = GetComponent<NavMeshAgent>();
            move_vect = Vector3.zero;
            start_pos = transform.position;
            move_target = transform.position;
            current_rot_target = transform.position + transform.forward;
            alert_target = follow_target ? follow_target.transform.position : transform.position;
            last_seen_pos = transform.position;
            current_speed = move_speed;
            rotate_val = 0f;
            RefreshPatrol();

            current_path = 0;
            if (path_list.Count >= 2)
                current_path = 1; //Dont start at start pos
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

            if (type != EnemyPatrolType.FacingOnly)
                path_list.Add(transform.position);

            foreach (GameObject patrol in patrol_path)
            {
                if (patrol)
                    path_list.Add(patrol.transform.position);
            }
        }

        private void FixedUpdate()
        {
            if (paused)
                return;

            bool fronted = CheckFronted(transform.forward);
            bool grounded = CheckGrounded(Vector3.down);

            Vector3 dist_vect = (move_target - transform.position);
            move_vect = dist_vect.normalized * current_speed * Mathf.Min(dist_vect.magnitude, 1f);

            if (use_pathfind && nav_agent && using_navmesh && dist_vect.magnitude > 1f)
            {
                nav_agent.enabled = true;
                nav_agent.speed = current_speed;
                nav_agent.SetDestination(move_target);
                rigid.velocity = Vector3.zero;
            }
            else
            {
                if (fronted)
                    move_vect = Vector3.zero;
                if (!grounded)
                    move_vect += Vector3.down * fall_speed;

                if (nav_agent && nav_agent.enabled)
                    nav_agent.enabled = false;

                current_move = Vector3.MoveTowards(current_move, move_vect, move_speed * 10f * Time.fixedDeltaTime);
                rigid.velocity = current_move;
            }
        }

        private void Update()
        {
            if (paused)
                return;

            state_timer += Time.deltaTime;
            wait_timer += Time.deltaTime;

            if (state == EnemyState.Alert)
            {
                UpdateAlert();
            }

            if (state == EnemyState.Patrol)
            {
                UpdatePatrol();
            }

            if (state == EnemyState.Chase)
            {
                UpdateFollow();
            }

            if (state == EnemyState.Confused)
            {
                UpdateConfused();
            }

            //Manual Rotation
            bool controlled_by_agent = use_pathfind && nav_agent && nav_agent.enabled && using_navmesh && nav_agent.hasPath;
            rotate_val = 0f;

            if (!controlled_by_agent && state != EnemyState.None && state != EnemyState.Wait)
            {
                Vector3 dir = current_rot_target - transform.position;
                dir.y = 0f;
                if (dir.magnitude > 0.1f)
                {
                    Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
                    Quaternion reachedRotation = Quaternion.RotateTowards(transform.rotation, target, rotate_speed * current_rot_mult * Time.deltaTime);
                    rotate_val = Quaternion.Angle(transform.rotation, target);
                    face_vect = dir.normalized;
                    transform.rotation = reachedRotation;
                }
            }
        }

        private void UpdateAlert()
        {
            if (state_timer < alert_wait_time)
            {
                FaceToward(alert_target);
            }
            else if(state_timer < alert_wait_time + alert_walk_time)
            {
                MoveTo(alert_target, move_speed);
            }
        }

        private void UpdateConfused()
        {
            if (wait_timer > alert_wait_time)
            {
                wait_timer = 0f;
                waiting = false;
                alert_target = GetRandomLookTarget();
                MoveTo(alert_target);
            }
        }

        private void UpdatePatrol()
        {
            bool facing_only = type == EnemyPatrolType.FacingOnly;
            float dist = (transform.position - start_pos).magnitude;
            bool is_far = dist > 0.5f;

            //Facing only
            if (!waiting && facing_only)
            {
                if (is_far)
                {
                    //Return to starting pos
                    MoveTo(start_pos, move_speed);
                    FaceToward(start_pos);
                }
                else
                {
                    //Rotate only
                    Vector3 targ = path_list[current_path];
                    FaceToward(targ);
                    CheckIfFacingReachedTarget(targ);
                }
            }

            //Regular patrol
            if (!waiting && !facing_only)
            {
                //Move following path
                Vector3 targ = path_list[current_path];
                MoveTo(targ, move_speed);
                FaceToward(GetNextTarget());

                //Check if reached target
                Vector3 dist_vect = (targ - transform.position);
                dist_vect.y = 0f;
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

            //Waiting
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

        //---- Patrol -----

        private void RewindPath()
        {
            if (type != EnemyPatrolType.FacingOnly)
            {
                path_rewind = !path_rewind;
                current_path += path_rewind ? -1 : 1;
                current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
            }
        }

        private void GoToNextPath()
        {
            if (type == EnemyPatrolType.FacingOnly)
            {
                if (current_path <= 0 || current_path >= path_list.Count - 1)
                    path_rewind = !path_rewind;
                current_path += path_rewind ? -1 : 1;
                current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
            }
            else if (type == EnemyPatrolType.Loop)
            {
                current_path = (current_path + 1) % path_list.Count;
                current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
            }
            else
            {
                if (current_path <= 0 || current_path >= path_list.Count - 1)
                    path_rewind = !path_rewind;
                current_path += path_rewind ? -1 : 1;
                current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
            }
        }

        //---- Chase -----

        public void SetAlertTarget(Vector3 pos)
        {
            alert_target = pos;
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

        //---- Actions -----

        public void Alert(Vector3 pos)
        {
            if (state != EnemyState.Chase)
            {
                ChangeState(EnemyState.Alert);
                SetAlertTarget(pos);
                StopMove();
            }
        }

        public void Follow(GameObject target)
        {
            ChangeState(EnemyState.Chase);
            SetFollowTarget(target);
            using_navmesh = true;
        }

        public void MoveTo(Vector3 pos, float speed = 1f)
        {
            move_target = pos;
            current_speed = speed;
            using_navmesh = true;
        }

        public void FaceToward(Vector3 pos, float speed_mult = 1f)
        {
            current_rot_target = pos;
            current_rot_mult = speed_mult;
        }

        public void StopMove()
        {
            using_navmesh = false;
            move_target = rigid.position;
            current_move = Vector3.zero;
            rigid.velocity = Vector3.zero;
            if (nav_agent && nav_agent.enabled)
                nav_agent.ResetPath(); //Cancel previous path
        }

        public void Kill()
        {
            if (onDeath != null)
                onDeath.Invoke();

            Destroy(gameObject);
        }

        public void ChangeState(EnemyState state)
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

        //---- Check state -----

        public bool CheckFronted(Vector3 dir)
        {
            Vector3 origin = transform.position + Vector3.up * 1f;
            RaycastHit hit;
            bool success = Physics.Raycast(new Ray(origin, dir.normalized), out hit, dir.magnitude, obstacle_mask.value);
            return success && (follow_target == null || !hit.collider.transform.IsChildOf(follow_target.transform));
        }

        public bool CheckGrounded(Vector3 dir)
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            RaycastHit hit;
            return Physics.Raycast(new Ray(origin, dir.normalized), out hit, dir.magnitude, obstacle_mask.value);
        }

        private void CheckIfFacingReachedTarget(Vector3 targ)
        {
            //Check if reached target
            Vector3 dist_vect = (targ - transform.position);
            dist_vect.y = 0f;
            float dot = Vector3.Dot(transform.forward, dist_vect.normalized);
            if (dot > 0.99f)
            {
                waiting = true;
                wait_timer = 0f;
            }
        }

        //---- Getters ------

        public bool HasReachedTarget()
        {
            Vector3 targ = follow_target ? follow_target.transform.position : last_seen_pos;
            if (state == EnemyState.Alert)
                targ = alert_target;
            return (targ - transform.position).magnitude < 0.5f;
        }

        private Vector3 GetRandomLookTarget()
        {
            Vector3 center = transform.position;
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            return center + offset;
        }

        public EnemyState GetState()
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

        public float GetRotationVelocity()
        {
            return rotate_val;
        }

        public bool IsRunning()
        {
            return state == EnemyState.Chase;
        }

        public Vector3 GetNextTarget()
        {
            if (use_pathfind && nav_agent && nav_agent.enabled && using_navmesh && nav_agent.hasPath)
                return nav_agent.nextPosition;
            return move_target;
        }

        public bool IsPaused()
        {
            return paused;
        }

        public static Enemy GetNearest(Vector3 pos, float range = 999f)
        {
            float min_dist = range;
            Enemy nearest = null;
            foreach (Enemy enemy in enemy_list)
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

        public static List<Enemy> GetAllInRange(Vector3 pos, float range)
        {
            List<Enemy> range_list = new List<Enemy>();
            foreach (Enemy enemy in enemy_list)
            {
                float dist = (enemy.transform.position - pos).magnitude;
                if (dist < range)
                {
                    range_list.Add(enemy);
                }
            }
            return range_list;
        }

        public static List<Enemy> GetAll()
        {
            return enemy_list;
        }

        //----- Debug Gizmos -------

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 prev_pos = transform.position;

            if (type != EnemyPatrolType.FacingOnly)
            {
                foreach (GameObject patrol in patrol_path)
                {
                    if (patrol)
                    {
                        Gizmos.DrawLine(prev_pos, patrol.transform.position);
                        prev_pos = patrol.transform.position;
                    }
                }

                if (type == EnemyPatrolType.Loop)
                    Gizmos.DrawLine(prev_pos, transform.position);
            }

            if (type == EnemyPatrolType.FacingOnly)
            {
                foreach (GameObject patrol in patrol_path)
                {
                    if (patrol)
                    {
                        Gizmos.DrawLine(transform.position, patrol.transform.position);
                    }
                }
            }
        }
    }

}