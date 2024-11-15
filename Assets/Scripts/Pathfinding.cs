using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public static List<Marker> FindPath(Marker startMarker, Marker destinationMarker)
    {
        // Priority queue to explore nodes, ordered by the lowest F score
        var openList = new List<Marker> { startMarker };
        var closedList = new List<Marker>();  // Nodes we've already processed

        // Dictionaries to hold g and f scores
        Dictionary<Marker, float> gScores = new Dictionary<Marker, float>();
        Dictionary<Marker, float> fScores = new Dictionary<Marker, float>();
        Dictionary<Marker, Marker> cameFrom = new Dictionary<Marker, Marker>();  // For tracing the path

        // Initialize start marker's scores
        gScores[startMarker] = 0f;
        fScores[startMarker] = Vector3.Distance(startMarker.Position, destinationMarker.Position);

        // Main A* loop
        while (openList.Count > 0)
        {
            // Get the marker with the lowest F score
            Marker currentMarker = GetLowestFScoreMarker(openList, fScores);
            
            // If we've reached the destination marker, reconstruct the path
            if (currentMarker == destinationMarker)
            {
                return ReconstructPath(cameFrom, currentMarker);
            }

            // Move current marker from open to closed
            openList.Remove(currentMarker);
            closedList.Add(currentMarker);

            // Process each neighbor
            foreach (Marker neighbor in currentMarker.adjacentMarkers)
            {
                if (closedList.Contains(neighbor))
                    continue;  // Skip already processed markers

                // Calculate tentative g score
                float tentativeGScore = gScores[currentMarker] + Vector3.Distance(currentMarker.Position, neighbor.Position);

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);  // Add to open list if not already there

                // If this is a better path to the neighbor, update scores
                if (!gScores.ContainsKey(neighbor) || tentativeGScore < gScores[neighbor])
                {
                    cameFrom[neighbor] = currentMarker;  // Set the parent
                    gScores[neighbor] = tentativeGScore;
                    fScores[neighbor] = gScores[neighbor] + Vector3.Distance(neighbor.Position, destinationMarker.Position);
                }
            }
        }

        // Return an empty list if no path is found
        return new List<Marker>();
    }

    // Helper method to get the marker with the lowest F score from the open list
    private static Marker GetLowestFScoreMarker(List<Marker> openList, Dictionary<Marker, float> fScores)
    {
        Marker lowest = openList[0];
        foreach (var marker in openList)
        {
            if (fScores[marker] < fScores[lowest])
                lowest = marker;
        }
        return lowest;
    }

    // Helper method to reconstruct the path from the cameFrom dictionary
    private static List<Marker> ReconstructPath(Dictionary<Marker, Marker> cameFrom, Marker current)
    {
        var path = new List<Marker> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);  // Insert at the beginning to get the correct path order
        }
        return path;
    }
}