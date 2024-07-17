using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class TwoDto3D : MonoBehaviour
{
    private Enemy[]enemies;
    public bool Victory;
    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    public Flowchart flowchart;
    public static List<string> ToThreeEnemies;
    private SoundManager soundManager;

    void Start()
    {
        soundManager = FindAnyObjectByType<SoundManager>();
    }
    // Start is called before the first frame update
    public void TwoDGameOver()
    {
        Debug.Log("Here");
        enemies = FindObjectsOfType<Enemy>();
        if (enemies.Length != 0)
            {
                for (int i = 0; i < enemies.Length; i++)
                {
                    ToThreeEnemies.Add(enemies[i].in3DName);
                }
        }               
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        if (enemies.Length == 0)
        {
            Victory= true;
            Debug.Log("Victory");
            soundManager.PlaySound("PlayerWin");
        }
        else
        {
            Victory= false;
            Debug.Log("Defeat");
            soundManager.PlaySound("CombatExit");
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
            BackToLevel();
        }
    }

    public void BackToLevel()
    {
        flowchart.ExecuteBlock("WhiteScreen");
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
                ThreeDTo2DData.ThreeDScene = null;
                SceneManager.LoadScene("Church_with_code");
            }
          
        }
        else
        {
            SceneManager.LoadScene(ThreeDTo2DData.ThreeDScene);
        }
    }
}
