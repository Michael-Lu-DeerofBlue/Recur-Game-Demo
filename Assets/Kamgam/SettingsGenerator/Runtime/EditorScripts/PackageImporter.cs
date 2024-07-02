#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kamgam.SettingsGenerator
{
    // Look into "PackageImporter.Data.cs" first.

    /// <summary>
    /// This class contains the package importing logic. It keeps track of what
    /// packages have been imported already and uses CrossCompileCallbacks to 
    /// call an Action once it is done.
    /// </summary>
    public static partial class PackageImporter
    {

        protected interface IPackage
        {
            bool IsNeeded();
            string GetPackagePath();
        }

        protected abstract class Package : IPackage
        {
            public string PackagePath;

            public Package(string packagePath)
            {
                PackagePath = packagePath;
            }

            public abstract bool IsNeeded();

            public string GetPackagePath()
            {
                return PackagePath;
            }

            public override string ToString()
            {
                return PackagePath;
            }
        }

        static System.Action _onComplete;

        #region Start Import Delayed
        static double startPackageImportAt;

        /// <summary>
        /// Use this after your asset has been freshly imported.
        /// I have yet to find a way to reliably (logically) predict
        /// when all the assets are really ready (AssetDatabase failed me on this).
        /// </summary>
        /// <param name="onComplete"></param>
        public static void ImportDelayed(Action onComplete)
        {
            // Some assets may not be loaded at this time. Thus we wait for them to be imported.
            _onComplete = onComplete;

            EditorApplication.update -= onEditorUpdate;
            EditorApplication.update += onEditorUpdate;

            // Why wait 3 seconds?
            // Let's say I found this value with a customer driven Monte Carlo method (aka "experience")
            startPackageImportAt = EditorApplication.timeSinceStartup + 3; // wait N seconds
        }

        static void onEditorUpdate()
        {
            // wait for the time to reach startPackageImportAt
            if (startPackageImportAt - EditorApplication.timeSinceStartup < 0)
            {
                EditorApplication.update -= onEditorUpdate;
                Import(_onComplete);
                return;
            }
        }
        #endregion

        static int _crossCompileCallbackID = -1;
        static string SessionPackagesToImportKey = typeof(PackageImporter).FullName + ".PackagesToImport";
        static string CrossCompileOmCompleteKey = typeof(PackageImporter).FullName + ".importedCallack";


        [MenuItem(AssetInfos.MenuEntryToTriggerImportManually, priority = 200)]
        public static void Import()
        {
            Import(onComplete: null);
        }

        public static void Import(Action onComplete)
        {
            if (onComplete != null)
                _onComplete = onComplete;

            // Don't import during play mode.
            if (EditorApplication.isPlaying)
                return;

            Debug.Log("PackageImporter: Checking ..");

            var packagesToImport = createListOfPackagesToImport();
            importNextPackage(packagesToImport);
        }

        static List<IPackage> createListOfPackagesToImport()
        {
            var packagesToImport = Packages.Where(p => p.IsNeeded()).ToList();
            if (packagesToImport.Count > 0)
            {
                Debug.Log("PackageImporter: Scheduled these packages for import:");
                foreach (var package in packagesToImport)
                {
                    Debug.Log(" * " + package.ToString());
                }
            }
            else
            {
                Debug.Log("PackageImporter: Everything looks okay. No imports needed.");
            }

            savePackagesToImportList(packagesToImport);

            if (packagesToImport.Count == 0)
            {
                onPackageImportDone(_onComplete);
            }

            return packagesToImport;
        }

        static void importNextPackage(List<IPackage> packagesToImport)
        {
            if (packagesToImport.Count > 0)
            {
                var package = packagesToImport[0];
                removePackageFromImportList(package);
                importPackage(package);
            }
        }

        static void importPackage(IPackage package)
        {
            // AssetDatabase.importPackageCompleted callbacks are lost after a recompile.
            // Therefore, if the package includes any scripts then these will not be called.
            // See: https://forum.unity.com/threads/assetdatabase-importpackage-callbacks-dont-work.544031/#post-3716791

            // We use CrossCompileCallbacks to register a callback for after compilation.
            _crossCompileCallbackID = CrossCompileCallbacks.RegisterCallback(onPackageImportedAfterRecompile);
            // We also have to store the external callback (if there is one)
            CrossCompileCallbacks.StoreAction(CrossCompileOmCompleteKey, _onComplete);
            // Delay to avoid "Calling ... from assembly reloading callbacks are not supported." errors.
            CrossCompileCallbacks.DelayExecutionAfterCompilation = true;

            // If the package does not contain any scripts then we can still use the normal callbacks.
            // AssetDatabase.importPackageCompleted will only fire if the package did NOT contain any code.
            AssetDatabase.importPackageCompleted -= onPackageImported;
            AssetDatabase.importPackageCompleted += onPackageImported;

            // import package
            Debug.Log("PackageImporter: Importing '" + package.GetPackagePath() + "'.");
            AssetDatabase.ImportPackage(package.GetPackagePath(), interactive: false);
            AssetDatabase.SaveAssets();
        }

        static void savePackagesToImportList(IList<IPackage> packages)
        {
            int[] packageIndices = new int[packages.Count];
            for (int i = 0; i < packages.Count; i++)
            {
                // We use the index of the package in the Packages list as ID.
                packageIndices[i] = Packages.IndexOf(packages[i]);
            }
            SessionState.SetIntArray(SessionPackagesToImportKey, packageIndices);
        }

        static List<IPackage> loadPackagesToImportList()
        {
            var packageIndices = SessionState.GetIntArray(SessionPackagesToImportKey, new int[] { });
            var packages = packageIndices.Select(i => Packages[i]).ToList();
            return packages;
        }

        static void removePackageFromImportList(IPackage package)
        {
            var packages = loadPackagesToImportList();
            packages.Remove(package);
            savePackagesToImportList(packages);
        }

        // This is only execute if the package did not contain any script files.
        static void onPackageImported(string packageName)
        {
            Debug.Log("PackageImporter: Package '" + packageName + "' imported.");

            // There was no recompile. Thus we clear the registered callback.
            CrossCompileCallbacks.ReleaseIndex(_crossCompileCallbackID);

            // Unregister from package completion.
            AssetDatabase.importPackageCompleted -= onPackageImported;

            onPackageImportDone(_onComplete);
            _onComplete = null;
        }

        static void onPackageImportedAfterRecompile()
        {
            Debug.Log("PackageImporter: Recompile detected. Assuming package import is done.");

            // The registered callback is already cleared by now.
            // Now we let's retrieve that stored extenal callback and hand it over.
            var onComplete = CrossCompileCallbacks.GetStoredAction(CrossCompileOmCompleteKey);
            onPackageImportDone(onComplete);
        }

        static void onPackageImportDone(Action onComplete)
        {
            var packagesToImport = loadPackagesToImportList();

            if (packagesToImport.Count > 0)
            {
                // Check for more packages to import
                Debug.Log("PackageImporter: Looking for next package.");

                // Make sure the onComplete callback is retained across multiple package loads.
                _onComplete = onComplete;

                // Start importing
                importNextPackage(packagesToImport);
            }
            else
            {
                AssetDatabase.SaveAssets();
                Debug.Log("PackageImporter: Done (no more packages to import).");

                onComplete?.Invoke();
            }
        }
    }
}
#endif