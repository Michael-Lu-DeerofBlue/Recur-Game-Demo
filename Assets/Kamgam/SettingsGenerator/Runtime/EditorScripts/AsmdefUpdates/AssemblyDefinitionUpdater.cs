#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

namespace Kamgam.SettingsGenerator
{
    public static class AssemblyDefinitionUpdater
    {
        [MenuItem("Tools/Settings Generator/Debug/Check and update assembly definitions")]
        public static bool CheckAndUpdate()
        {
            // If both HDRP and UPR defines are set the we have an ambiguous situation.
#if KAMGAM_RENDER_PIPELINE_HDRP && KAMGAM_RENDER_PIPELINE_URP
            return UpdateAssemblyDefinitionForRenderPipeline();
#else
            return false;
#endif
        }

        public static bool UpdateAssemblyDefinitionForRenderPipeline()
        {
            var rp = RenderPipelineDetector.GetCurrentRenderPiplelineType();
            Debug.Log($"Detected that both URP and HDRP are installed but only '{rp}' is used. Will modify assembly definition to resolve ambiguity.");

            // Stop if BultIn is used (should never happen due to check in CheckAndUpdate).
            if (rp == RenderPipelineDetector.RenderPiplelineType.BuiltIn)
            {
                return ReplaceIn("SettingsGenerator.asmdef", triggerReimport: true,
                    "\"KAMGAM_RENDER_PIPELINE_URP\"", "\"_disabled_KAMGAM_RENDER_PIPELINE_URP\"",
                    "\"KAMGAM_RENDER_PIPELINE_HDRP\"", "\"_disabled_KAMGAM_RENDER_PIPELINE_HDRP\"",
                    "\"KAMGAM_RENDER_PIPELINE_CORE\"", "\"_disabled_KAMGAM_RENDER_PIPELINE_CORE\"");
            }
            else if (rp == RenderPipelineDetector.RenderPiplelineType.URP)
            {
                return ReplaceIn("SettingsGenerator.asmdef", triggerReimport: true,
                    "\"_disabled_KAMGAM_RENDER_PIPELINE_URP\"", "\"KAMGAM_RENDER_PIPELINE_URP\"",
                    "\"KAMGAM_RENDER_PIPELINE_HDRP\"", "\"_disabled_KAMGAM_RENDER_PIPELINE_HDRP\"",
                    "\"_disabled_KAMGAM_RENDER_PIPELINE_CORE\"", "\"KAMGAM_RENDER_PIPELINE_CORE\"");
            }
            else // if (rp == RenderPipelineDetector.RenderPiplelineType.HDRP)
            {
                return ReplaceIn("SettingsGenerator.asmdef", triggerReimport: true,
                    "\"KAMGAM_RENDER_PIPELINE_URP\"", "\"_disabled_KAMGAM_RENDER_PIPELINE_URP\"",
                    "\"_disabled_KAMGAM_RENDER_PIPELINE_HDRP\"", "\"KAMGAM_RENDER_PIPELINE_HDRP\"",
                    "\"_disabled_KAMGAM_RENDER_PIPELINE_CORE\"", "\"KAMGAM_RENDER_PIPELINE_CORE\"");
            }
        }

        public static bool ReplaceIn(string asmdefFileName, bool triggerReimport,
            string search0, string replace0,
            string search1 = null, string replace1 = null,
            string search2 = null, string replace2 = null)
        {
            if (string.IsNullOrEmpty(search0) || string.IsNullOrEmpty(replace0) || string.IsNullOrEmpty(asmdefFileName))
                return false;

            if (!asmdefFileName.EndsWith(".asmdef"))
            {
                asmdefFileName += ".asmdef";
            }

            string filePath = null;
            var guids = AssetDatabase.FindAssets("t:asmdef");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(asmdefFileName))
                {
                    filePath = path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(filePath))
                return false;

            // Get full path
            string fullPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6); // trim "Assets"
            fullPath += filePath;

            // Replace
            string originalText = File.ReadAllText(fullPath);

            string text = originalText.Replace(search0, replace0);

            if (!string.IsNullOrEmpty(search1) && !string.IsNullOrEmpty(replace1))
                text = text.Replace(search1, replace1);

            if (!string.IsNullOrEmpty(search2) && !string.IsNullOrEmpty(replace2))
                text = text.Replace(search2, replace2);

            // Skip if nothing changed.
            if (string.Compare(originalText, text) == 0)
                return false;

            // write to tmp
            string tmpPath = fullPath + ".tmp~";

            if (File.Exists(tmpPath))
                File.Delete(tmpPath);

            File.WriteAllText(tmpPath, text);

            File.Delete(fullPath);

            // swap tmp with file
            File.Move(tmpPath, fullPath);
            File.Delete(tmpPath);

            // import
            if (triggerReimport)
                AssetDatabase.Refresh();

            return true;
        }
    }
}
#endif
