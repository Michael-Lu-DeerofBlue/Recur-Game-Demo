using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.EnemyVision
{
    /// <summary>
    /// Will generate noise when the player steps on it
    /// </summary>
    
    public class NoiseZone : MonoBehaviour
    {
        public float alert_range = 20f;
        public float cooldown = 4f;

        public GameObject noise_fx;

        private AudioSource audio_source;
        private float timer = 0f;

        void Awake()
        {
            audio_source = GetComponent<AudioSource>();
        }

        void Update()
        {
            timer += Time.deltaTime;
        }

        public void TriggerNoise()
        {
            timer = -cooldown;

            List<EnemyVision> list = EnemyVision.GetAllInRange(transform.position, alert_range);
            foreach (EnemyVision enemy in list)
            {
                enemy.Alert(transform.position);
            }

            if (noise_fx != null)
                Instantiate(noise_fx, transform.position + Vector3.up * 1f, noise_fx.transform.rotation);

            if (audio_source != null)
                audio_source.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (timer > 0f && other.GetComponent<VisionTarget>())
            {
                TriggerNoise();
            }
        }
    }

}
