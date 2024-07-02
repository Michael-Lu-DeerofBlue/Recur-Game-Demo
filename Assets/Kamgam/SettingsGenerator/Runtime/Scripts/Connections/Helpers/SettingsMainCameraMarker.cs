using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this to mark a camera as the MAIN CAMERA that settings should be applied to.<br />
/// Add this as a component to a camera and enable it.
/// </summary>
public class SettingsMainCameraMarker : MonoBehaviour
{
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SettingsMainCameraMarker))]
public class SettingsMainCameraMarkerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        UnityEditor.EditorGUILayout.HelpBox("Use this to mark a camera as the MAIN CAMERA that settings should be applied to.", UnityEditor.MessageType.Info);
    }
}
#endif
