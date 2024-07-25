using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Rendering;
using System.Reflection;

namespace VolumetricFogAndMist2 {

    [CustomEditor(typeof(VolumetricFog))]
    public partial class VolumetricFogEditor : Editor {

        VolumetricFogProfile cachedProfile;
        Editor cachedProfileEditor;
        SerializedProperty profile;
        SerializedProperty maskEditorEnabled, maskBrushMode, maskBrushColor, maskBrushWidth, maskBrushFuzziness, maskBrushOpacity;
        SerializedProperty enablePointLights, enableNativeLights, nativeLightsMultiplier, enableAPV, apvIntensityMultiplier;
        SerializedProperty enableVoids;
        SerializedProperty enableFogOfWar, fogOfWarCenter, fogOfWarIsLocal, fogOfWarSize, fogOfWarShowCoverage, fogOfWarTextureSize, fogOfWarRestoreDelay, fogOfWarRestoreDuration, fogOfWarSmoothness, fogOfWarBlur;
        SerializedProperty enableFollow, followTarget, followMode, followOffset, followIncludeDistantFog;
        SerializedProperty enableFade, fadeDistance, fadeOut, fadeController, enableSubVolumes, subVolumes;
        SerializedProperty enableUpdateModeOptions, updateMode, updateModeCamera, updateModeBounds;
        SerializedProperty showBoundary;

        static GUIStyle boxStyle;
        VolumetricFog fog;
        public static VolumetricFog lastEditingFog;

        void OnEnable() {
            profile = serializedObject.FindProperty("profile");

            enablePointLights = serializedObject.FindProperty("enablePointLights");
            enableNativeLights = serializedObject.FindProperty("enableNativeLights");
            nativeLightsMultiplier = serializedObject.FindProperty("nativeLightsMultiplier");
            enableAPV = serializedObject.FindProperty("enableAPV");
            apvIntensityMultiplier = serializedObject.FindProperty("apvIntensityMultiplier");
            enableVoids = serializedObject.FindProperty("enableVoids");
            enableFogOfWar = serializedObject.FindProperty("enableFogOfWar");
            fogOfWarCenter = serializedObject.FindProperty("fogOfWarCenter");
            fogOfWarIsLocal = serializedObject.FindProperty("fogOfWarIsLocal");
            fogOfWarSize = serializedObject.FindProperty("fogOfWarSize");
            fogOfWarShowCoverage = serializedObject.FindProperty("fogOfWarShowCoverage");
            fogOfWarTextureSize = serializedObject.FindProperty("fogOfWarTextureSize");
            fogOfWarRestoreDelay = serializedObject.FindProperty("fogOfWarRestoreDelay");
            fogOfWarRestoreDuration = serializedObject.FindProperty("fogOfWarRestoreDuration");
            fogOfWarSmoothness = serializedObject.FindProperty("fogOfWarSmoothness");
            fogOfWarBlur = serializedObject.FindProperty("fogOfWarBlur");

            maskEditorEnabled = serializedObject.FindProperty("maskEditorEnabled");
            maskBrushColor = serializedObject.FindProperty("maskBrushColor");
            maskBrushMode = serializedObject.FindProperty("maskBrushMode");
            maskBrushWidth = serializedObject.FindProperty("maskBrushWidth");
            maskBrushFuzziness = serializedObject.FindProperty("maskBrushFuzziness");
            maskBrushOpacity = serializedObject.FindProperty("maskBrushOpacity");

            enableFollow = serializedObject.FindProperty("enableFollow");
            followTarget = serializedObject.FindProperty("followTarget");
            followMode = serializedObject.FindProperty("followMode");
            followOffset = serializedObject.FindProperty("followOffset");
            followIncludeDistantFog = serializedObject.FindProperty("followIncludeDistantFog");

            enableFade = serializedObject.FindProperty("enableFade");
            fadeDistance = serializedObject.FindProperty("fadeDistance");
            fadeOut = serializedObject.FindProperty("fadeOut");
            fadeController = serializedObject.FindProperty("fadeController");
            enableSubVolumes = serializedObject.FindProperty("enableSubVolumes");
            subVolumes = serializedObject.FindProperty("subVolumes");
            enableUpdateModeOptions = serializedObject.FindProperty("enableUpdateModeOptions");
            updateMode = serializedObject.FindProperty("updateMode");
            updateModeCamera = serializedObject.FindProperty("updateModeCamera");
            updateModeBounds = serializedObject.FindProperty("updateModeBounds");
            showBoundary = serializedObject.FindProperty("showBoundary");

            fog = (VolumetricFog)target;
            lastEditingFog = fog;
        }


        public override void OnInspectorGUI() {

            var pipe = GraphicsSettings.currentRenderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
            if (pipe == null) {
                EditorGUILayout.HelpBox("Universal Rendering Pipeline asset is not set in Project Settings / Graphics !", MessageType.Error);
                return;
            }

            if (!pipe.supportsCameraDepthTexture) {
                EditorGUILayout.HelpBox("Depth Texture option is required in Universal Rendering Pipeline asset!", MessageType.Error);
                if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                    Selection.activeObject = pipe;
                }
                EditorGUILayout.Separator();
                GUI.enabled = false;
            }

