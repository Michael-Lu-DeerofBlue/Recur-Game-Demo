using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBlock : MonoBehaviour
{
    public static int height = 24;
    public static int width = 12;
    public static int extendedWidth = 17;
    public static Transform[,] grid = new Transform[extendedWidth, height];
    private void Update()
    {
        while (true)
        {
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                this.enabled = false;
                break;
            }
        }
    }
    public void UpdateGhostBlock(float x)
    {
        Vector3 newPosition = transform.position;
        newPosition.x = x;
        transform.position = newPosition;
    }

    bool ValidMove()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
                return false;
        }
        return true;
    }
}

