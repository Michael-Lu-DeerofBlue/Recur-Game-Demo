using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> waypointTransforms;
    public List<EdgeData> edges;
    private Graph graph;

    void Start()
    {
        InitializeGraph();
        var startWaypoint = graph.Waypoints[0];
        var targetWaypoint = graph.Waypoints[graph.Waypoints.Count - 1];
        var shortestPath = Dijkstra.FindShortestPath(graph, startWaypoint, targetWaypoint);

        if (shortestPath != null)
        {
            foreach (var waypoint in shortestPath)
            {
                Debug.Log(waypoint.Name);
            }
        }
        else
        {
            Debug.Log("No path found");
        }
    }

    public void InitializeGraph()
    {
        graph = new Graph();
        var waypoints = new Dictionary<Transform, Waypoint>();

        foreach (var transform in waypointTransforms)
        {
            var waypoint = new Waypoint(transform, transform.name);
            waypoints[transform] = waypoint;
            graph.AddWaypoint(waypoint);
        }

        foreach (var edgeData in edges)
        {
            var from = waypoints[edgeData.From];
            var to = waypoints[edgeData.To];
            graph.AddEdge(from, to, edgeData.Cost);
        }
    }
}

[System.Serializable]
public class EdgeData
{
    public Transform From;
    public Transform To;
    public float Cost;
}