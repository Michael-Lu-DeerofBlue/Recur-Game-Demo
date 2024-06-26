using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLayOutManager : MonoBehaviour
{

    public GameObject[] enemyLayouts;
    public int enemyNum = 7;
    public Enemy TestenemyPrefab;
    public static Dictionary<string, bool> EnemiesList = new Dictionary<string, bool>();
    void Start()
    {
        if (enemyNum > 0 && enemyNum <= enemyLayouts.Length)
        {
            GameObject selectedLayout = enemyLayouts[enemyNum - 1];

            GameObject instantiatedLayout = Instantiate(selectedLayout, Vector3.zero, Quaternion.identity);

            Transform[] childTransforms = instantiatedLayout.GetComponentsInChildren<Transform>();

            foreach (Transform child in childTransforms)
            {
                if (child != instantiatedLayout.transform) 
                {
                    Vector3 spawnPosition = child.localPosition; 
                    Instantiate(TestenemyPrefab, spawnPosition, Quaternion.identity, transform);
                    Debug.Log($"Name: {child.name}, Position: {child.position}");
                }
            }
        }
        else
        {
            Debug.LogError("enemyNum >8, not work");
        }
    }

    void Update()
    {

    }
}
