using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    public Dictionary<GameObject, Color> heroBlocks = new Dictionary<GameObject, Color>();
    public Dictionary<GameObject, GameObject> ghosthBlocksDictionary = new Dictionary<GameObject, GameObject>();
    public GameObject[] BlockShapes;
    public GameObject[] GGhostBlockShapes;
    public Color[] SetColors;
    public Color[] RandomColors;
    private GameObject NewBlock;
    private GameObject ghostBlock;
    private int lastSpawnedIndex;
    public int blockIdCounter;
    public bool SettedColors;
    // Start is called before the first frame update

    public void SpawnNewBlock(int blockIndex, Color color)
    {
        if (!checkGameEnd())
        {
            blockIdCounter++;
            lastSpawnedIndex = blockIndex;
            NewBlock = Instantiate(BlockShapes[lastSpawnedIndex], transform.position, Quaternion.identity);
            ApplySetColor(NewBlock, color);
            NewBlock.GetComponent<BlockStageController>().inFall = true;
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

    public void ApplySetColor(GameObject block, Color color)
    {
        Renderer[] renderers = block.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    public void ApplyRandomColor(GameObject block)
    {
        // Choose a random color from the Colors array
        Color randomColor = RandomColors[Random.Range(0, RandomColors.Length)];

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

