//#define FOG_BORDER
//#define FOG_SHADOW_CANCELLATION

using UnityEngine;
using UnityEditor;

namespace VolumetricFogAndMist2 {

    [CustomEditor(typeof(VolumetricFogProfile))]
    public class VolumetricFogProfileEditor : Editor {

        SerializedProperty raymarchQuality, raymarchMinStep, jittering, dithering;
        SerializedProperty renderQueue, sortingLayerID, sortingOrder;
        SerializedProperty constantDensity, noiseTexture, noiseStrength, noiseScale, noiseFinalMultiplier, noiseTextureOptimizedSize;
        SerializedProperty useDetailNoise, detailTexture, detailScale, detailStrength, detailOffset;
        SerializedProperty density;
        SerializedProperty shape, scaleNoiseWithHeight, border, customHeight, height, verticalOffset, distance, distanceFallOff, maxDistance, maxDistanceFallOff;
        SerializedProperty terrainFit, terrainFitResolution, terrainLayerMask, terrainFogHeight, terrainFogMinAltitude, terrainFogMaxAltitude;

        SerializedProperty albedo, enableDepthGradient, depthGradient, depthGradientMaxDistance, enableHeightGradient, heightGradient;
        SerializedProperty brightness, deepObscurance, specularColor, specularThreshold, specularIntensity, ambientLightMultiplier;

        SerializedProperty turbulence, windDirection, useCustomDetailNoiseWindDirection, detailNoiseWindDirection;

        SerializedProperty dayNightCycle, sunDirection, sunColor, sunIntensity, lightDiffusionModel, lightDiffusionPower, lightDiffusionIntensity;
        SerializedProperty receiveShadows, shadowIntensity, shadowCancellation, shadowMaxDistance;
        SerializedProperty cookie;

        SerializedProperty distantFog, distantFogColor, distantFogStartDistance, distantFogDistanceDensity, distantFogMaxHeight, distantFogBaseAltitude, distantFogHeightDensity, distantFogDiffusionIntensity, distantFogRenderQueue;

