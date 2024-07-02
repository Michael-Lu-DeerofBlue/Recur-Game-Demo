using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator.Examples
{
    [ExecuteAlways]
    public class UIToolkitDemoNotice : MonoBehaviour
    {
        public void Start()
        {
#if UNITY_EDITOR
#if !UNITY_2021_2_OR_NEWER
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !SessionState.GetBool("UIToolkitDemoNoticeShown", false))
            {
                SessionState.SetBool("UIToolkitDemoNoticeShown", true);
                EditorUtility.DisplayDialog("UI Toolkit NOT supported!", "UI Toolkit is only fully supported in Unity 2021.2.0 or newer. Please uprade your project to a newer version. This demo will most likely not work properly.", "OK");
            }
#endif
#endif
        }
    }
}
