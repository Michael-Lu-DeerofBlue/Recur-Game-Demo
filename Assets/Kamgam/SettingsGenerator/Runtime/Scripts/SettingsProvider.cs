#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// The Settings Provider creates an instance of the Settings object
    /// and keeps a reference to it so other objects can come and ask
    /// for it at any time.
    /// <br /><br />
    /// It also handles resetting objects before play mode
    /// if Domain-Reload is disabled (via IResetBeforeDomainReload).
    /// </summary>
    [CreateAssetMenu(fileName = "SettingsProvider", menuName = "SettingsGenerator/SettingsProvider", order = 1)]
    public class SettingsProvider : ScriptableObject
#if UNITY_EDITOR
        , IResetBeforeDomainReload
#endif
    {
        /// <summary>
        /// Hold a reference to the last used SettingsProvider.<br />
        /// You should NOT build your code upon this, it may be null (especially before initialization).<br />
        /// However it can be very handy if you know that you are only using one single provider and you need to fetch it quickly.
        /// </summary>
        public static SettingsProvider LastUsedSettingsProvider;

        [SerializeField, Tooltip("The player prefs key under which your settings will be saved.")]
        protected string playerPrefsKey;

        [Tooltip("The default settings asset.\nYou can leave this empty if you define all your settings via script.")]
        [FormerlySerializedAs("Default")]
        public Settings SettingsAsset;

        protected Settings _settings;
        public Settings Settings
        {
            get
            {
                LastUsedSettingsProvider = this;

#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                    return null;
#endif

                if (_settings == null)
                {
                    // Create a new
                    if (SettingsAsset == null)
                    {
                        // Create a new instance from code.
                        _settings = ScriptableObject.CreateInstance<Settings>();
                    }
                    else
                    {
                        // Create from asset.
                        _settings = ScriptableObject.Instantiate(SettingsAsset);
                    }

                    // Make a global backup copy of the current quality level (we use this in the Connections to restore it later).
                    QualityPresets.AddCurrentLevel();

                    // Load user settings from storage
                    Settings.Load(playerPrefsKey);

                    // Register to setting changed for auto-loading
                    Settings.OnSettingChanged += onSettingChanged;
                }

                return _settings;
            }
        }

        /// <summary>
        /// If turned on then for each change in a setting a save will be SCHEDULED. If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.
        /// </summary>
        [Tooltip("If turned on then for each change in a setting a save will be SCHEDULED. If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.")]
        public bool AutoSave = true;

        /// <summary>
        /// Only used if AutoSave is turned on. If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.
        /// </summary>
        [Tooltip("Only used if AutoSave is turned on. If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.")]
        public float AutoSaveWaitTimeInSec = 1.0f;

        /// <summary>
        /// Use this to check whether or not the settings have loaded.
        /// </summary>
        public bool HasSettings()
        {
            return _settings != null;
        }

#pragma warning disable CS0414
        [SerializeField, HideInInspector] private bool _hasBeenInitialisedInEditor = false;
#pragma warning restore  CS0414
        [System.NonSerialized] private double _awakeTime;

#if UNITY_EDITOR
        private void Awake()
        {
            if (!_hasBeenInitialisedInEditor)
            {
                _hasBeenInitialisedInEditor = true;
                EditorUtility.SetDirty(this);

                _awakeTime = EditorApplication.timeSinceStartup;
                UnityEditor.EditorApplication.update += waitForValidPath;
            }
        }

        private void waitForValidPath()
        {
            // Delay for 0.5 Sek to give the deserialization time to load the settings asset.
            // TODO: Add a logic fix, not a timing based fix. Actually with "_hasBeenInitialisedInEditor"
            //       this should no longer be needed. Investigate and remove.
            if (EditorApplication.timeSinceStartup - _awakeTime > 0.5f)
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(this);

                // Wait until the path is ready
                if (!string.IsNullOrEmpty(path))
                {
                    UnityEditor.EditorApplication.update -= waitForValidPath;

                    EditorCreateNewSettingsInstance();
                }
            }
        }

        static string getSanitizedProductName()
        {
            return System.Text.RegularExpressions.Regex.Replace(Application.productName, @"[^-a-zA-Z0-9._ ]", "");
        }

        public void EditorCreateNewSettingsInstance()
        {
            // Rename provider to product name.
            var providerPath = AssetDatabase.GetAssetPath(this);
            if (providerPath.EndsWith("SettingsProvider.asset"))
            {
                string sanitizedProductName = getSanitizedProductName();
                string newName = name + " (" + sanitizedProductName + ")";
                AssetDatabase.RenameAsset(providerPath, newName);
                name = newName;
            }

            // Upgrading (special calse: do NOT propose settings creation for code demo provider).
            if (playerPrefsKey == "SGSettings_Code")
            {
                return;
            }

            if (SettingsAsset != null)
            {
                return;
            }

            bool createSettings = EditorUtility.DisplayDialog(
                "Create Settings list for '" + this.name + "'?",
                "It seems this is a new SettingsProvider.\n\n" +
                "Would you like to create a settings list for it?"
                , "Yes (recommended)", "No");
            if (createSettings)
            {
                Logger.LogMessage("SettingsProvider found: " + providerPath);

                // Auto create settings file for provider.
                var settingsPath = System.IO.Path.GetDirectoryName(providerPath).Replace("\\", "/") + "/" + name.Replace("Provider", "") + ".asset";
                if (settingsPath == providerPath)
                    settingsPath = settingsPath.Replace(".asset", " Settings.asset");

                // Create based on template
                bool createdFromTemplate = false;
                string templatePath = "";
                Settings settings = null;
                var guids = AssetDatabase.FindAssets("t:Settings \"Settings (Template)\"");
                if (guids.Length > 0)
                {
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var settingsTemplate = AssetDatabase.LoadAssetAtPath<Settings>(path);
                        if (settingsTemplate != null)
                        {
                            settings = ScriptableObject.Instantiate(settingsTemplate);

                            // Set all settings disabled.
                            settings.RebuildSettingsCache();
                            foreach (var setting in settings.GetAllSettings())
                            {
                                setting.IsActive = false;
                            }

                            createdFromTemplate = true;
                            templatePath = path;
                            break;
                        }
                    }
                }

                // If template was not used then create an empty settings list.
                if (settings == null)
                {
                    settings = Settings.CreateInstance<Settings>();
                    settings.GetOrCreateFloat("dummy", 1f);
                }

                AssetDatabase.CreateAsset(settings, settingsPath);
                SettingsAsset = settings;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);

                EditorGUIUtility.PingObject(this);
                EditorGUIUtility.PingObject(SettingsAsset);
                Selection.objects = new Object[] { SettingsAsset };

                if (createdFromTemplate)
                    Logger.LogMessage("Settings created based on '" + templatePath + "' under: " + settingsPath);
                else
                    Logger.LogMessage("Settings created under: " + settingsPath);
            }
        }
