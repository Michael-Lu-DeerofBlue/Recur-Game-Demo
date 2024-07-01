using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLayOutManager : MonoBehaviour
{
    public GameObject[] enemyLayouts; // Array of enemy layout prefabs
    public Enemy[] TestenemyPrefabs; // Array of enemy prefabs
    public TargetSelector targetSelector; // Reference to the target selector script

    public static Dictionary<string, bool> EnemiesList = new Dictionary<string, bool> {
        { "Perseus", false },
        { "Bride (1)", false },
        { "Bride (2)", true },
        { "Hound (2)", false },
        { "Eagle (3)", false },
    };

    private int enemyNum;

    void Start()
    {

        // Set enemyNum to the length of the TestenemyPrefabs array
        enemyNum = TestenemyPrefabs.Length;
        targetSelector = FindObjectOfType<TargetSelector>();
        if (enemyNum > 0 && enemyNum <= enemyLayouts.Length)
        {
            GameObject selectedLayout = enemyLayouts[enemyNum - 1];
            GameObject instantiatedLayout = Instantiate(selectedLayout, Vector3.zero, Quaternion.identity);

            Transform[] childTransforms = instantiatedLayout.GetComponentsInChildren<Transform>();

            // Iterate over each child transform
            for (int i = 0; i < childTransforms.Length - 1 && i < TestenemyPrefabs.Length; i++)
            {
                Transform child = childTransforms[i + 1]; // Skip the root transform
                Vector3 spawnPosition = child.localPosition; // Get the local position of the child

                Enemy instantiatedEnemy = Instantiate(TestenemyPrefabs[i], spawnPosition, Quaternion.identity, transform);
                Debug.Log($"Name: {TestenemyPrefabs[i].name}, Position: {instantiatedEnemy.transform.position}");
            }
            targetSelector.SelectLeftTopTarget();
        }
        else
        {
            Debug.LogError("enemyNum is not within the valid range.");
        }
    }

    void Update()
    {
        // Update logic (if needed)
    }
}

