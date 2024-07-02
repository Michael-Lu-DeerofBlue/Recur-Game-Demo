using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// This class is nothing but a marker for the CameraFOVConnection to get all the
    /// cameras that should be affected by the FOV setting.
    /// </summary>
    public class FieldOfViewMarker : MonoBehaviour
    {
        public static List<FieldOfViewMarker> Markers = new List<FieldOfViewMarker>();

        public static bool HasValidMarkers()
        {
            foreach (var marker in Markers)
            {
                if (marker.IsValid())
                    return true;
            }

            return false;
        }

        public static FieldOfViewMarker GetFirstValidMarker()
        {
            foreach (var marker in Markers)
            {
                if (marker.IsValid())
                    return marker;
            }

            return null;
        }

        protected Camera _camera;
        public Camera Camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = this.GetComponent<Camera>();
                }
                return _camera;
            }
        }

        public void Awake()
        {
            Markers.Add(this);
        }

        public void OnDestroy()
        {
            Markers.Remove(this);
        }

        public bool IsValid()
        {
            return isActiveAndEnabled && gameObject != null && gameObject.activeInHierarchy && Camera != null;
        }
    }
}
