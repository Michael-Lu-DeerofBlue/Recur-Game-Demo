using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightMove : MonoBehaviour
{
    public List<Transform> polygonVertices; // List of four vertices forming the polygon
    public float rotationSpeed = 1.0f; // Speed at which the spotlight rotates
    public float pointChangeInterval = 2.0f; // Time interval to change target point
    public float minDistance = 1.0f; // Minimum distance between two points

    private Vector3 currentTargetPoint;
    private Vector3 previousTargetPoint;
    private float timeSinceLastChange;

    void Start()
    {
        if (polygonVertices.Count != 4)
        {
            Debug.LogError("Polygon must have exactly 4 vertices.");
            return;
        }

        SetRandomTargetPoint(true);
    }

    void Update()
    {
        // Rotate towards the current target point
        Vector3 direction = currentTargetPoint - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Change target point after the interval
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= pointChangeInterval)
        {
            SetRandomTargetPoint(false);
            timeSinceLastChange = 0f;
        }
    }

    void SetRandomTargetPoint(bool firstPoint)
    {
        if (firstPoint)
        {
            currentTargetPoint = GetRandomPointInPolygon(polygonVertices);
            previousTargetPoint = currentTargetPoint;
        }
        else
        {
            Vector3 newTargetPoint;
            do
            {
                newTargetPoint = GetRandomPointInPolygon(polygonVertices);
            } while (Vector3.Distance(newTargetPoint, previousTargetPoint) < minDistance);

            previousTargetPoint = currentTargetPoint;
            currentTargetPoint = newTargetPoint;
        }
    }

    Vector3 GetRandomPointInPolygon(List<Transform> vertices)
    {
        // Convert Transform list to Vector3 list for easier handling
        List<Vector3> points = new List<Vector3>();
        foreach (Transform vertex in vertices)
        {
            points.Add(vertex.position);
        }

        // Generate a random point within the polygon using Barycentric coordinates
        Vector3 a = points[0];
        Vector3 b = points[1];
        Vector3 c = points[2];
        Vector3 d = points[3];

        // Randomly select a triangle within the quad
        if (Random.value < 0.5f)
        {
            return RandomPointInTriangle(a, b, c);
        }
        else
        {
            return RandomPointInTriangle(a, c, d);
        }
    }

    Vector3 RandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1)
        {
            r1 = 1 - r1;
            r2 = 1 - r2;
        }

        Vector3 randomPoint = (1 - r1 - r2) * a + r1 * b + r2 * c;
        return randomPoint;
    }

}
