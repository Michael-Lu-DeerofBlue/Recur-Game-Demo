using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace VolumetricFogAndMist2 {

    public static class Tools {

        public static Color ColorBlack = Color.black;

        public static void CheckCamera(ref Camera cam) {
            if (cam != null) return;
            cam = Camera.main;
            if (cam == null) {
                Camera[] cameras = Misc.FindObjectsOfType<Camera>();
                for (int k = 0; k < cameras.Length; k++) {
                    if (cameras[k].isActiveAndEnabled && cameras[k].gameObject.activeInHierarchy) {
                        cam = cameras[k];
                        return;
                    }

                }
            }
        }

        public static VolumetricFogManager CheckMainManager() {
            VolumetricFogManager fogManager = VolumetricFogManager.GetManagerIfExists();
            if (fogManager == null) {
                GameObject go = new GameObject();
                fogManager = go.AddComponent<VolumetricFogManager>();
                go.name = fogManager.managerName;
#if UNITY_EDITOR
            if (UnityEditor.SceneManagement.StageUtility.GetCurrentStage() != UnityEditor.SceneManagement.StageUtility.GetMainStage()) {
                UnityEditor.SceneManagement.StageUtility.PlaceGameObjectInCurrentStage(go);
            }
            Undo.RegisterCreatedObjectUndo(go, "Create Volumetric Fog Manager");
#endif
            }
            return fogManager;
        }


        public static void CheckManager<T>(ref T manager) where T : Component {
            if (manager == null) {
                VolumetricFogManager root = CheckMainManager();
                if (root == null) return;
                manager = root.GetComponentInChildren<T>(true);
                if (manager == null) {
                    GameObject o = new GameObject();
                    o.transform.SetParent(root.transform, false);
                    manager = o.AddComponent<T>();
                    o.name = ((IVolumetricFogManager)manager).managerName;
#if UNITY_EDITOR
                        Undo.RegisterCreatedObjectUndo(o, "Create Fog Manager");
#endif

                }
            }
        }


        static Mesh _fullScreenMesh;

        public static Mesh fullscreenMesh {
            get {
                if (_fullScreenMesh != null) {
                    return _fullScreenMesh;
                }
                float num = 1f;
                float num2 = 0f;
                Mesh val = new Mesh();
                _fullScreenMesh = val;
                _fullScreenMesh.SetVertices(new List<Vector3> {
            new Vector3 (-1f, -1f, 0f),
            new Vector3 (-1f, 1f, 0f),
            new Vector3 (1f, -1f, 0f),
            new Vector3 (1f, 1f, 0f)
        });
                _fullScreenMesh.SetUVs(0, new List<Vector2> {
            new Vector2 (0f, num2),
            new Vector2 (0f, num),
            new Vector2 (1f, num2),
            new Vector2 (1f, num)
        });
                _fullScreenMesh.SetIndices(new int[6] { 0, 1, 2, 2, 1, 3 }, (MeshTopology)0, 0, false);
                _fullScreenMesh.UploadMeshData(true);
                return _fullScreenMesh;
            }
        }

    }

}