#endif

        private string getDefaultStorageKey()
        {
            string newPlayerPrefsKey = "Settings." + System.Text.RegularExpressions.Regex.Replace(Application.productName, @"[^-a-zA-Z0-9_]", "");
            return newPlayerPrefsKey;
        }

        public void OnEnable()
        {
            if (string.IsNullOrEmpty(playerPrefsKey))
            {
                playerPrefsKey = getDefaultStorageKey();
            }
        }

#if UNITY_EDITOR
        // Domain Reload handling
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void onResetBeforePlayMode()
        {
            DomainReloadUtils.CallOnResetOnAssets(typeof(SettingsProvider));
        }

        public void ResetBeforePlayMode()
        {
            _settings = null;
        }
#endif

        public void Reset()
        {
            if (Settings != null)
                Settings.Reset();
        }

        public void Reset(params string[] ids)
        {
            Settings.Reset(ids);
        }

        public void ResetGroups(params string[] groups)
        {
            Settings.ResetGroups(groups);
        }

        public void ResetGroup(string group)
        {
            Settings.ResetGroups(group);
        }

        /// <summary>
        /// Applies the settings.<br />
        /// This means that if a setting has a connection it will be pushed and then pulled.
        /// </summary>
        /// <param name="changedOnly">Apply only those which have changed.</param>
        public void Apply(bool changedOnly = true)
        {
            Settings?.Apply(changedOnly);
        }


        // Load & Save

        public void Load()
        {
            if (_settings == null)
            {
                // At the very first load this will be executed.

                // Accessing the "Settings" getter for the very first time
                // causes a load automatically, thus we do not need to load
                // anything here.
                Settings.RefreshRegisteredResolvers();
            }
            else
            {
                // Pull values from connections to initialize the default values.
                Settings.PullFromConnections();

                // Load user settings from storage
                // Also triggers resolver updates (aka Settings.RefreshRegisteredResolvers())
                Settings.Load(playerPrefsKey);
            }
        }

        public void ResetToLastSave()
        {
            // Load user settings from storage
            // Also triggers resolver updates (aka Settings.RefreshRegisteredResolvers())
            Settings.Load(playerPrefsKey);
        }

        public void Save()
        {
            Settings?.Save(playerPrefsKey);
        }

        public void Delete()
        {
            if (Settings != null)
            {
                Settings.Delete(playerPrefsKey);
            }
            else
            {
                // This only exists to support deleting in Editor if not in play mode.
                // Notice the static use of Settings.
                Settings.DeletePlayerPrefs(playerPrefsKey);
            }
        }

        // Auto Save

        protected void onSettingChanged(ISetting setting)
        {
            if (AutoSave)
            {
                ScheduleAutoSave(AutoSaveWaitTimeInSec);
            }
        }

        [System.NonSerialized]
        protected float _autoSaveTime = -1f;

        /// <summary>
        /// If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.
        /// </summary>
        public void ScheduleAutoSave(float autoSaveWaitTimeInSec)
        {
            if (_autoSaveTime < 0f)
            {
                _autoSaveTime = Time.realtimeSinceStartup + autoSaveWaitTimeInSec;
                scheduleAutoSaveAsync();
            }
            else
            {
                _autoSaveTime = Time.realtimeSinceStartup + autoSaveWaitTimeInSec;
            }
        }

        protected async void scheduleAutoSaveAsync()
        {
            // Wait for the timer to run out.
            float deltaTime = _autoSaveTime - Time.realtimeSinceStartup;
            while (deltaTime > 0)
            {
                await System.Threading.Tasks.Task.Delay(Mathf.RoundToInt(deltaTime * 1000) + 50);
                deltaTime = _autoSaveTime - Time.realtimeSinceStartup;
            }

            Save();
            _autoSaveTime = -1f;
        }

        // Static Helpers in Editor
