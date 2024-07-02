using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// The settings system automatically initializes itself whenever it is used first.<br />
    /// This happens for example if you show a UI with a SettingsResolver on it.<br />
    /// However, in most games you do not show the settings UI immediately but you do want the
    /// settings to be applied immediately (like audio volume). In that case add THIS class
    /// to a game object in your very first scene.
    /// </summary>
    [DefaultExecutionOrder(-10)] // Not strictly necessary. We use it to make the Initializer
                                 // run before any of the other settings code.
    public class SettingsInitializer : MonoBehaviour
    {
        private static SettingsInitializer _instance;

        /// <summary>
        /// You should not not call this in or before Awake(). Wait until Start().<br />
        /// The reason for this is that some parts of Unity (Post Processing Volumes) actually
        /// initialize themselves in Awake() and thus the settings system has to wait for 
        /// them to be ready to receive the settings. Once Start() has been reached everything 
        /// should be ready and you are good to go.
        /// </summary>e 
        public static SettingsInitializer Instance => _instance;

        /// <summary>
        /// Quick access to settings (if ther are any). May return null.<br />
        /// You should not not call this in or before Awake(). Wait until Start().
        /// </summary>
        public static Settings Settings => Instance?.Provider?.Settings;

        /// <summary>
        /// Can be used to check if settings are available and loaded.
        /// </summary>
        public static bool HasSettings() => _instance != null && _instance.Provider != null && _instance.Provider.HasSettings();

        /// <summary>
        /// Don't forget to hook this up with the right provider.
        /// </summary>
        [Tooltip("Don't forget to hook this up with the right provider.")]
        public SettingsProvider Provider;

        /// <summary>
        /// Enable if you are unloading the scene that contains the initializer.<br />
        /// If you can then disable this and use additive scene loading instead.
        /// </summary>
        [Tooltip("Enable if you are unloading the scene that contains the initializer.\n" +
                 "If you can then disable this and use additive scene loading instead.")]
        public bool DoNotDestroy = true;

        /// <summary>
        /// Used only if DoNotDestry is enabled.
        /// If enabled then it will re-apply the settings in Start() after reloading this scene.
        /// </summary>
        [Tooltip("Used only if DoNotDestry is enabled.\n" +
                 "If enabled then it will re-apply the settings in Start() after reloading this scene.")]
        public bool ApplyOnReload = true;

        public void Awake()
        {
            // Make sure we only ever have one of these if DoNotDestroy is turned on.
            if (DoNotDestroy)
            {
                if (_instance != null)
                {
                    GameObject.Destroy(gameObject);

                    if (ApplyOnReload)
                        _instance.StartCoroutine(onInstanceReloaded());

                    return;
                }
                else
                {
                    GameObject.DontDestroyOnLoad(gameObject);
                }
            }
            _instance = this;


            // You may think about starting the settings in Awake(). DON'T

            // Some systems, like the post-processing volumes, are initialized in Awake().
            // If you init the settings in Awake() too then no one knows which one gets executed
            // first(the order is not defined).
            // 
            // If the settings system gets initialized first, then it will not find the Post-Pro
            // volumes(they are not there yet).This will lead to „There was no active '...' PostPro
            // effect“ warnings *.
            // 
            // However, if you put your settings init code in Start().Then the volumes will already
            // be initialized and everyone is happy.
            //
            // If for some reason you must intialized the settings in Awake() then please use 
            // the [DefaultExecutionOrder(int.MaxValue)] attribute to make it execute LAST.


            // If you want some custom SAVE and LOAD methods then you will have to register them
            // BEFORE the settings are initialized. For this Awake() is the right spot.

            // Settings.CustomLoadMethod = customLoad;
            // Settings.CustomSaveMethod = customSave;
            // Settings.CustomDeleteMethod = customDelete;
        }

        public void Start()
        {
            if (Provider == null)
            {
                Debug.LogError("You have not set the Provider on you SettingsInitializer. Please set a provider!", this);
                throw new System.Exception("Missing Provider on Settings Initializer.");
            }

            // We have to call the settings system at least once to initialize the load.
            var _ = Provider.Settings;
        }

        static WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

        private IEnumerator onInstanceReloaded()
        {
            yield return _waitForEndOfFrame;

            Provider.Apply(changedOnly: false);
        }


        #region Editor Stuff
#if UNITY_EDITOR
        public void Reset()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            editorAutoSelectProvider();
        }

        bool _editorOnValidated;

        public void OnValidate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(this.gameObject);
            if (prefabStatus != PrefabInstanceStatus.Connected)
                return;

            if (!_editorOnValidated)
            {
                _editorOnValidated = true;
                editorAutoSelectProvider();
            }
        }

        protected void editorAutoSelectProvider()
        {
            if (Provider == null)
            {
                var provider = SettingsProvider.LastUsedSettingsProvider;

                if (provider == null)
                {
                    var providerGuids = AssetDatabase.FindAssets("t:SettingsProvider");
                    if (providerGuids.Length > 0)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(providerGuids[0]);
                        if (path != null)
                        {
                            provider = AssetDatabase.LoadAssetAtPath<SettingsProvider>(path);
                        }
                    }
                }

                if (provider != null)
                {
                    Provider = provider;
                    EditorUtility.DisplayDialog($"Using provider \"{provider.name}\"",
                        $"The SettingsInitializer has automatically chosen the provider: \"{provider.name}\".\n\n" +
                        $"If that is not the provider you want then please assign it in the inspector.",
                        "Ok");
                    EditorUtility.SetDirty(this);
                    EditorUtility.SetDirty(this.gameObject);
                }
            }
        }
#endif
        #endregion
    }
}
