#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
#if UNITY_2023_1_OR_NEWER
using UnityEditor.Build;
#endif

namespace Kamgam.SettingsGenerator
{
    public static class AssemblyDetector
    {
        public static void DetectAndUpdateDefine(string typeFullName, string define)
        {
			bool found = false;
			
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeFullName);
                if (type != null)
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