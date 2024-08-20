using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;
using PixelCrushers;

public class TwoDto3D : MonoBehaviour
{
    private Enemy[]enemies;
    public bool Victory;
    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    public Flowchart gameFlowchart;
    private SingletonFlowchart bgmFlowchart;
    public static List<string> ToThreeEnemies;
    private SoundManager soundManager;
    public static bool win;
    void Start()
    {
        soundManager = FindAnyObjectByType<SoundManager>();
        bgmFlowchart = SingletonFlowchart.Instance; //get the singleton bgm flowchart in the scene
        if (bgmFlowchart == null)
        {
            Debug.LogWarning("BGM flowchart not found!");
        }
    }
    // Start is called before the first frame update
    public void TwoDGameOver()
    {
        enemies = FindObjectsOfType<Enemy>();
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        if (enemies.Length == 0)
        {
            win = true;
            Victory= true;
            Debug.Log("Victory");
            soundManager.PlaySfx("PlayerWin");
        }
        else
        {
            win= false;
            Victory= false;
            Debug.Log("Defeat");
            soundManager.PlaySfx("CombatExit");
        }
        inventory = itemManager.inventory;
        foreach (var item in inventory)
        {
            Debug.Log($"Inventory item: {item.Key}, quantity: {item.Value}");
        }
        BackToLevel();
        // when need to save data from2d to 3D, just give Victory and iventory parameters.
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToThreeEnemies = new List<string>();
            win = true;
            BackToLevel();
        }
    }

    public void BackToLevel()
    {
        gameFlowchart.ExecuteBlock("WhiteScreen");
        StartCoroutine(LoadBattle());
    }

    public IEnumerator LoadBattle()
    {
        yield return new WaitForSeconds(0.7f);
        if (ThreeDTo2DData.ThreeDScene == "CentralMeditationRoom")
        {
            if (ES3.KeyExists("First Combat"))
            {
                if (ES3.Load<bool>("First Combat"))
                {
                    ThreeDTo2DData.ThreeDScene = null;
                    SceneManager.LoadScene("CentralMeditationRoom");
                }
                
            }
            else
            {
                //bgmFlowchart.ExecuteBlock("FadeToCalm"); //fade music to calm ver.
                Debug.Log("load battle stoppping music");
                bgmFlowchart.ExecuteBlock("StopMusic");
                ThreeDTo2DData.ThreeDScene = null;
                SceneManager.LoadScene("Church_with_code");
            }
          
        }
        else if (GalleryLevel.deadalus)
        {
            SceneManager.LoadScene("Yellow_Red_Blue");
        }
        else if (ThreeDTo2DData.ThreeDScene == "Air Island Jungle")
        {
            SceneTransitioner.Instance.ReturnToAirIsland();
        }
        else
        {
            SceneManager.LoadScene(ThreeDTo2DData.ThreeDScene);
        }
    }

    public void TheLabyrinth()
    {

    }
}
