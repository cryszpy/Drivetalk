using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Road : MonoBehaviour
{
    [SerializeField] protected List<Marker> carMarkers;
    [SerializeField] protected bool isCorner;
    public GameObject centerDivider;

    public List<Marker> incomingMarkers;
    public List<Marker> outgoingMarkers;

    // This is a helper function to connect the markers between adjacent road pieces.
    public void ConnectMarkersToAdjacentRoad(Road adjacentRoad)
    {
        // For corners and intersections which have multiple markers per road
        /* if (isCorner) {

            foreach (var marker in carMarkers) {

                Debug.DrawLine(Vector3.zero, marker.Position, Color.magenta, 10);
                if (marker.OpenForConnections) {

                    Marker closestMarker = FindClosestMarkerOnAdjacentRoad(marker, this);
                    //Debug.DrawLine(Vector3.zero, closestMarker.Position, Color.white, 10);
                    if (closestMarker != null) {

                        // Connect the current marker to the closest marker on the adjacent road
                        marker.ConnectToAdjacentMarkers(new List<Marker> { closestMarker });
                    }
                }
                break;
            }
        } 
        // For straight roads with only 1 marker per lane
        else  */{
            // Loop through each marker on this road piece and connect it to the next road piece's markers
            foreach (var marker in carMarkers)
            {
                if (marker.OpenForConnections && marker.adjacentMarkers.Count <= 1)
                {
                    Debug.Log(marker.adjacentMarkers.Count);
                    // Find the closest marker in the adjacent road piece
                    Marker closestMarker = FindClosestMarkerOnAdjacentRoad(marker, adjacentRoad);
                    if (closestMarker != null)
                    {
                        // Connect the current marker to the closest marker on the adjacent road
                        marker.ConnectToAdjacentMarkers(new List<Marker> { closestMarker });
                    }
                }
            }
        }
        
    }

    // This helper method finds the closest marker on an adjacent road piece to a given marker.
    private Marker FindClosestMarkerOnAdjacentRoad(Marker currentMarker, Road adjacentRoad)
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
    }
}

