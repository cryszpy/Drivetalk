using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public List<Road> allRoads; // List of all road pieces in the scene

    private void Awake()
    {
        allRoads = new List<Road>(FindObjectsByType<Road>(FindObjectsSortMode.InstanceID)); // Find all Road objects in the scene
        ConnectAdjacentRoads();
    }

    // Connect all roads that are adjacent to each other
    private void ConnectAdjacentRoads()
    {
        for (int i = 0; i < allRoads.Count; i++)
        {
            Road currentRoad = allRoads[i];

            for (int j = 0; j < allRoads.Count; j++)
            {
                // Skip connecting the road to itself
                if (i == j) continue;

                Road adjacentRoad = allRoads[j];

                // Check if the two roads are adjacent
                if (AreRoadsAdjacent(currentRoad, adjacentRoad))
                {
                    currentRoad.ConnectMarkersToAdjacentRoad(adjacentRoad); // Connect markers between adjacent roads
                }
            }
        }
    }

    // Check if two roads are adjacent (you can adjust this logic based on your needs)
    private bool AreRoadsAdjacent(Road roadA, Road roadB)
    {
        // Simple proximity check: distance between the centers of the roads
        float distanceThreshold = 20f; // Adjust the threshold as needed
        float distance = Vector3.Distance(roadA.transform.position, roadB.transform.position);
        //Debug.Log(distance + " | " + roadA.transform.position + " | " + roadB.transform.position);

        return distance <= distanceThreshold;
    }
}
