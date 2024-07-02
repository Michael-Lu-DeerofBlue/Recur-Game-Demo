#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
#if UNITY_2023_1_OR_NEWER
using UnityEditor.Build;
#endif

namespace Kamgam.SettingsGenerator
{
    public static class AssemblyDefinitionDetector
    {
        public static void DetectAndUpdateDefine(string assemblyName, string define)
        {
            // Check if the asmdef file exists (if not then the asset has been removed).
            // We do NOT use reflections on the assemblies here because even if the asset files
            // have been removed the assemblies are still loaded and this would give a false positive result.
            bool found = false;
            // Nasty string search. n2h: use regexp.
            string defineNameJsonString0 = "\"name\": \"" + assemblyName + "\"";
            string defineNameJsonString1 = "\"name\":\"" + assemblyName + "\"";
            var defines = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset");
            foreach (var defGUID in defines)
            {
                var path = AssetDatabase.GUIDToAssetPath(defGUID);
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                var text = asset.text.Substring(0, 100); // only search in the beginning of the text.
                if (text.Contains(defineNameJsonString0)
                    || text.Contains(defineNameJsonString1))
                {
                    found = true;
                    break;
                }
            }

            // Add or remove defines.
            if (found)
            {
                // InControl is installed
                addDefineSymbol(define);
            }
            else
            {
                removeDefineSymbol(define);
            }
        }

        private static bool addDefineSymbol(string defineName)
        {
            bool didChange = false;

            foreach (BuildTargetGroup targetGroup in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (isObsolete(targetGroup))
                    continue;

#if UNITY_2023_1_OR_NEWER
                string currentDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(targetGroup));
#else
                string currentDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
#endif

                if (currentDefineSymbols.Contains(defineName))
                    continue;

#if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(targetGroup), currentDefineSymbols + ";" + defineName);
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentDefineSymbols + ";" + defineName);
#endif
                Logger.Log($"{defineName} symbol has been added for {targetGroup}.");

                didChange = true;
            }

            return didChange;
        }

        private static void removeDefineSymbol(string defineName)
        {
            foreach (BuildTargetGroup targetGroup in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (isObsolete(targetGroup))
                    continue;

#if UNITY_2023_1_OR_NEWER
                string currentDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(targetGroup));
#else
                string currentDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
#endif

                if (currentDefineSymbols.Contains(defineName))
                {
                    currentDefineSymbols = currentDefineSymbols.Replace(";" + defineName, "");
#if UNITY_2023_1_OR_NEWER
                    PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(targetGroup), currentDefineSymbols);
#else
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentDefineSymbols);
#endif
                    Logger.Log($"{defineName} symbol has been removed for {targetGroup}.");
                }
            }
        }

        private static bool isObsolete(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), inherit: false);
            return (attributes != null && attributes.Length > 0);
        }
    }
}
#endif