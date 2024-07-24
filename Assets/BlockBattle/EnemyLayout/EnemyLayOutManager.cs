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
    public Enemy Deer;
    public Enemy MockingBird;
    public Enemy[] Mondrinion;
    private int enemyNum;

    void Start()
    {
        //除了改底下的那个GenerateEnemyList，其他不要再改了！！！因为我在做整体的系统的串联，所以不要再改了
        EnemiesList = ThreeDTo2DData.dataDictionary;
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
                    enemy.GetComponent<Enemy>().in3DName = kvp.Key;
                    enemy = Perseus;
                    break;
                case "Bride":
                    if (EnemiesList[kvp.Key]) { enemy = HeadLessBack; enemy.GetComponent<Enemy>().in3DName = kvp.Key; break; }
                    else { enemy = HeadLessFront; enemy.GetComponent<Enemy>().in3DName = kvp.Key; break; }
                case "Lion":
                    enemy.GetComponent<Enemy>().in3DName = kvp.Key;
                    enemy = Lion;
                    break;
                case "Deer":
                    if (EnemiesList[kvp.Key]) { 
                        enemy = Deer; //无脆弱
                        break;
                        }
                    else {
                        enemy = Deer; //有脆弱
                        break;
                    }
                case "Bird":
                    enemy.GetComponent<Enemy>().in3DName = kvp.Key;
                    enemy = MockingBird;
                    break;
                case "Mondrinion":
                    enemy.GetComponent<Enemy>().in3DName = kvp.Key;
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
                Vector3 spawnScale = child.localScale;

                Enemy instantiatedEnemy = Instantiate(enemyPrefabs[i], spawnPosition, Quaternion.identity, transform);
                instantiatedEnemy.transform.localScale = spawnScale;
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

