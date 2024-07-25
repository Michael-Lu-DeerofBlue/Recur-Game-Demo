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
    public Flowchart BGMFlowchart;
    public static List<string> ToThreeEnemies;
    private SoundManager soundManager;
    public static bool win;
    void Start()
    {
        soundManager = FindAnyObjectByType<SoundManager>();
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
                BGMFlowchart.ExecuteBlock("FadeToCalm"); //fade music to calm ver.
                ThreeDTo2DData.ThreeDScene = null;
                SceneManager.LoadScene("Church_with_code");
            }
          
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
