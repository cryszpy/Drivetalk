using System.Collections.Generic;
using UnityEngine;


public class Road : MonoBehaviour
{
    [Tooltip("List of all car markers on this road piece.")]
    [SerializeField] protected List<Marker> carMarkers;

    /* public List<GameObject> adjacentRoads = new();
    public List<float> dotProducts = new(); */

    private void Start() {

        // If the list of all roads does not have this road, add it
        if (!GameStateManager.roadManager.allRoads.Contains(this)) {
            GameStateManager.roadManager.allRoads.Add(this);
        }
    }

    private void OnDisable() {

        if (GameStateManager.roadManager.allRoads.Contains(this)) {
            GameStateManager.roadManager.allRoads.Remove(this);
        }
    }

    // This is a helper function to connect the markers between adjacent road pieces.
    /* public void ConnectMarkersToAdjacentRoad(Road adjacentRoad)
    {
        // Loop through each marker on this road piece and connect it to the next road piece's markers
        foreach (var marker in carMarkers)
        {
            if (marker.OpenForConnections && marker.adjacentMarkers.Count <= 1)
            {
                // Find the closest marker in the adjacent road piece
                Marker closestMarker = FindClosestMarkerOnAdjacentRoad(marker, adjacentRoad);
                if (closestMarker != null)
                {
                    // Connect the current marker to the closest marker on the adjacent road
                    marker.ConnectToAdjacentMarkers(new List<Marker> { closestMarker });
                }
            }
        }
    } */

    // This helper method finds the closest marker on an adjacent road piece to a given marker.
    /* private Marker FindClosestMarkerOnAdjacentRoad(Marker currentMarker, Road adjacentRoad)
    {
        Marker closestMarker = null;
        float closestDistance = float.MaxValue;

        // Loop through all markers on the adjacent road piece and find the closest one
        foreach (var marker in adjacentRoad.carMarkers)
        {
            if (marker.OpenForConnections && marker != currentMarker)
            {
                float distance = Vector3.Distance(currentMarker.Position, marker.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMarker = marker;
                }
            }
        }
        return closestMarker;
    } */
}

