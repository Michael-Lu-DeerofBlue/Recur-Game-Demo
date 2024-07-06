using UnityEngine;

public class MeshHeightCropper : MonoBehaviour
{
    public float cropHeight = 1.0f; // Height to crop

    void Start()
    {
        // Get the MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter component not found.");
            return;
        }

        // Get the mesh
        Mesh mesh = meshFilter.mesh;
        if (mesh == null)
        {
            Debug.LogError("Mesh not found.");
            return;
        }

        // Get the vertices of the mesh
        Vector3[] vertices = mesh.vertices;

        // Iterate over the vertices and crop their height
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y > cropHeight)
            {
                vertices[i].y = cropHeight;
            }
        }

        // Update the mesh with the modified vertices
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
