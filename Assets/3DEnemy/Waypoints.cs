using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
    public Transform Transform { get; private set; }
    public List<Edge> Edges { get; private set; }
    public string Name { get; set; }

    public Waypoint(Transform transform, string name)
    {
        Transform = transform;
        Edges = new List<Edge>();
        Name = name;
    }
}

public class Edge
{
    public Waypoint From { get; private set; }
    public Waypoint To { get; private set; }
    public float Cost { get; private set; }

    public Edge(Waypoint from, Waypoint to, float cost)
    {
        From = from;
        To = to;
        Cost = cost;
    }
}

public class Graph
{
    public List<Waypoint> Waypoints { get; private set; }

    public Graph()
    {
        Waypoints = new List<Waypoint>();
    }

    public void AddWaypoint(Waypoint waypoint)
    {
        Waypoints.Add(waypoint);
    }

    public void AddEdge(Waypoint from, Waypoint to, float cost)
    {
        var edge = new Edge(from, to, cost);
        from.Edges.Add(edge);
    }
}
