using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.EnemyVision
{
    /// <summary>
    /// Call this manually to generate noise
    /// </summary>

    public class NoiseSource2D : MonoBehaviour
    {
        public float alert_range = 20f;

        public GameObject noise_fx;

        private AudioSource audio_source;

        private void Awake()
        {
            audio_source = GetComponent<AudioSource>();
        }

        public void TriggerNoise()
        {
            List<EnemyVision2D> list = EnemyVision2D.GetAllInRange(transform.position, alert_range);
            foreach (EnemyVision2D enemy in list)
            {
                enemy.Alert(transform.position);
            }

            if (noise_fx != null)
                Instantiate(noise_fx, transform.position + Vector3.up * 1f, noise_fx.transform.rotation);

            if (audio_source != null)
                audio_source.Play();
        }
    }

}
