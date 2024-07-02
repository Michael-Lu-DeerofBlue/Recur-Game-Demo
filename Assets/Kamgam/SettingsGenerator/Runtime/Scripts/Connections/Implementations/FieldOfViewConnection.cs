using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class FieldOfViewConnection : Connection<float>
    {
        public const float DefaultFallback = 60f;

        public bool UseMain;
        public bool UseMarkers;

        // Cache for the fov value in case no camera exists yet.
        [System.NonSerialized]
        protected float _fieldOfView = DefaultFallback;

        public FieldOfViewConnection(bool useMain = true, bool useMarkers = true)
        {
            UseMain = useMain;
            UseMarkers = useMarkers;

            CameraDetector.Instance.OnNewCameraFound += onNewCamera;
        }

        protected void onNewCamera(Camera cam)
        {
            Apply();
        }

        public void Apply()
        {
            Set(_fieldOfView);
        }

        public override float Get()
        {
            if (UseMain && Camera.main != null)
            {
                return Camera.main.fieldOfView;
            }
            
            if (UseMarkers)
            {
                var marker = FieldOfViewMarker.GetFirstValidMarker();
                if(marker != null)
                {
                    return marker.Camera.fieldOfView;
                }
            }

            return _fieldOfView;
        }

        public override void Set(float fieldOfView)
        {
            _fieldOfView = fieldOfView;

            if (UseMain && Camera.main != null)
            {
                Camera.main.fieldOfView = fieldOfView;
            }
            else if (UseMarkers)
            {
                foreach (var marker in FieldOfViewMarker.Markers)
                {
                    if (marker.IsValid())
                    {
                        marker.Camera.fieldOfView = fieldOfView;
                    }
                }
            }
        }
    }
}
