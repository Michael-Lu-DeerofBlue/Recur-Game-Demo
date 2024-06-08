using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBlock : MonoBehaviour
{
    private BlockManager blockManager;

    private void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        SetTransparent();
    }

    private void Update()
    {
        transform.position += new Vector3(0, -1, 0);
        if (!ValidMove())
        {
            SetOpaque();
            transform.position -= new Vector3(0, -1, 0);
            this.enabled = false;
        }
    }

    public void UpdateGhostBlock(float x)
    {
        if (!ValidMove())
        {
            transform.position -= new Vector3(x, 0, 0);
        }
    }

    bool ValidMove()
    {
        foreach (Transform child in transform)
        {
            Vector3 pos = blockManager.RoundVector(child.position);
            if (!blockManager.IsInsideGrid(pos) || blockManager.IsOccupied(pos))
            {
                return false;
            }
        }
        return true;
    }

    void SetTransparent()
    {
        // Get all Renderer components in the ghost block and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Set the color to transparent
        foreach (Renderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = 0.0f; // Set transparency (0 = fully transparent, 1 = fully opaque)
            renderer.material.color = color;
        }
    }
    void SetOpaque()
    {
        // Get all Renderer components in the ghost block and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Set the color to fully opaque
        foreach (Renderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = 1f; // Set transparency to fully opaque
            renderer.material.color = color;
        }
    }
}


