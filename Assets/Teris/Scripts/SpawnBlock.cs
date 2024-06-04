using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnBlock : MonoBehaviour
{
    public GameObject[] BlockShapes;
    public Color[] Colors;

    // Start is called before the first frame update
    void Start()
    {
        NewBlock();
    }

    public void NewBlock()
    {
        // Instantiate a random Tetromino
        GameObject newBlock = Instantiate(BlockShapes[Random.Range(0, BlockShapes.Length)], transform.position, Quaternion.identity);
        ApplyRandomColor(newBlock);
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

}




