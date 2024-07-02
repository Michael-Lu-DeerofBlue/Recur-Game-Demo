#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Kamgam.SettingsGenerator
{
    public static class AssetInfos
    {
        // I used this as a fallback in case something goes wrong with the auto import.
        // The import by menu ignores any version check and always forces a new import.
        public const string MenuEntryToTriggerImportManually = "Tools/Settings Generator/Debug/Import Packages";

        /// <summary>
        /// The root path of your asset folder starting with "Assets/", ending with a slash "/".<br />
        /// The version marker file will be stored there.<br />
        /// Example: "Assets/Kamgam/SettingsGenerator/"<br />
        /// If left empty then "Assets/" will be assumed.
        /// </summary>
        public static string AssetRootPath = "Assets/Kamgam/SettingsGenerator/";

        /// <summary>
        /// Change this to return the version of the software.<br />
        /// This is a separate method because your version may be stored in another class or a file.<br />
        /// The return value if this is compared against the install version marker.
        /// </summary>
        /// <returns></returns>
        public static Version GetVersion()
        {
            // This will differ in your Asset. I store my version as a string "1.0.0" in my settings.
            return VersionHelper.Parse(SettingsGeneratorSettings.Version);
        }

        // Uncomment this if you want it run at every recompile.
        // I usually call it after intializing my settings for the first time.
        /*
        [UnityEditor.Callbacks.DidReloadScripts]
        static void AutoInstall()
        {
            PackageImporter.ImportDelayed(null);
        }
        //*/
    }

    /// <summary>
    /// This class contains the package importing logic. It keeps track of what
    /// packages have been imported already and uses CrossCompileCallbacks to 
    /// call an Action once it is done.
    /// </summary>
    public static partial class PackageImporter
    {
        // ---- THIS IS WHERE YOU DECLARE WHAT PACKAGES TO IMPORT AND WHEN -------------------------------
        protected class RenderPipelinePackage : Package
        {
            public enum RenderPipelineType
            {
                URP = 0,
                HDRP = 1,
                BuiltIn = 2
            }

            public RenderPipelineType RenderPipeline;

            public RenderPipelinePackage(RenderPipelineType renderPipeline, string packagePath) : base(packagePath)
            {
                RenderPipeline = renderPipeline;
            }

            public override bool IsNeeded()
            {
                return GetCurrentRenderPipeline() == RenderPipeline;
            }

            public static RenderPipelineType GetCurrentRenderPipeline()
            {
                var currentRP = GraphicsSettings.currentRenderPipeline;

                // null if built-in
                if (currentRP != null)
                {
                    if (currentRP.GetType().Name.Contains("Universal"))
                    {
                        return RenderPipelineType.URP;
                    }
                    else
                    {
                        return RenderPipelineType.HDRP;
                    }
                }

                return RenderPipelineType.BuiltIn;
            }
        }

        protected class UIToolkitPackage : Package
        {
            public bool UsesModule = true;

            public UIToolkitPackage(bool usesModule, string packagePath) : base(packagePath)
            {
                UsesModule = usesModule;
            }

            public override bool IsNeeded()
            {
#if UNITY_2021_2_OR_NEWER
                // Unity has the UI Toolkit included as an internal module.
                return UsesModule;
#elif KAMGAM_UI_ELEMENTS
                // Unity uses the old UI toolkit package.
                return !UsesModule;
#else
                // No UI Toolkit is used.
                return false;
#endif
            }
        }

        static List<IPackage> Packages = new List<IPackage>()
        {
            new RenderPipelinePackage( RenderPipelinePackage.RenderPipelineType.URP,  AssetInfos.AssetRootPath + "Packages/SettingsGeneratorURP.unitypackage" ),
            new RenderPipelinePackage( RenderPipelinePackage.RenderPipelineType.HDRP, AssetInfos.AssetRootPath + "Packages/SettingsGeneratorHDRP.unitypackage" ),
            new UIToolkitPackage( usesModule: false, AssetInfos.AssetRootPath + "Packages/SettingsGeneratorUIToolkitPackage.unitypackage" ),
            new UIToolkitPackage( usesModule: true, AssetInfos.AssetRootPath + "Packages/SettingsGeneratorUIToolkitModule.unitypackage" )
        };
        // -----------------------------------------------------------------------------------------------
    }
}
#endif