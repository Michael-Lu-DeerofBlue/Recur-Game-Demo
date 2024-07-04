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
    public static Enemy[] ToThreeEnemies;
    // Start is called before the first frame update
    public void TwoDGameOver()
    {
        Debug.Log("Here");
        enemies = FindObjectsOfType<Enemy>();
        ToThreeEnemies = enemies;
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        if (enemies.Length == 0)
        {
            Victory= true;
            Debug.Log("Victory");
        }
        else
        {
            Victory= false;
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
            ToThreeEnemies =  new Enemy[0];
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
        if (ThreeDTo2DData.ThreeDScene == "CentralMeditationRoom")
        {
            ThreeDTo2DData.ThreeDScene = null;
            SceneManager.LoadScene("Church_with_code");
        }
        else
        {
            SceneManager.LoadScene(ThreeDTo2DData.ThreeDScene);
        }
    }
}