        private void OnEnable() {
            try {
                raymarchQuality = serializedObject.FindProperty("raymarchQuality");
                raymarchMinStep = serializedObject.FindProperty("raymarchMinStep");
                jittering = serializedObject.FindProperty("jittering");
                dithering = serializedObject.FindProperty("dithering");

                renderQueue = serializedObject.FindProperty("renderQueue");
                sortingLayerID = serializedObject.FindProperty("sortingLayerID");
                sortingOrder = serializedObject.FindProperty("sortingOrder");

                constantDensity = serializedObject.FindProperty("constantDensity");

                noiseTexture = serializedObject.FindProperty("noiseTexture");
                noiseStrength = serializedObject.FindProperty("noiseStrength");
                noiseScale = serializedObject.FindProperty("noiseScale");
                noiseFinalMultiplier = serializedObject.FindProperty("noiseFinalMultiplier");
                noiseTextureOptimizedSize = serializedObject.FindProperty("noiseTextureOptimizedSize");

                useDetailNoise = serializedObject.FindProperty("useDetailNoise");
                detailTexture = serializedObject.FindProperty("detailTexture");
                detailScale = serializedObject.FindProperty("detailScale");
                detailStrength = serializedObject.FindProperty("detailStrength");
                detailOffset = serializedObject.FindProperty("detailOffset");

                density = serializedObject.FindProperty("density");
                shape = serializedObject.FindProperty("shape");
                scaleNoiseWithHeight = serializedObject.FindProperty("scaleNoiseWithHeight");
                border = serializedObject.FindProperty("border");

                customHeight = serializedObject.FindProperty("customHeight");
                height = serializedObject.FindProperty("height");
                verticalOffset = serializedObject.FindProperty("verticalOffset");

                distance = serializedObject.FindProperty("distance");
                distanceFallOff = serializedObject.FindProperty("distanceFallOff");
                maxDistance = serializedObject.FindProperty("maxDistance");
                maxDistanceFallOff = serializedObject.FindProperty("maxDistanceFallOff");

                terrainFit = serializedObject.FindProperty("terrainFit");
                terrainFitResolution = serializedObject.FindProperty("terrainFitResolution");
                terrainLayerMask = serializedObject.FindProperty("terrainLayerMask");
                terrainFogHeight = serializedObject.FindProperty("terrainFogHeight");
                terrainFogMinAltitude = serializedObject.FindProperty("terrainFogMinAltitude");
                terrainFogMaxAltitude = serializedObject.FindProperty("terrainFogMaxAltitude");

                albedo = serializedObject.FindProperty("albedo");
                enableDepthGradient = serializedObject.FindProperty("enableDepthGradient");
                depthGradient = serializedObject.FindProperty("depthGradient");
                depthGradientMaxDistance = serializedObject.FindProperty("depthGradientMaxDistance");
                enableHeightGradient = serializedObject.FindProperty("enableHeightGradient");
                heightGradient = serializedObject.FindProperty("heightGradient");

                brightness = serializedObject.FindProperty("brightness");
                deepObscurance = serializedObject.FindProperty("deepObscurance");
                specularColor = serializedObject.FindProperty("specularColor");
                specularThreshold = serializedObject.FindProperty("specularThreshold");
                specularIntensity = serializedObject.FindProperty("specularIntensity");
                ambientLightMultiplier = serializedObject.FindProperty("ambientLightMultiplier");

                turbulence = serializedObject.FindProperty("turbulence");
                windDirection = serializedObject.FindProperty("windDirection");
                useCustomDetailNoiseWindDirection = serializedObject.FindProperty("useCustomDetailNoiseWindDirection");
                detailNoiseWindDirection = serializedObject.FindProperty("detailNoiseWindDirection");

                dayNightCycle = serializedObject.FindProperty("dayNightCycle");
                sunDirection = serializedObject.FindProperty("sunDirection");
                sunColor = serializedObject.FindProperty("sunColor");
                sunIntensity = serializedObject.FindProperty("sunIntensity");

                lightDiffusionModel = serializedObject.FindProperty("lightDiffusionModel");
                lightDiffusionPower = serializedObject.FindProperty("lightDiffusionPower");
                lightDiffusionIntensity = serializedObject.FindProperty("lightDiffusionIntensity");

                receiveShadows = serializedObject.FindProperty("receiveShadows");
                shadowIntensity = serializedObject.FindProperty("shadowIntensity");
                shadowCancellation = serializedObject.FindProperty("shadowCancellation");
                shadowMaxDistance = serializedObject.FindProperty("shadowMaxDistance");

                cookie = serializedObject.FindProperty("cookie");

                distantFog = serializedObject.FindProperty("distantFog");
                distantFogColor = serializedObject.FindProperty("distantFogColor");
                distantFogStartDistance = serializedObject.FindProperty("distantFogStartDistance");
                distantFogDistanceDensity = serializedObject.FindProperty("distantFogDistanceDensity");
                distantFogMaxHeight = serializedObject.FindProperty("distantFogMaxHeight");
                distantFogBaseAltitude = serializedObject.FindProperty("distantFogBaseAltitude");
                distantFogHeightDensity = serializedObject.FindProperty("distantFogHeightDensity");
                distantFogDiffusionIntensity = serializedObject.FindProperty("distantFogDiffusionIntensity");
                distantFogRenderQueue = serializedObject.FindProperty("distantFogRenderQueue");
            } catch { }
        }


        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(raymarchQuality);
            EditorGUILayout.PropertyField(raymarchMinStep);
            EditorGUILayout.PropertyField(jittering);
            EditorGUILayout.PropertyField(dithering);
            EditorGUILayout.PropertyField(renderQueue);
            EditorGUILayout.PropertyField(sortingLayerID);
            EditorGUILayout.PropertyField(sortingOrder);

