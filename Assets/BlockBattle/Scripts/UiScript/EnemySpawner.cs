using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  
    public int enemyCount = 3;      
    public int maxRows = 2;         

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            Debug.LogError("GridLayoutGroup component not found!");
            return;
        }

        int enemiesPerRow = Mathf.CeilToInt((float)enemyCount / maxRows);

        float gridWidth = gridLayout.GetComponent<RectTransform>().rect.width;
        float gridHeight = gridLayout.GetComponent<RectTransform>().rect.height;
        gridLayout.cellSize = new Vector2(gridWidth / enemiesPerRow, gridHeight / maxRows);

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform);
        }
    }
}

