using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLayOutManager : MonoBehaviour
{
    public GameObject[] enemyLayouts; // Array of enemy layout prefabs
    public List<Enemy> enemyPrefabs = new List<Enemy>(); // Array of enemy prefabs
    public TargetSelector targetSelector; // Reference to the target selector script

    public static Dictionary<string, bool> EnemiesList = new Dictionary<string, bool>();
    public Enemy Perseus;
    public Enemy HeadLessBack;
    public Enemy HeadLessFront;
    public Enemy Lion;
    public Enemy MockingBird;
    public Enemy[] Mondrinion;
    private int enemyNum;

    void Start()
    {
        EnemiesList = ThreeDTo2DData.dataDictionary;
        EnemiesList = ThreeDTo2DData.dataDictionary;
        /*
        EnemiesList = new Dictionary<string, bool>(){
        { "Mondrinion 1", false }, 
        { "Mondrinion 2", false },
        { "Mondrinion 3", false },
        { "Mondrinion 4", false },
        { "Mondrinion 5", false },
        { "Mondrinion 6", false },
        { "Mondrinion 7", false },};
        */
        GenerateEnemyList();
        SpawnEnemies();
    }
    private void GenerateEnemyList()
    {
        foreach (var kvp in EnemiesList)
        {
            string enemyName = kvp.Key.Split(' ')[0];
            Enemy enemy = Mondrinion[0];
            switch (enemyName)
            {
                case "Perseus":
                    enemy = Perseus;
                    break;
                case "Bride":
                    if (EnemiesList[kvp.Key]) { enemy = HeadLessBack; break; }
                    else { enemy = HeadLessFront; break; }
                case "Lion":
                    enemy = Lion;
                    break;
                case "Bird":
                    enemy = MockingBird;
                    break;
                case "Mondrinion":
                    int index = int.Parse(kvp.Key.Split(' ')[1]);
                    enemy = Mondrinion[index];
                    break;
                default:
                    break;
            }
            enemyPrefabs.Add(enemy);
        }
    }
    private void SpawnEnemies()
    {
        // Set enemyNum to the length of the TestenemyPrefabs array
        enemyNum = enemyPrefabs.Count;
        targetSelector = FindObjectOfType<TargetSelector>();
        if (enemyNum > 0 && enemyNum <= enemyLayouts.Length)
        {
            GameObject selectedLayout = enemyLayouts[enemyNum - 1];
            GameObject instantiatedLayout = Instantiate(selectedLayout, Vector3.zero, Quaternion.identity);

            Transform[] childTransforms = instantiatedLayout.GetComponentsInChildren<Transform>();

            // Iterate over each child transform
            for (int i = 0; i < childTransforms.Length - 1 && i < enemyPrefabs.Count; i++)
            {
                Transform child = childTransforms[i + 1]; // Skip the root transform
                Vector3 spawnPosition = child.localPosition; // Get the local position of the child

                Enemy instantiatedEnemy = Instantiate(enemyPrefabs[i], spawnPosition, Quaternion.identity, transform);
                Debug.Log($"Name: {enemyPrefabs[i].name}, Position: {instantiatedEnemy.transform.position}");
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

