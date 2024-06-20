using System.Collections.Generic;
using System.Linq;

public static class Dijkstra
{
    public static List<Waypoint> FindShortestPath(Graph graph, Waypoint start, Waypoint target)
    {
        var distances = new Dictionary<Waypoint, float>();
        var previous = new Dictionary<Waypoint, Waypoint>();
        var unvisited = new List<Waypoint>();

        // Initialize distances and previous dictionaries
        foreach (var waypoint in graph.Waypoints)
        {
            distances[waypoint] = float.MaxValue;
            previous[waypoint] = null;
            unvisited.Add(waypoint);
        }

        distances[start] = 0;

        while (unvisited.Count > 0)
        {
            // Sort unvisited waypoints by distance
            unvisited.Sort((a, b) => distances[a].CompareTo(distances[b]));
            var current = unvisited[0];
            unvisited.Remove(current);

            if (current == target)
            {
                // Build the shortest path by following the previous nodes
                var path = new List<Waypoint>();
                while (previous[current] != null)
                {
                    path.Add(current);
                    current = previous[current];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (var edge in current.Edges)
            {
                var neighbor = edge.To;
                if (!unvisited.Contains(neighbor))
                {
                    continue;
                }

                var newDist = distances[current] + edge.Cost;
                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previous[neighbor] = current;
                }
            }
        }

        return null; // No path found
    }
}
