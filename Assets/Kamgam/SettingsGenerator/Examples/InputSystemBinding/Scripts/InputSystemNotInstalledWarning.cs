using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{ 
    [ExecuteAlways]
    public class InputSystemNotInstalledWarning : MonoBehaviour
    {
        public GameObject Target;

        void OnEnable()
        {
#if !ENABLE_INPUT_SYSTEM
            Target.SetActive(true);
#else
            Target.SetActive(false);
#endif 

#if !UNITY_EDITOR
            Target.SetActive(false);
#endif
        }
    }
}
