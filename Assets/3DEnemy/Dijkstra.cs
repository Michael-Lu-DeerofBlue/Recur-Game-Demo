using System.Collections.Generic;
using System.Linq;

public static class Dijkstra
{
    /// <summary>
    /// Finds the shortest path between two waypoints in a graph.
    /// Returns a list of waypoints representing the shortest path from start to target, or null if no path is found.
    /// </summary>
    public static List<Waypoint> FindShortestPath(Graph graph, Waypoint start, Waypoint target)
    {
        // Dictionary to store the shortest known distance to each waypoint
        var distances = new Dictionary<Waypoint, float>();

        // Dictionary to store the previous waypoint for each waypoint in the shortest path
        var previous = new Dictionary<Waypoint, Waypoint>();

        // List of waypoints that have not been visited yet
        var unvisited = new List<Waypoint>();

        // Initialize distances and previous dictionaries
        foreach (var waypoint in graph.Waypoints)
        {
            distances[waypoint] = float.MaxValue; // Set all distances to infinity
            previous[waypoint] = null;            // Set previous waypoint to null
            unvisited.Add(waypoint);              // Add all waypoints to the unvisited list
        }

        distances[start] = 0; // Distance to the start waypoint is zero

        // Main loop to process waypoints
        while (unvisited.Count > 0)
        {
            // Sort unvisited waypoints by distance (smallest distance first)
            unvisited.Sort((a, b) => distances[a].CompareTo(distances[b]));
            var current = unvisited[0]; // Get the waypoint with the smallest known distance
            unvisited.Remove(current);  // Mark it as visited by removing it from the unvisited list

            // If the target has been reached, reconstruct and return the shortest path
            if (current == target)
            {
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

            // Update distances for neighbors of the current waypoint
            foreach (var edge in current.Edges)
            {
                var neighbor = edge.To;
                if (!unvisited.Contains(neighbor))
                {
                    continue; // Skip neighbors that have already been visited
                }

                // Calculate the new distance to the neighbor
                var newDist = distances[current] + edge.Cost;
                if (newDist < distances[neighbor])
                {
                    // Update the distance and set the current waypoint as the previous waypoint on the shortest path
                    distances[neighbor] = newDist;
                    previous[neighbor] = current;
                }
            }
        }

        return null; // No path found
    }
}
