using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Adds the light to the list of lights in LightDetector.<br />
    /// Use this for dynamically created lights or if LightDetector.ScanAfterSceneLoad
    /// is turned off.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class LightDetectorComponent : MonoBehaviour
    {
        public void Awake()
        {
            LightDetector.Instance.Add(this.GetComponent<Light>());
        }
    }
}
