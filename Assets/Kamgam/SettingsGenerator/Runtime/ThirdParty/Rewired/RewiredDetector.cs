#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class RewiredDetector
    {
        // We do not do it automatically since that would execute every time.
        // See: https://forum.unity.com/threads/asmdef-questions.651517/
        // [InitializeOnLoadMethod]
        public static void AutoDetectAndUpdateDefine()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                DetectAndUpdateDefine();

                CompilationPipeline.compilationFinished -= onCompilationFinished;
                CompilationPipeline.compilationFinished += onCompilationFinished;
            }
        }

        private static void onCompilationFinished(object obj)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                DetectAndUpdateDefine();
            }
        }

        [MenuItem("Tools/Settings Generator/Third Party/Rewired Setup", priority = 221)]
        public static void DetectAndUpdateDefine()
        {
            AssemblyDetector.DetectAndUpdateDefine(typeFullName: "Rewired.ActionElementMap", define: "KAMGAM_REWIRED");
        }
    }
}
#endif