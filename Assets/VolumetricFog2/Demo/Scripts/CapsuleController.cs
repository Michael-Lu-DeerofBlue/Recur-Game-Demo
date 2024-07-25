using UnityEngine;
using VolumetricFogAndMist2;

namespace VolumetricFogAndMist2.Demos {

    public class CapsuleController : MonoBehaviour {

        public VolumetricFog fogVolume;
        public float moveSpeed = 10f;
        public float fogHoleRadius = 8f;
        public float clearDuration = 0.2f;
        public float distanceCheck = 1f;

        Vector3 lastPos = new Vector3(float.MaxValue, 0, 0);

        void Update() {

            float disp = Time.deltaTime * moveSpeed;

            // moves capsule with arrow keys
            if (Input.GetKey(KeyCode.LeftArrow)) {
                transform.Translate(-disp, 0, 0);
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                transform.Translate(disp, 0, 0);
            }
            if (Input.GetKey(KeyCode.UpArrow)) {
                transform.Translate(0, 0, disp);
            } else if (Input.GetKey(KeyCode.DownArrow)) {
                transform.Translate(0, 0, -disp);
            }

            // do not call SetFogOfWarAlpha every frame; only when capsule moves
            if ((transform.position - lastPos).magnitude > distanceCheck) {
                lastPos = transform.position;
                fogVolume.SetFogOfWarAlpha(transform.position, radius: fogHoleRadius, fogNewAlpha: 0, duration: clearDuration);
            }

        }
    }

}