            EditorGUILayout.PropertyField(constantDensity);
            if (!constantDensity.boolValue) {
                EditorGUILayout.PropertyField(noiseTexture);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(noiseStrength, new GUIContent("Strength"));
                EditorGUILayout.PropertyField(noiseScale, new GUIContent("Scale"));
                EditorGUILayout.PropertyField(scaleNoiseWithHeight);
                EditorGUILayout.PropertyField(noiseFinalMultiplier, new GUIContent("Multiplier"));
                EditorGUILayout.PropertyField(noiseTextureOptimizedSize, new GUIContent("Final Texture Size"));
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(useDetailNoise, new GUIContent("Detail Noise"));
                if (useDetailNoise.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(detailTexture);
                    EditorGUILayout.PropertyField(detailStrength, new GUIContent("Strength"));
                    EditorGUILayout.PropertyField(detailScale, new GUIContent("Scale"));
                    EditorGUILayout.PropertyField(detailOffset, new GUIContent("Offset"));
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.PropertyField(density);
            EditorGUILayout.PropertyField(shape);
#if FOG_BORDER
            EditorGUILayout.PropertyField(border);
#else
            GUI.enabled = false;
            EditorGUILayout.LabelField("Border", "(Disabled in Volumetric Fog Manager)");
            GUI.enabled = true;
#endif
            EditorGUILayout.PropertyField(customHeight, new GUIContent("Custom Volume Height"));
            if (customHeight.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(height);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(verticalOffset);
            EditorGUILayout.PropertyField(distance);
            if (distance.floatValue > 0) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(distanceFallOff);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(maxDistance);
            EditorGUILayout.PropertyField(maxDistanceFallOff);

            EditorGUILayout.PropertyField(terrainFit);
            if (terrainFit.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(terrainFitResolution, new GUIContent("Resolution"));
                EditorGUILayout.PropertyField(terrainLayerMask, new GUIContent("Layer Mask"));
                if (Terrain.activeTerrain != null) {
                    int terrainLayer = Terrain.activeTerrain.gameObject.layer;
                    if ((terrainLayerMask.intValue & (1 << terrainLayer)) == 0) {
                        EditorGUILayout.HelpBox("Current terrain layer is not included in this layer mask. Terrain fit may not work properly.", MessageType.Warning);
                    }
                }
                EditorGUILayout.PropertyField(terrainFogHeight, new GUIContent("Fog Height"));
                EditorGUILayout.PropertyField(terrainFogMinAltitude, new GUIContent("Min Altitude"));
                EditorGUILayout.PropertyField(terrainFogMaxAltitude, new GUIContent("Max Altitude"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(albedo);
            Color albedoColor = albedo.colorValue;
            albedoColor.a = EditorGUILayout.Slider(new GUIContent("Alpha"), albedoColor.a, 0, 1f);
            albedo.colorValue = albedoColor;
            EditorGUILayout.PropertyField(enableDepthGradient);
            if (enableDepthGradient.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(depthGradient);
                EditorGUILayout.PropertyField(depthGradientMaxDistance, new GUIContent("Max Distance"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(enableHeightGradient);
            if (enableHeightGradient.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(heightGradient);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(brightness);
            EditorGUILayout.PropertyField(deepObscurance);
            EditorGUILayout.PropertyField(specularColor);
            EditorGUILayout.PropertyField(specularThreshold);
            EditorGUILayout.PropertyField(specularIntensity);

            EditorGUILayout.PropertyField(turbulence);
            EditorGUILayout.PropertyField(windDirection);
            EditorGUILayout.PropertyField(useCustomDetailNoiseWindDirection, new GUIContent("Custom Detail Noise Wind"));
            if (useCustomDetailNoiseWindDirection.boolValue) {
                EditorGUILayout.PropertyField(detailNoiseWindDirection);
            }

            EditorGUILayout.PropertyField(dayNightCycle);
            if (dayNightCycle.boolValue) {
                VolumetricFogManager manager = VolumetricFogManager.GetManagerIfExists();
                if (manager != null && manager.sun == null) {
                    EditorGUILayout.HelpBox("You must assign a directional light to the Sun property of the Volumetric Fog Manager.", MessageType.Warning);
                    if (GUILayout.Button("Go to Volumetric Fog Manager")) {
                        Selection.activeGameObject = manager.gameObject;
                        EditorGUIUtility.ExitGUI();
                        return;
                    }
                }
            } else { 
                EditorGUILayout.PropertyField(sunDirection);
                EditorGUILayout.PropertyField(sunColor);
                EditorGUILayout.PropertyField(sunIntensity);
            }
            EditorGUILayout.PropertyField(ambientLightMultiplier, new GUIContent("Ambient Light", "Amount of ambient light that influences fog colors"));
            EditorGUILayout.PropertyField(lightDiffusionModel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(lightDiffusionPower, new GUIContent("Spread"));
            EditorGUILayout.PropertyField(lightDiffusionIntensity, new GUIContent("Intensity"));
            EditorGUI.indentLevel--;
#if UNITY_2021_3_OR_NEWER
                EditorGUILayout.PropertyField(cookie);
#endif

            EditorGUILayout.PropertyField(receiveShadows);
            if (receiveShadows.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(shadowIntensity);
#if FOG_SHADOW_CANCELLATION
                EditorGUILayout.PropertyField(shadowCancellation);
#endif
                EditorGUILayout.PropertyField(shadowMaxDistance);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(distantFog, new GUIContent("Enable Distant Fog"));
            if (distantFog.boolValue) {
                EditorGUILayout.PropertyField(distantFogColor, new GUIContent("Color"));
                EditorGUILayout.PropertyField(distantFogStartDistance, new GUIContent("Start Distance"));
                EditorGUILayout.PropertyField(distantFogDistanceDensity, new GUIContent("Distance Density"));
                EditorGUILayout.PropertyField(distantFogBaseAltitude, new GUIContent("Base Altitude"));
                EditorGUILayout.PropertyField(distantFogMaxHeight, new GUIContent("Max Height"));
                EditorGUILayout.PropertyField(distantFogHeightDensity, new GUIContent("Height Density"));
                EditorGUILayout.PropertyField(distantFogDiffusionIntensity, new GUIContent("Diffusion Intensity Multiplier"));
                EditorGUILayout.PropertyField(distantFogRenderQueue, new GUIContent("Render Queue"));
                if (VolumetricFogRenderFeature.isRenderingBeforeTransparents && distantFogRenderQueue.intValue > 2500) {
                    EditorGUILayout.HelpBox("Please make sure the render queue is 2500 or less if Volumetric Fog Renderer Feature runs 'Before Transparent Objects'.", MessageType.Warning);
                }
            }

            serializedObject.ApplyModifiedProperties();

        }
    }

}
