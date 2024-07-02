using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class CameraDetector : MonoBehaviour
    {
        public delegate void OnNewCameraFoundDelegate(Camera cam);

        /// <summary>
        /// Called if a new camera is found.
        /// </summary>
        public OnNewCameraFoundDelegate OnNewCameraFound;

        private static CameraDetector _instance;
        public static CameraDetector Instance
        {
            get
            {
                if (!_instance)
                {
#if UNITY_EDITOR
                    // Keep the instance null outside of play mode to avoid leaking
                    // instances into the scene.
                    if (!UnityEditor.EditorApplication.isPlaying)
                    {
                        return null;
                    }
#endif

                    _instance = new GameObject().AddComponent<CameraDetector>();
                    _instance.name = _instance.GetType().ToString();

#if UNITY_EDITOR
                    _instance.hideFlags = HideFlags.DontSave;
                    if (UnityEditor.EditorApplication.isPlaying)
                    {
#endif
                        DontDestroyOnLoad(_instance.gameObject);
#if UNITY_EDITOR
                    }
#endif
                }
                return _instance;
            }
        }

        protected Camera[] _previousCameras = new Camera[10] { null, null, null, null, null, null, null, null, null, null };
        protected Camera[] _cameras = new Camera[10] { null, null, null, null, null, null, null, null, null, null };
        public Camera[] Cameras
        {
            get => _cameras;
        }

        private CameraDetector() {}

        void Update()
        {
            Camera cam;

            // copy current to previous and clean current
            for (int i = 0; i < _cameras.Length; i++)
            {
                _previousCameras[i] = _cameras[i];
                _cameras[i] = null;
            }

            // save in current cameras
            increaseCapacity();
            Camera.GetAllCameras(_cameras);

            for (int i = 0; i < _cameras.Length; i++)
            {
                // new camera?
                cam = _cameras[i];
                if (cam != null && !contains(_previousCameras, cam))
                {
                    OnNewCameraFound?.Invoke(cam);
                }
            }
        }

        protected void increaseCapacity()
        {
            if (_cameras.Length < Camera.allCamerasCount)
            {
                // Create new and init with null
                var newCameras = new Camera[Camera.allCamerasCount];
                var newPreviousCameras = new Camera[Camera.allCamerasCount];
                for (int i = 0; i < newCameras.Length; i++)
                {
                    newCameras[i] = null;
                    newPreviousCameras[i] = null;
                }

                // Copy previous
                for (int i = 0; i < _previousCameras.Length; i++)
                {
                    newPreviousCameras[i] = _previousCameras[i];
                }

                // Copy current
                /* Unnecessary in this context */
                /*
                for (int i = 0; i < _cameras.Length; i++)
                {
                    newCameras[i] = _cameras[i];
                }
                */

                _cameras = newCameras;
                _previousCameras = newPreviousCameras;
            }
        }

        protected bool contains(Camera[] cameras, Camera cam)
        {
            for (int i = 0; i < cameras.Length; i++)
                if (cameras[i] == cam)
                    return true;

            return false;
        }
    }
}
