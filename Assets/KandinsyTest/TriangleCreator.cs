using UnityEngine;

public class TriangleCreator : MonoBehaviour
{
    void Start()
    {
        // Create a new GameObject with a MeshFilter and MeshRenderer
        GameObject triangle = new GameObject("Triangle");
        MeshFilter meshFilter = triangle.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = triangle.AddComponent<MeshRenderer>();

        // Create a new mesh and set its vertices, triangles, and normals
        Mesh mesh = new Mesh();

        // Define the vertices of the triangle
        mesh.vertices = new Vector3[]
        {
            new Vector3(0, 0, 0), // Vertex 0
            new Vector3(1, 0, 0), // Vertex 1
            new Vector3(0, 1, 0)  // Vertex 2
        };

        // Define the triangles (order in which vertices are connected)
        mesh.triangles = new int[]
        {
            0, 1, 2 // One triangle connecting the three vertices
        };

        // Define the normals (direction each vertex is facing)
        mesh.normals = new Vector3[]
        {
            -Vector3.forward, // Normal for Vertex 0
            -Vector3.forward, // Normal for Vertex 1
            -Vector3.forward  // Normal for Vertex 2
        };

        // Optionally, set the UVs for texturing
        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0), // UV for Vertex 0
            new Vector2(1, 0), // UV for Vertex 1
            new Vector2(0, 1)  // UV for Vertex 2
        };

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = mesh;

        // Optionally, set a material for the MeshRenderer
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }
}
