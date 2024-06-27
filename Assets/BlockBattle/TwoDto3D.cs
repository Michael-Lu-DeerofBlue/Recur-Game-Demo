using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDto3D : MonoBehaviour
{
    private Enemy[]enemies;
    public bool Victory;
    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    // Start is called before the first frame update
    public void TwoDGameOver()
    {
        enemies= FindObjectsOfType<Enemy>();
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
}
