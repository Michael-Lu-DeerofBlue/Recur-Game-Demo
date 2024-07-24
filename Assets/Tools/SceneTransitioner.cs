using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance;
    public GameObject levelController;
    public string airIslandSceneName = "Air Island";
    public string battleLevelSceneName = "BattleLevel";

    private bool isBattleLevelLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load the BattleLevel scene additively
    public void SwitchToBattleLevel()
    {
        if (!isBattleLevelLoaded)
        {
            StartCoroutine(LoadSceneAdditively(battleLevelSceneName));
        }
    }

    // Unload the BattleLevel scene
    public void ReturnToAirIsland()
    {
        if (isBattleLevelLoaded)
        {
            StartCoroutine(UnloadScene(battleLevelSceneName));
        }
        FindAnyObjectByType<Level3>().GetComponent<Level3>().ReloadBackToBattle();
    }

    private IEnumerator LoadSceneAdditively(string sceneName)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        isBattleLevelLoaded = true;
        // Optionally, set the active scene to the new scene
        // SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    private IEnumerator UnloadScene(string sceneName)
    {
        var asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        isBattleLevelLoaded = false;
    }
}
