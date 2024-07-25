using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VolumetricFogAndMist2.Demos {

    public class DemoSceneControls : MonoBehaviour {

        public VolumetricFogProfile[] profiles;
        public VolumetricFog fogVolume;
        public Text presetNameDisplay;

        int index;

        void Start() {
            SetProfile(index);
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.F)) {
                index++;
                if (index >= profiles.Length) index = 0;
                SetProfile(index);
            }

            if (Input.GetKeyDown(KeyCode.T)) {
                fogVolume.gameObject.SetActive(!fogVolume.gameObject.activeSelf);
            }
        }

        void SetProfile(int profileIndex) {

            // move cloud profiles a bit up
            if (profileIndex < 2) {
                fogVolume.transform.position = Vector3.up * 25;
            } else {
                fogVolume.transform.position = Vector3.zero;
            }

            fogVolume.profile = profiles[profileIndex];
            presetNameDisplay.text = "Current fog preset: " + profiles[profileIndex].name;
            fogVolume.UpdateMaterialPropertiesNow();
        }
    }

}