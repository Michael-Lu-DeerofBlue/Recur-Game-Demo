#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Kamgam.SettingsGenerator
{
    [CustomEditor(typeof(InputBindingConnectionSO))]
    public class InputBindingConnectionSOEditor : Editor
    {
#if !ENABLE_INPUT_SYSTEM
        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
#else
        public InputBindingConnectionSO connectionSO;

        protected Vector2 _scrollPos;

        public void OnEnable()
        {
            connectionSO = target as InputBindingConnectionSO;
        }

        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (connectionSO.InputActionAsset != null)
            {
                string pathOfCurrentGUID = "";

                for (int i = 0; i < connectionSO.InputActionAsset.actionMaps.Count; i++)
                {
                    var actionMap = connectionSO.InputActionAsset.actionMaps[i];
                    for (int b = 0; b < connectionSO.InputActionAsset.actionMaps[i].bindings.Count; b++)
                    {
                        var binding = connectionSO.InputActionAsset.actionMaps[i].bindings[b];
                        if(binding.id.ToString() == connectionSO.BindingId)
                        {
                            string effPath = EditorApplication.isPlaying ? binding.effectivePath : binding.path;
                            string path = actionMap.name + "/" + binding.action + "/" + (binding.isComposite ? binding.name + " (" + effPath + ")" : effPath);
                            pathOfCurrentGUID = path;
                        }
                    }
                }

                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.alignment = TextAnchor.MiddleLeft;
                labelStyle.fontStyle = FontStyle.Bold;
                if (string.IsNullOrEmpty(pathOfCurrentGUID))
                {
                    labelStyle.normal.textColor = new Color(1f, 1f, 0.2f);
                    pathOfCurrentGUID = "<Unknown>";
                }

                GUILayout.Label("  ID ->  " + pathOfCurrentGUID, labelStyle);


                _scrollPos = GUILayout.BeginScrollView(_scrollPos);

                // Buttons
                GUIStyle contentStyle = new GUIStyle(GUI.skin.button);
                contentStyle.alignment = TextAnchor.MiddleLeft;

                GUILayout.Label("Available Bindings:");
                for (int i = 0; i < connectionSO.InputActionAsset.actionMaps.Count; i++)
                {
                    var actionMap = connectionSO.InputActionAsset.actionMaps[i];
                    for (int b = 0; b < connectionSO.InputActionAsset.actionMaps[i].bindings.Count; b++)
                    {
                        var binding = connectionSO.InputActionAsset.actionMaps[i].bindings[b];
                        string path = actionMap.name + "/" + binding.action + "/" + (binding.isComposite ? binding.name + " ("+ binding.path + ")" : binding.path);
                        GUI.enabled = !binding.isComposite;
                        if (GUILayout.Button((binding.isPartOfComposite ? "  |- " :  "") + path, contentStyle))
                        {
                            connectionSO.BindingId = binding.id.ToString();
                            markAsChangedIfEditing();
                        }
                        GUI.enabled = true;
                    }
                }

                GUILayout.EndScrollView();

                GUILayout.Space(7);
                if (GUILayout.Button("Clear Override"))
                {
                    var connection = (InputBindingConnection)connectionSO.GetConnection();
                    connection.ClearOverride();
                }
            }
        }

        protected void markAsChangedIfEditing()
        {
            if (EditorApplication.isPlaying)
                return;

            EditorUtility.SetDirty(connectionSO);
            Undo.RegisterCompleteObjectUndo(connectionSO, "Changed Connection SO.");
        }
#endif
    }
}
#endif
