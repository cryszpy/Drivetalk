using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [Tooltip("List of all roads in the game's map.")]
    public List<Road> allRoads = new(); // List of all road pieces in the scene

    /* private void Start()
    {
        // Find all Road objects in the scene
        ConnectAdjacentRoads();
    }

    // Connect all roads that are adjacent to each other
    public void ConnectAdjacentRoads()
    {

        // For every road—
        foreach (Road currentRoad in allRoads) {

            // For every road again—
            foreach (Road adjacentRoad in allRoads) {

                // Skip connecting the road to itself
                if (currentRoad == adjacentRoad) continue;

                // Check if the two roads are adjacent
                if (AreRoadsAdjacent(currentRoad, adjacentRoad))
                {
                    currentRoad.ConnectMarkersToAdjacentRoad(adjacentRoad); // Connect markers between adjacent roads
                    currentRoad.adjacentRoads.Add(adjacentRoad.gameObject);
                }
            }
        }
    }

    // Check if two roads are adjacent
    private bool AreRoadsAdjacent(Road roadA, Road roadB)
    {
        // Simple proximity check: distance between the centers of the roads
        float distanceThreshold = 20f;
        float distance = Vector3.Distance(roadA.transform.position, roadB.transform.position);

        return distance <= distanceThreshold;
    } */
}
