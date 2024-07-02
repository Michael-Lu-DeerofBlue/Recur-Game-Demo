using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Kamgam.SettingsGenerator.Examples
{
    public class SceneLoader : MonoBehaviour
    {
#if UNITY_EDITOR
        public UnityEditor.SceneAsset SceneToLoad;
#endif
        public string SceneName;
        public bool LoadAdditively = true;

        protected SettingInt audioMusicVolumeSetting;

        void Start()
        {
#if UNITY_EDITOR
            SceneName = SceneToLoad.name;
            EditorSceneManager.LoadSceneInPlayMode(
                UnityEditor.AssetDatabase.GetAssetPath(SceneToLoad),
                new LoadSceneParameters(LoadAdditively ? LoadSceneMode.Additive : LoadSceneMode.Single)
                );
#else
            SceneManager.LoadScene(
                SceneName,
                LoadAdditively ? LoadSceneMode.Additive : LoadSceneMode.Single
            );
#endif
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (SceneToLoad != null && SceneToLoad.name != null)
                SceneName = SceneToLoad.name;
        }
#endif
    }
}
