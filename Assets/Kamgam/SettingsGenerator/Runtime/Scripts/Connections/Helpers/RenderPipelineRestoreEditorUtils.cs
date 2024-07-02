#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kamgam.SettingsGenerator
{
    public static class RenderPipelineRestoreEditorUtils
    {
        /// <summary>
        /// Uses the getter on the current render pipeline asset
        /// and saves the value in backupValues.<br />
        /// It also registers onPlayModeEdit as a callback for
        /// EditorApplication.playModeStateChanged.
        /// </summary>
        /// <typeparam name="TAsset">Usually either UniversalRenderPipelineAsset or HDRenderPipelineAsset</typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="backupValues"></param>
        /// <param name="getter"></param>
        /// <param name="onPlayModeExit"></param>
        public static void InitAfterDomainReload<TAsset, TValue>(
            ref Dictionary<RenderPipelineAsset, TValue> backupValues,
            System.Func<TAsset, TValue> getter,
            System.Action<PlayModeStateChange> onPlayModeExit = null)
            where TAsset : RenderPipelineAsset
        {
            // Save values after domain reload.
            if (backupValues == null)
            {
                backupValues = new Dictionary<RenderPipelineAsset, TValue>();
            }
            backupValues.Clear();

            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                var rpAsset = QualitySettings.GetRenderPipelineAssetAt(i) as TAsset;
                if (rpAsset != null)
                {
                    var value = getter.Invoke(rpAsset);
                    if (backupValues.ContainsKey(rpAsset))
                    {
                        backupValues[rpAsset] = value;
                    }
                    else
                    {
                        backupValues.Add(rpAsset, value);
                    }
                }
            }

            if (onPlayModeExit != null)
            {
                EditorApplication.playModeStateChanged -= onPlayModeExit;
                EditorApplication.playModeStateChanged += onPlayModeExit;
            }
        }

        /// <summary>
        /// Reads the values from backupValues and uses the setter to apply it
        /// to the current render pipeline asset.
        /// </summary>
        /// <typeparam name="TAsset">Usually either UniversalRenderPipelineAsset or HDRenderPipelineAsset</typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="state"></param>
        /// <param name="backupValues"></param>
        /// <param name="setter"></param>
        public static void OnPlayModeExit<TAsset, TValue>(
            PlayModeStateChange state,
            Dictionary<RenderPipelineAsset, TValue> backupValues,
            System.Action<TAsset, TValue> setter)
            where TAsset : RenderPipelineAsset
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                if (backupValues != null)
                {
                    foreach (var kv in backupValues)
                    {
                        if (kv.Key == null)
                            continue;
                        setter.Invoke(kv.Key as TAsset, kv.Value);
                    }
                }
            }
        }
    }
}
#endif
