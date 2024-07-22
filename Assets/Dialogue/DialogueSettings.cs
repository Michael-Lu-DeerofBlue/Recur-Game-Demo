using UnityEngine;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Kamgam.SettingsGenerator;
using PixelCrushers.DialogueSystem;

public class DialogueSettings : MonoBehaviour
{
    
    private void FixedUpdate()
    {
        var settings = SettingsInitializer.Settings; 
        SettingFloat textSpeed = settings.GetFloat(id: "textSpeed");
        DialogueManager.DisplaySettings.subtitleSettings.subtitleCharsPerSecond = textSpeed.GetFloatValue();
    }

}