#if UNITY_EDITOR
        public static System.Collections.Generic.List<SettingsProvider> EditorFindAllProviders(bool excludeExampleProviders)
        {
            var providers = new System.Collections.Generic.List<SettingsProvider>();

            var guids = AssetDatabase.FindAssets("t:" + typeof(SettingsProvider).Name);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (excludeExampleProviders && path.Contains("Kamgam"))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<SettingsProvider>(path);
                if (asset != null)
                    providers.Add(asset);
            }

            return providers;
        }

        public static SettingsProvider EditorFindTemplate()
        {
            var providers = EditorFindAllProviders(excludeExampleProviders: false);
            foreach (var provider in providers)
            {
                if (provider.name.Contains("Template"))
                    return provider;
            }

            return null;
        }

        /// <summary>
        /// Creates a new settings provider asset based on the template.
        /// </summary>
        /// <param name="dirName">Foldername that will be created under Assets/[dirName].</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SettingsProvider EditorCreateProviderBasedOnTemplate(string dirName, string name = null, bool pingAfterCreation = false)
        {
            var template = EditorFindTemplate();
            if (template != null)
            {
                // Create dir
                AssetDatabase.CreateFolder("Assets", dirName);
                
                // Create name
                string sanitizedProductName = getSanitizedProductName();
                string newName = name;
                if (string.IsNullOrEmpty(newName))
                {
                    newName = "SettingsProvider (" + sanitizedProductName + ")";
                }

                // Create provider
                var provider = SettingsProvider.Instantiate(template);
                provider.name = newName;
                provider.playerPrefsKey = provider.getDefaultStorageKey();

                // Create Settings object
                var settingsTemplate = template.SettingsAsset;
                var settings = Settings.Instantiate(settingsTemplate);
                provider.SettingsAsset = settings;
                settings.name = provider.name.Replace("Provider", "");
                // Set all settings disabled.
                settings.RebuildSettingsCache();
                foreach (var setting in settings.GetAllSettings())
                {
                    setting.IsActive = false;
                }

                // Persist assets
                AssetDatabase.CreateAsset(provider, "Assets/" + dirName + "/" + provider.name + ".asset");
                AssetDatabase.CreateAsset(settings, "Assets/" + dirName + "/" + settings.name + ".asset");

                if (pingAfterCreation)
                {
                    EditorGUIUtility.PingObject(provider);
                }

                return provider;
            }

            return null;
        }

        public static bool DoesCustomProviderExist()
        {
            var providers = Kamgam.SettingsGenerator.SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true);
            return providers.Count > 0;
        }

        public static SettingsProvider GetFirstCustomProvider()
        {
            var providers = Kamgam.SettingsGenerator.SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true);
            foreach (var provider in providers)
            {
                string path = AssetDatabase.GetAssetPath(provider);
                if (!path.Contains("Kamgam/SettingsGenerator/"))
                    return provider;
            }

            return null;
        }
#endif
    }
}
