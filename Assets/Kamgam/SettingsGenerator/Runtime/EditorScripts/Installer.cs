#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class Installer
    {
        [UnityEditor.Callbacks.DidReloadScripts(998000)]
        public static void InstallIfNeeded()
        {
            bool versionChanged = VersionHelper.UpgradeVersion(AssetInfos.GetVersion, out Version oldVersion, out Version newVersion);
            if (versionChanged)
            {
                Debug.Log("SettingsGenerator Version changed from " + oldVersion + " to " + newVersion);

                var minVersion = VersionHelper.DefaultVersion;

                // Make sure we only upgrade if it's not the first installation. 
                if (oldVersion > minVersion)
                {
                    // Update IsActive flag in old settings
                    var versionWithIsActiveFlag = new Version("1.12.1");
                    if (oldVersion < versionWithIsActiveFlag && newVersion >= versionWithIsActiveFlag)
                    {
                        SetAllSettingsActive();
                    }
                }

                // Import packages
                int index = CrossCompileCallbacks.RegisterCallback(onImportPackages);

                // If both URP and HDRP packages are installed then this will detect
                // which one is used and then forces only one DEFINE to be used.
                bool didChangeAssemblies = AssemblyDefinitionUpdater.CheckAndUpdate();
                if (!didChangeAssemblies)
                {
                    CrossCompileCallbacks.ReleaseIndex(index); 
                    onImportPackages();
                }
            }
        }

        [MenuItem("Tools/Settings Generator/Debug/Set All Settings Active")]
        public static void SetAllSettingsActive()
        {
            var settingsGuids = AssetDatabase.FindAssets("t:" + typeof(Settings).FullName);
            foreach (var guid in settingsGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                
                Logger.LogMessage($"Upgrading Settings IsActive flag in '{path}'");

                var settings = AssetDatabase.LoadAssetAtPath<Settings>(path);
                foreach (var setting in settings.GetAllSettings())
                {
                    // Since older settings did not have the IsActive flag we set them all to active by default.
                    setting.IsActive = true;
                }
                // Save
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssetIfDirty(settings);
            }
        }

        private static void onImportPackages()
        {
            // Import packages and then show welcome screen.
            PackageImporter.ImportDelayed(showWelcomeMessage);
        }

        static void showWelcomeMessage()
        {
            bool openExample = EditorUtility.DisplayDialog(
                    "Settings Generator",
                    "Thank you for choosing Settings Generator.\n\n" +
                    "Please start by reading the manual.\n\n" +
                    "If you can find the time I would appreciate your feedback in the form of a review.\n\n" +
                    "I have prepared some examples for you.",
                    "Open Example", "Open manual (web)"
                    );

            if (openExample)
                SettingsGeneratorSettings.OpenExample();
            else
                SettingsGeneratorSettings.OpenManual();
        }

    }
}
#endif