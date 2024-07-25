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
    public Enemy Floral;
    public Enemy Artemis;
    public Enemy Deadalus;
    public Enemy[] Mondrinion;
    private int enemyNum;
    public  

    void Start()
    {
        //除了改底下的那个GenerateEnemyList，其他不要再改了！！！因为我在做整体的系统的串联，所以不要再改了
        EnemiesList = ThreeDTo2DData.dataDictionary ;
        GenerateEnemyList();
        SpawnEnemies();
    }

    
    private void GenerateEnemyList()
    {
        foreach (var kvp in EnemiesList)
        {
            Debug.Log(kvp.Key + Time.time);
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
                    if (EnemiesList[kvp.Key])
                    {
                        enemy = Lion; //有脆弱
                        enemy.FragilingNum++;
                        Debug.Log("backstabbed lion");
                        break;
                    }
                    else
                    {
                        enemy = Lion; //无脆弱
                        break;
                    }
                case "Deer":
                    if (EnemiesList[kvp.Key]) { 
                        enemy = Deer; //有脆弱
                        enemy.FragilingNum++;
                        Debug.Log("backstabbed deer");
                        break;
                        }
                    else {
                        enemy = Deer; //无脆弱
                        enemy.FragilingNum++;
                        break;
                    }
                case "Bird":
                    enemy.GetComponent<Enemy>().in3DName = kvp.Key;
                    enemy = MockingBird;
                    break;
                case "Floral":
                    enemy.GetComponent<Enemy>().in3DName = kvp.Key;
                    enemy = Floral;
                    break;
                case "Artemis":
                    enemy.GetComponent<Enemy>().in3DName = kvp.Key;
                    enemy = Artemis;
                    break;
                case "Deadalus":
                    enemy.GetComponent<Enemy>().in3DName = kvp.Key;
                    enemy = Deadalus;
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

    public void  RelayoutEnemies()
    {
        // Find all existing enemies in the scene
        Enemy[] existingEnemies = FindObjectsOfType<Enemy>();
        int enemyCount = existingEnemies.Length;

        if (enemyCount > 0 && enemyCount <= enemyLayouts.Length)
        {
            // Get the layout corresponding to the number of enemies
            GameObject selectedLayout = enemyLayouts[enemyCount - 1];
            GameObject instantiatedLayout = Instantiate(selectedLayout, Vector3.zero, Quaternion.identity);

            Transform[] childTransforms = instantiatedLayout.GetComponentsInChildren<Transform>();

            // Iterate over each child transform
            for (int i = 0; i < childTransforms.Length - 1 && i < existingEnemies.Length; i++)
            {
                Transform child = childTransforms[i + 1]; // Skip the root transform
                Vector3 newPosition = child.localPosition; // Get the local position of the child
                Vector3 newScale = child.localScale;

                // Reposition and rescale the existing enemy
                existingEnemies[i].transform.position = newPosition;
                existingEnemies[i].transform.localScale = newScale;
                existingEnemies[i].ReCreateEnemyUI();
            }

            // Destroy the instantiated layout as it was only needed for positioning
            targetSelector.SelectLeftTopTarget();

        }
        else
        {
            Debug.LogError("The number of existing enemies is not within the valid range.");
        }
    }
    void Update()
    {
        // Update logic (if needed)
    }
}

