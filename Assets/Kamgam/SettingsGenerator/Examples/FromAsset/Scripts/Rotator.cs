using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    public class Rotator : MonoBehaviour
    {
        public float Speed = 1f;
        public Vector3 Axis = Vector3.up;

        void Update()
        {
            this.transform.Rotate(Axis, Speed * Time.deltaTime * 60f);
        }
    }
}
