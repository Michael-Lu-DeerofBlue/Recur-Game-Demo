using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
#endif

#if KAMGAM_POST_PRO_BUILTIN
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Kamgam.SettingsGenerator.Examples
{
    [ExecuteInEditMode]
    public class PostProFixer : MonoBehaviour
    {
        public GameObject PostProVolumeParent;

        public string id = "";

#if UNITY_EDITOR
#if KAMGAM_POST_PRO_BUILTIN
        protected bool _done;


        void Update()
        {
            if (!EditorApplication.isPlaying && !_done)
            {
                _done = true;

                string settingName = "SG_PostProFixer_" + SettingsGeneratorSettings.Version + "_done" + id;
                bool done = SessionState.GetBool(settingName, false);
                SessionState.SetBool(settingName, true);
                if (!done)
                {
                    Fix();
                }
            }
        }

        public void Fix()
        {
            var layer = GetComponent<PostProcessLayer>();
            if(layer == null)
            {
                layer = gameObject.AddComponent<PostProcessLayer>();
            }

            var volume = PostProVolumeParent.GetComponent<PostProcessVolume>();
            if(volume == null)
            {
                volume = PostProVolumeParent.AddComponent<PostProcessVolume>();
            }
            layer.volumeLayer = LayerMask.GetMask("Water");
            volume.gameObject.layer = LayerMask.NameToLayer("Water");

            if (volume != null && (volume.profile == null || !volume.profile.name.Contains("Settings")))
            {
                var profileGUIDs = AssetDatabase.FindAssets("SettingsFromAssetDemo PostProProfile");
                if (profileGUIDs.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(profileGUIDs[0]);
                    if (path != null)
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(path);
                        volume.sharedProfile = asset;
                        volume.profile = asset;
                        Debug.Log("Assigned PostPro Profile '" + path + "' to PostPro Volume.");
                    }
                }
            }

            EditorUtility.SetDirty(volume);
            EditorUtility.SetDirty(layer);
            EditorUtility.SetDirty(gameObject);
            EditorUtility.SetDirty(PostProVolumeParent);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);

            EditorSceneManager.SaveScene(gameObject.scene);
        }

        public void Revert()
        {
            var layer = GetComponent<PostProcessLayer>();
            if (layer != null)
            {
                DestroyImmediate(layer);
            }

            var volume = PostProVolumeParent.GetComponent<PostProcessVolume>();
            if (volume != null)
            {
                DestroyImmediate(volume);
            }

            if (volume != null)
                EditorUtility.SetDirty(volume);
            if (layer != null)
                EditorUtility.SetDirty(layer);
            EditorUtility.SetDirty(gameObject);
            EditorUtility.SetDirty(PostProVolumeParent);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);

            EditorSceneManager.SaveScene(gameObject.scene);
        }
#else
        
        // If no Post Processing is installed then show a message requesting installation.
        void Update()
        {
            if (!EditorApplication.isPlaying)
            {
                // Skip if URP or HDRP (they have their own PostPro Stack).
                if (GraphicsSettings.currentRenderPipeline != null)
                    return;

                bool done = SessionState.GetBool("SG_PostProFixer_info_done", false);
                SessionState.SetBool("SG_PostProFixer_info_done", true);
                if (!done)
                {
                    bool openPackageManager = EditorUtility.DisplayDialog("Install PostProcessing Stack V2",
                        "Some effects (Ambient Occlusion, Gamma, Motion Blur, ...) require Post Processing.\n" +
                        "You should install the PostProcessing Stack from the package manager if you want to use these settings.",
                        "Open Package Manager", "Cancel");

                    if(openPackageManager)
                    {
                        UnityEditor.PackageManager.UI.Window.Open("com.unity.postprocessing");
                    }
                }
            }
        }

#endif

#endif
    }
}

#if UNITY_EDITOR && KAMGAM_POST_PRO_BUILTIN
namespace Kamgam.SettingsGenerator.Examples
{
    [CustomEditor(typeof(PostProFixer))]
    public class PostProFixerEditor : Editor
    {
        public PostProFixer fixer;

        public void OnEnable()
        {
            fixer = target as PostProFixer;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Fix"))
            {
                fixer.Fix();
            }

            if (GUILayout.Button("Revert"))
            {
                fixer.Revert();
            }
        }
    }
}

#endif
