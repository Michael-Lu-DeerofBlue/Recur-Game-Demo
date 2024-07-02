using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIComponentsForSettings
{
    [CustomEditor(typeof(AutoNavigationOverrides))]
    public class NavigationOverridesEditor : Editor
    {
        public AutoNavigationOverrides overrides;

        public void OnEnable()
        {
            overrides = target as AutoNavigationOverrides;
        }

        override public void OnInspectorGUI()
        {
            if (EditorApplication.isPlaying)
            {
                if (!overrides.enabled)
                {
                    EditorGUILayout.HelpBox(new GUIContent("The component disabled itself in Awake() since there were no overrides set."));
                    if (GUILayout.Button("Enable"))
                    {
                        overrides.enabled = true;
                    }
                    GUI.enabled = false;
                }
            }
            else
            {
                if (overrides.DisableOnAwakeIfNotNeeded && !overrides.HasOverrides() && !overrides.IsBlockingAnyDirection)
                {
                    EditorGUILayout.HelpBox(new GUIContent("This component will disable itself in Awake() because there are no overrides configured."));
                }
            }

            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Apply Overrides"))
                {
                    overrides.ApplyOverrides();
                    markAsChangedIfEditing();
                }
            }
            else
            {
                GUILayout.Label("Auto fill:");
                GUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Up"))
                {
                    overrides.SelectOnUpOverride = overrides.Selectable.FindSelectableOnUp();
                    markAsChangedIfEditing();
                }
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    overrides.SelectOnUpOverride = null;
                    markAsChangedIfEditing();
                }

                if (GUILayout.Button("Down"))
                {
                    overrides.SelectOnDownOverride = overrides.Selectable.FindSelectableOnDown();
                    markAsChangedIfEditing();
                }
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    overrides.SelectOnDownOverride = null;
                    markAsChangedIfEditing();
                }

                if (GUILayout.Button("Left"))
                {
                    overrides.SelectOnLeftOverride = overrides.Selectable.FindSelectableOnLeft();
                    markAsChangedIfEditing();
                }
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    overrides.SelectOnLeftOverride = null;
                    markAsChangedIfEditing();
                }

                if (GUILayout.Button("Right"))
                {
                    overrides.SelectOnRightOverride = overrides.Selectable.FindSelectableOnRight();
                    markAsChangedIfEditing();
                }
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    overrides.SelectOnRightOverride = null;
                    markAsChangedIfEditing();
                }

                GUILayout.EndHorizontal();
            }
        }

        protected void markAsChangedIfEditing()
        {
            if (EditorApplication.isPlaying)
                return;

            Undo.RegisterCompleteObjectUndo(overrides, "Changed auto nav override.");

            // Schedule an update to the scene view will rerender (otherwise
            // the change would not be visible unless clicked into the scene view).
            EditorApplication.QueuePlayerLoopUpdate();

            // Make sure the scene can be saved
            EditorUtility.SetDirty(overrides);

            // Make sure the Prefab recognizes the changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(overrides);
        }
    }
}
