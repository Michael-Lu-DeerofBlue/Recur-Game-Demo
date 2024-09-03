using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a waypoint in the graph, containing a position (Transform), a list of edges (connections to other waypoints), and a name.
/// </summary>
public class Waypoint
{
    public Transform Transform { get; private set; }  // The position of the waypoint
    public List<Edge> Edges { get; private set; }     // Connections to other waypoints
    public string Name { get; set; }                  // The name of the waypoint

    /// <summary>
    /// Constructs a new Waypoint with the specified transform and name.
    /// </summary>
    /// <param name="transform">The Transform representing the waypoint's position.</param>
    /// <param name="name">The name of the waypoint.</param>
    public Waypoint(Transform transform, string name)
    {
        Transform = transform;
        Edges = new List<Edge>();
        Name = name;
    }
}

/// <summary>
/// Represents an edge in the graph, which is a connection between two waypoints with an associated cost.
/// </summary>
public class Edge
{
    public Waypoint From { get; private set; }  // The starting waypoint of the edge
    public Waypoint To { get; private set; }    // The ending waypoint of the edge
    public float Cost { get; private set; }     // The cost of traveling along this edge

    /// <summary>
    /// Constructs a new Edge connecting two waypoints with a specified cost.
    /// </summary>
    /// <param name="from">The starting waypoint.</param>
    /// <param name="to">The ending waypoint.</param>
    /// <param name="cost">The cost associated with this edge.</param>
    public Edge(Waypoint from, Waypoint to, float cost)
    {
        From = from;
        To = to;
        Cost = cost;
    }
}

/// <summary>
/// Represents a graph composed of waypoints and edges. Provides methods to add waypoints, connect them with edges, and retrieve waypoints.
/// </summary>
public class Graph
{
    public List<Waypoint> Waypoints { get; private set; }  // List of all waypoints in the graph

    public Graph()
    {
        Waypoints = new List<Waypoint>();
    }

    /// <summary>
    /// Adds a waypoint to the graph.
    /// </summary>
    /// <param name="waypoint">The waypoint to add.</param>
    public void AddWaypoint(Waypoint waypoint)
    {
        Waypoints.Add(waypoint);
    }

    /// <summary>
    /// Adds an edge connecting two waypoints in the graph.
    /// </summary>
    /// <param name="from">The starting waypoint of the edge.</param>
    /// <param name="to">The ending waypoint of the edge.</param>
    /// <param name="cost">The cost associated with traveling along this edge.</param>
    public void AddEdge(Waypoint from, Waypoint to, float cost)
    {
        var edge = new Edge(from, to, cost);
        from.Edges.Add(edge);
    }

    /// <summary>
    /// Retrieves a waypoint in the graph based on its transform.
    /// </summary>
    /// <param name="transform">The transform of the waypoint to find.</param>
    /// <returns>The corresponding waypoint, or null if not found.</returns>
    public Waypoint GetWaypoint(Transform transform)
    {
        foreach (var waypoint in Waypoints)
        {
            if (waypoint.Transform == transform)
            {
                return waypoint;
            }
        }
        return null;
    }
}
