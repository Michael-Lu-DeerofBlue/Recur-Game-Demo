using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.EnemyVision
{
    /// <summary>
    /// Demo script on how to link animations and use Enemy events
    /// </summary>
    
    [RequireComponent(typeof(EnemyVision2D))]
    public class EnemyDemo2D : MonoBehaviour
    {
        public GameObject exclama_prefab;
        public GameObject death_fx_prefab;

        private EnemyVision2D enemy;
        private Animator animator;
        

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            enemy = GetComponent<EnemyVision2D>();
            enemy.onDeath += OnDeath;
            enemy.onAlert += OnAlert;
            enemy.onSeeTarget += OnSeen;
            enemy.onDetectTarget += OnDetect;
            enemy.onTouchTarget += OnTouch;
            
        }

        void Update()
        {
            if (animator != null && enemy.GetEnemy() != null)
            {
                animator.SetBool("Move", enemy.GetEnemy().GetMove().magnitude > 0.5f);
                animator.SetBool("Run", enemy.GetEnemy().IsRunning());
            }
        }

        //Can be either because seen or heard noise
        private void OnAlert(Vector3 target)
        {
            if (exclama_prefab != null)
                Instantiate(exclama_prefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            if (animator != null)
                animator.SetTrigger("Surprised");
        }

        private void OnSeen(VisionTarget target, int distance)
        {
            //Add code for when target get seen and enemy get alerted, 0=touch, 1=near, 2=far, 3=other
        }

        private void OnDetect(VisionTarget target, int distance)
        {
            //Add code for when the enemy detect you as a threat (and start chasing), 0=touch, 1=near, 2=far, 3=other
        }

        private void OnTouch(VisionTarget target)
        {
            //Add code for when you get caught
        }

        private void OnDeath()
        {
            if (animator != null)
                animator.SetTrigger("Death");
            if(death_fx_prefab)
                Instantiate(death_fx_prefab, transform.position + Vector3.up * 0.5f, death_fx_prefab.transform.rotation);
        }
    }
}
