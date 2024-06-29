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
    public static bool ToThreeDVictory;
    // Start is called before the first frame update
    public void TwoDGameOver()
    {
        enemies= FindObjectsOfType<Enemy>();
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        if (enemies.Length == 0)
        {
            Victory= true;
            ToThreeDVictory = true;
            Debug.Log("Victory");
        }
        else
        {
            Victory= false;
            ToThreeDVictory = false;
            Debug.Log("Defeat");
        }
        inventory = itemManager.inventory;
        foreach (var item in inventory)
        {
            Debug.Log($"Inventory item: {item.Key}, quantity: {item.Value}");
        }
        // when need to save data from2d to 3D, just give Victory and iventory parameters.
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
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
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(ThreeDTo2DData.ThreeDScene);
    }
}