            // Check depth texture mode
            FieldInfo renderers = pipe.GetType().GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            if (renderers == null) return;
            foreach (var renderer in (object[])renderers.GetValue(pipe)) {
                if (renderer == null) continue;
                FieldInfo depthTextureModeField = renderer.GetType().GetField("m_CopyDepthMode", BindingFlags.NonPublic | BindingFlags.Instance);
                if (depthTextureModeField != null) {
                    int depthTextureMode = (int)depthTextureModeField.GetValue(renderer);
                    if (depthTextureMode == 1) { // transparent copy depth mode
                        EditorGUILayout.HelpBox("Depth Texture Mode in URP asset must be set to 'After Opaques' or 'Force Prepass'.", MessageType.Warning);
                        if (GUILayout.Button("Show Pipeline Asset")) {
                            Selection.activeObject = (Object)renderer;
                            GUIUtility.ExitGUI();
                        }
                        EditorGUILayout.Separator();
                    }
                }
            }

            if (boxStyle == null) {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.padding = new RectOffset(15, 10, 5, 5);
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(profile);

            if (profile.objectReferenceValue != null) {
                if (cachedProfile != profile.objectReferenceValue) {
                    cachedProfile = null;
                }
                if (cachedProfile == null) {
                    cachedProfile = (VolumetricFogProfile)profile.objectReferenceValue;
                    cachedProfileEditor = CreateEditor(profile.objectReferenceValue);
                }

                // Drawing the profile editor
                EditorGUILayout.BeginVertical(boxStyle);
                cachedProfileEditor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            } else {
                EditorGUILayout.HelpBox("Create or assign a fog profile.", MessageType.Info);
                if (GUILayout.Button("New Fog Profile")) {
                    CreateFogProfile();
                }
            }

            EditorGUIUtility.labelWidth = 170;
            EditorGUILayout.PropertyField(enableNativeLights);
            if (enableNativeLights.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(nativeLightsMultiplier, new GUIContent("Intensity Multiplier"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(enableAPV, new GUIContent("Enable APV (Probe Volumes)"));
            if (enableAPV.boolValue) {
                EditorGUI.indentLevel++;
#if !UNITY_2023_1_OR_NEWER
                EditorGUILayout.HelpBox("Adaptative Probe Volumes are only available in Unity 2023.", MessageType.Warning);
#endif
                EditorGUILayout.PropertyField(apvIntensityMultiplier, new GUIContent("Intensity Multiplier"));
                EditorGUI.indentLevel--;
            }

            GUI.enabled = !enableNativeLights.boolValue;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(enablePointLights);
            if (GUILayout.Button("Point Light Manager", GUILayout.Width(180))) {
                Selection.activeGameObject = VolumetricFogManager.pointLightManager.gameObject;
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(enableVoids);
            if (GUILayout.Button("Void Manager", GUILayout.Width(180))) {
                Selection.activeGameObject = VolumetricFogManager.fogVoidManager.gameObject;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(enableFollow);
            if (enableFollow.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(followTarget, new GUIContent("Target"));
                EditorGUILayout.PropertyField(followMode, new GUIContent("Mode"));
                if ((VolumetricFogFollowMode)followMode.intValue == VolumetricFogFollowMode.FullXYZ) {
                    EditorGUILayout.PropertyField(followIncludeDistantFog, new GUIContent("Include Distant Fog", "Also adjusts distant fog base altitude to the followed object altitude."));
                }
                EditorGUILayout.PropertyField(followOffset, new GUIContent("Offset"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(enableFade);
            if (enableFade.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(fadeDistance);
                EditorGUILayout.PropertyField(fadeOut);
                EditorGUILayout.PropertyField(fadeController);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(enableSubVolumes);
            if (enableSubVolumes.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("If no sub-volumes are specified below, any sub-volume in the scene will be used.", MessageType.Info);
                EditorGUILayout.PropertyField(fadeController, new GUIContent("Character Controller"));
                EditorGUILayout.PropertyField(subVolumes);
                EditorGUI.indentLevel--;
            }

            bool requiresFogOfWarTextureReload = false;
            EditorGUILayout.PropertyField(enableFogOfWar);
            if (enableFogOfWar.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(fogOfWarCenter, new GUIContent("World Center"));
                EditorGUILayout.PropertyField(fogOfWarIsLocal, new GUIContent("Is Local", "Enable if fog of war center is local to the fog volume"));
                EditorGUILayout.PropertyField(fogOfWarSize, new GUIContent("World Coverage"));
                EditorGUILayout.PropertyField(fogOfWarShowCoverage, new GUIContent("Show Coverage Bounds"));
                EditorGUILayout.PropertyField(fogOfWarTextureSize, new GUIContent("Texture Size"));
                EditorGUILayout.PropertyField(fogOfWarRestoreDelay, new GUIContent("Restore Delay"));
                EditorGUILayout.PropertyField(fogOfWarRestoreDuration, new GUIContent("Restore Duration"));
                EditorGUILayout.PropertyField(fogOfWarSmoothness, new GUIContent("Border Smoothness"));
                EditorGUILayout.PropertyField(fogOfWarBlur, new GUIContent("Blur"));

                EditorGUILayout.Separator();
                EditorGUILayout.PropertyField(maskEditorEnabled, new GUIContent("Fog Of War Editor", "Activates terrain brush to paint/remove fog of war at custom locations."));

                if (maskEditorEnabled.boolValue) {
                    if (GUILayout.Button("Create New Mask Texture")) {
                        if (EditorUtility.DisplayDialog("Create Mask Texture", "A texture asset will be created with the size specified in current profile (" + fog.fogOfWarTextureSize + "x" + fog.fogOfWarTextureSize + ").\n\nContinue?", "Ok", "Cancel")) {
                            CreateNewMaskTexture();
                            GUIUtility.ExitGUI();
                        }
                    }
                    EditorGUI.BeginChangeCheck();
                    fog.fogOfWarTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Coverage Texture", "Fog of war coverage mask. A value of alpha of zero means no fog."), fog.fogOfWarTexture, typeof(Texture2D), false);
                    if (EditorGUI.EndChangeCheck()) {
                        requiresFogOfWarTextureReload = true;
                    }
                    Texture2D tex = fog.fogOfWarTexture;
                    if (tex != null) {
                        EditorGUILayout.LabelField("   Texture Size", tex.width.ToString());
                        string path = AssetDatabase.GetAssetPath(tex);
                        if (string.IsNullOrEmpty(path)) {
                            path = "(Temporary texture)";
                        }
                        EditorGUILayout.LabelField("   Texture Path", path);
                    }

                    if (tex != null) {
                        EditorGUILayout.Separator();
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(maskBrushMode, new GUIContent("Brush Mode", "Select brush operation mode."));
                        if (GUILayout.Button("Toggle", GUILayout.Width(70))) {
                            maskBrushMode.intValue = maskBrushMode.intValue == 0 ? 1 : 0;
                        }
                        EditorGUILayout.EndHorizontal();
                        if (maskBrushMode.intValue == (int)MaskTextureBrushMode.ColorFog) {
                            EditorGUILayout.PropertyField(maskBrushColor, new GUIContent("   Color", "Brush color."));
                        }
                        EditorGUILayout.PropertyField(maskBrushWidth, new GUIContent("   Width", "Width of the snow editor brush."));
                        EditorGUILayout.PropertyField(maskBrushFuzziness, new GUIContent("   Fuzziness", "Solid vs spray brush."));
                        EditorGUILayout.PropertyField(maskBrushOpacity, new GUIContent("   Opacity", "Stroke opacity."));
                        EditorGUILayout.BeginHorizontal();
                        if (tex == null) GUI.enabled = false;
                        if (GUILayout.Button("Fill Mask")) {
                            fog.ResetFogOfWar(1f);
                            maskBrushMode.intValue = (int)MaskTextureBrushMode.RemoveFog;
                        }
                        if (GUILayout.Button("Clear Mask")) {
                            fog.ResetFogOfWar(0);
                            maskBrushMode.intValue = (int)MaskTextureBrushMode.AddFog;
                        }

                        GUI.enabled = true;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(enableUpdateModeOptions);
            if (enableUpdateModeOptions.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(updateMode);
                EditorGUILayout.PropertyField(updateModeCamera, new GUIContent("Camera"));
                if (updateMode.intValue == (int)VolumetricFogUpdateMode.WhenCameraIsInsideArea) {
                    EditorGUILayout.PropertyField(updateModeBounds, new GUIContent("Bounds"));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(showBoundary);

            EditorGUILayout.Separator();
            if (GUILayout.Button("Show Volumetric Fog Manager Settings")) {
                Selection.activeObject = VolumetricFogManager.instance;
            }

            serializedObject.ApplyModifiedProperties();
            if (requiresFogOfWarTextureReload) {
                fog.ReloadFogOfWarTexture();
            }

        }


        void CreateFogProfile() {

            // Find directional light and adjusts brightness to avoid excessive bright fog
            float brightness = 1f;
#if UNITY_2023_2_OR_NEWER
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
#else
            Light[] lights = FindObjectsOfType<Light>();
#endif
            if (lights != null) {
                foreach (Light light in lights) {
                    if (light.type == LightType.Directional) {
                        brightness /= light.intensity;
                        break;
                    }
                }
            }

            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path)) {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }
            VolumetricFogProfile fp = CreateInstance<VolumetricFogProfile>();
            fp.name = "New Volumetric Fog Profile";
            fp.brightness = brightness;
            AssetDatabase.CreateAsset(fp, path + "/" + fp.name + ".asset");
            AssetDatabase.SaveAssets();
            profile.objectReferenceValue = fp;
            EditorGUIUtility.PingObject(fp);
        }
    }

}