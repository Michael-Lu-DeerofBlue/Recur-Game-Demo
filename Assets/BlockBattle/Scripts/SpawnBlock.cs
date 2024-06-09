using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    public GameObject[] BlockShapes;
    public GameObject[] GGhostBlockShapes;
    public Color[] Colors;
    private GameObject NewBlock;
    private GameObject ghostBlock;
    private int lastSpawnedIndex;
    public int blockIdCounter;
    private BlockManager blockManager;
    // Start is called before the first frame update
    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        SpawnNewBlock();
    }

    public void SpawnNewBlock()
    {
        if (!checkGameEnd())
        {
            blockIdCounter++;
            // Generate a random index
            lastSpawnedIndex = Random.Range(0, BlockShapes.Length);

            // Instantiate a random Tetromino

            NewBlock = Instantiate(BlockShapes[lastSpawnedIndex], transform.position, Quaternion.identity);
            ApplyRandomColor(NewBlock);
            SpawnGhostBlock();
        }
    }

    public bool checkGameEnd()
    {
        for (int i = 0; i < 13; i++)
        {
            if (BlockManager.grid[i, 23] != null)
            {
                return true;
            }
        }
        return false;
    }

    public void SpawnGhostBlock()
    {
        if (ghostBlock != null)
        {
            Destroy(ghostBlock);
        }
        ghostBlock = Instantiate(GGhostBlockShapes[lastSpawnedIndex], NewBlock.transform.position, NewBlock.transform.localRotation);

        ApplyGhostColor(ghostBlock);
    }

    public void ApplyRandomColor(GameObject block)
    {
        // Choose a random color from the Colors array
        Color randomColor = Colors[Random.Range(0, Colors.Length)];

        // Get all Renderer components in the Tetromino and its children
        Renderer[] renderers = block.GetComponentsInChildren<Renderer>();

        // Apply the random color to each Renderer
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = randomColor;
        }
    }
    public void ApplyGhostColor(GameObject block)
    {
        // Get all Renderer components in the ghost block and its children
        Renderer[] renderers = block.GetComponentsInChildren<Renderer>();

        // Apply the fixed gray color to each Renderer
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = Color.gray;
        }
    }

    // Method to get the last spawned index
    public int GetLastSpawnedIndex()
    {
        return lastSpawnedIndex;
    }
}

