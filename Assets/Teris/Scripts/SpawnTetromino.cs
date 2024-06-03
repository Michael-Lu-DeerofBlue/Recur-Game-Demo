using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTetromino : MonoBehaviour
{
    public GameObject[] Tetrominoes;
    public Color[] Colors;

    // Start is called before the first frame update
    void Start()
    {
        NewTetromino();
    }

    public void NewTetromino()
    {
        // Instantiate a random Tetromino
        GameObject NewTetromino = Instantiate(Tetrominoes[Random.Range(0, Tetrominoes.Length)], transform.position, Quaternion.identity);
        ApplyRandomColor(NewTetromino);
    }

    public void ApplyRandomColor(GameObject tetromino)
    {
        // Choose a random color from the Colors array
        Color randomColor = Colors[Random.Range(0, Colors.Length)];

        // Get all Renderer components in the Tetromino and its children
        Renderer[] renderers = tetromino.GetComponentsInChildren<Renderer>();

        // Apply the random color to each Renderer
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = randomColor;
        }
    }
}


