using Kamgam.LocalizationForSettings;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CustomEditor(typeof(SettingEvent), editorForChildClasses: true)]
    public class SettingEventEditor : SettingResolverEditor
    {
        protected SettingEvent _evt;

        public override bool AlwaysAutoAssignProvider => true;

        public override void OnEnable()
        {
            base.OnEnable();
            _evt = target as SettingEvent;
        }

        public override void OnInspectorGUI()
        {
            // Exclude some from showing in the inspector
            _fieldsToHide.Clear();
            _fieldsToHide.Add("m_LocalizationProvider");
            _fieldsToHide.Add("m_Calls");
            _fieldsToHide.Add("m_PersistentCalls");

            base.OnInspectorGUI();

            if(GUILayout.Button("Trigger Event"))
            {
                _evt.TriggerEvent();
            }
        }
    }
}
