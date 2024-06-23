using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndieMarc.EnemyVision
{
    /// <summary>
    /// Demo script to move and control the player character
    /// </summary>
    
    public class CharacterControls2D : MonoBehaviour
    {
        public float move_speed = 7f;
        public float move_accel = 40f;

        [Header("Hide")]
        public bool can_hide = true;
        public KeyCode hide_key = KeyCode.LeftShift;

        [Header("Jump")]
        public bool can_jump = true;
        public KeyCode jump_key = KeyCode.Space;
        public float jump_strength = 5f;
        public float jump_time_min = 0.4f;
        public float jump_time_max = 0.8f;
        public float jump_gravity = 5f;
        public float jump_fall_gravity = 10f;
        public float jump_move_percent = 0.75f;
        public LayerMask ground_mask = ~(0); //All bit 1
        public float ground_raycast_dist = 0.1f;

        public UnityAction onJump;
        public UnityAction onLand;

        private Rigidbody2D rigid;
        private Animator animator;
        private Collider2D collide;
        private VisionTarget vision_target;

        private CapsuleCollider2D capsule_coll;
        private ContactFilter2D contact_filter;
        private Vector3 start_scale;

        private Vector3 move_input;
        private Vector3 move;
        private bool was_grounded;
        private bool is_grounded;
        private bool is_ceiled;
        private bool is_fronted;
        private bool is_jumping;
        private bool is_dead = false;

        private float jump_timer = 0f;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            vision_target = GetComponent<VisionTarget>();
            collide = GetComponentInChildren<Collider2D>();
            capsule_coll = GetComponentInChildren<CapsuleCollider2D>();
            start_scale = transform.localScale;

            contact_filter = new ContactFilter2D();
            contact_filter.layerMask = ground_mask;
            contact_filter.useLayerMask = true;
            contact_filter.useTriggers = false;
        }
        
        //Handle physics
        void FixedUpdate()
        {
            if (is_dead)
                return;

            //Movement velocity
            float desiredSpeed = Mathf.Abs(move_input.x) > 0.1f ? move_input.x * move_speed : 0f;
            float acceleration = move_accel;
            acceleration = !is_grounded ? jump_move_percent * acceleration : acceleration;
            move.x = Mathf.MoveTowards(move.x, desiredSpeed, acceleration * Time.fixedDeltaTime);

            bool invisible = can_hide && Input.GetKey(hide_key);
            if (invisible)
                move.x = 0f;

            was_grounded = is_grounded;
            is_grounded = DetectObstacle(Vector3.down);
            is_ceiled = DetectObstacle(Vector3.up);
            is_fronted = DetectObstacle(GetFacing());

            UpdateFacing();
            UpdateJump();

            //Move
            move.x = is_fronted ? 0f : move.x;
            rigid.velocity = move;
            
        }

        //Handle render and controls
        void Update()
        {
            if (is_dead)
                return;

            move_input = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                move_input += Vector3.forward;
            if (Input.GetKey(KeyCode.A))
                move_input += Vector3.left;
            if (Input.GetKey(KeyCode.D))
                move_input += Vector3.right;
            if (Input.GetKey(KeyCode.S))
                move_input += Vector3.back;

            bool invisible = can_hide && Input.GetKey(hide_key);
            if (vision_target)
                vision_target.visible = !invisible;
            if (collide)
                collide.enabled = !invisible;
            
            //Controls
            if (!invisible && Input.GetKeyDown(jump_key))
                Jump();

            //Anim
            if (animator != null)
            {
                animator.SetBool("Move", is_grounded && move.magnitude > 0.1f);
                animator.SetBool("Hide", invisible);
            }
        }

        private void UpdateFacing()
        {
            if (Mathf.Abs(move.x) > 0.01f)
            {
                float side = (move.x < 0f) ? -1f : 1f;
                transform.localScale = new Vector3(Mathf.Abs(start_scale.x) * side, start_scale.y, start_scale.z);
            }
        }

        private void UpdateJump()
        {
            //Jump
            jump_timer += Time.fixedDeltaTime;

            //Jump end timer
            if (is_jumping && !Input.GetKey(jump_key) && jump_timer > jump_time_min)
                is_jumping = false;
            if (is_jumping && jump_timer > jump_time_max)
                is_jumping = false;

            //Jump hit ceil
            if (is_ceiled)
            {
                is_jumping = false;
                move.y = Mathf.Min(move.y, 0f);
            }

            //Add jump velocity
            if (!is_grounded)
            {
                //Falling
                float gravity = !is_jumping ? jump_fall_gravity : jump_gravity; //Gravity increased when going down
                move.y = Mathf.MoveTowards(move.y, -move_speed * 2f, gravity * Time.fixedDeltaTime);
            }
            else if (!is_jumping)
            {
                //Grounded
                move.y = 0f;
            }

            if (!was_grounded && is_grounded)
            {
                if (onLand != null)
                    onLand.Invoke();
            }
        }

        public void Jump()
        {
            if (can_jump)
            {
                if (is_grounded)
                {
                    move.y = jump_strength;
                    jump_timer = 0f;
                    is_jumping = true;
                    if (animator != null)
                        animator.SetTrigger("Jump");
                    if (onJump != null)
                        onJump.Invoke();
                }
            }
        }

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

        public Vector2 GetSize()
        {
            if (capsule_coll != null)
                return new Vector2(Mathf.Abs(capsule_coll.transform.lossyScale.x) * capsule_coll.size.x, Mathf.Abs(capsule_coll.transform.lossyScale.y) * capsule_coll.size.y);
            return new Vector2(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));
        }

        public Vector2 GetMove()
        {
            return move;
        }

        public Vector2 GetFacing()
        {
            return Vector2.right * Mathf.Sign(transform.localScale.x);
        }

        public bool IsJumping()
        {
            return is_jumping;
        }

        public bool IsGrounded()
        {
            return is_grounded;
        }

        public bool IsDead()
        {
            return is_dead;
        }
    }
}
