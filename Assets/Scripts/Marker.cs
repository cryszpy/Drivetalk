using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class Marker : MonoBehaviour
{
    [Tooltip("Reference to the car pointer script.")]
    private CarPointer carPointer;

    [Tooltip("Reference to this marker's position.")]
    public Vector3 Position { get => transform.position; }

    [Tooltip("List of all pre-made valid connections for this marker.")]
    public List<Marker> adjacentMarkers;

    [Tooltip("Boolean flag; Whether this marker is open for new connections.")]
    [SerializeField] private bool openForConnections;
    public bool OpenForConnections {
        get { return openForConnections; }
    }

    [Tooltip("A* heuristic cost to reach this marker from the start.")]
    public float gCost;

    [Tooltip("A* heuristic cost to reach the destination.")]
    public float hCost;

    [Tooltip("A* heuristic for this marker's total cost (gCost + hCost).")]
    public float fCost => gCost + hCost;

    private void Start() {

        // Finds any missing script references
        carPointer = GameObject.FindGameObjectWithTag("CarPointer").GetComponent<CarPointer>();

        // Adds this marker to the list of all markers in the game's map
        carPointer.allMarkers.Add(this);
    }

    // Connect this marker to the closest marker of the next road piece
    public void ConnectToAdjacentMarkers(List<Marker> markersToConnect)
    {
        // For each marker in the passed-in list of markers on the adjacent road piece—
        foreach (var marker in markersToConnect)
        {
            // If this marker isn't connected to the iterated marker, and the marker is open for connections—
            if (!adjacentMarkers.Contains(marker) && marker.OpenForConnections)
            {
                //Debug.DrawLine(Vector3.zero, marker.Position, Color.cyan, 10);

                // Make a connection between the two markers
                adjacentMarkers.Add(marker);

                // Make reverse connection
                marker.ConnectToAdjacentMarkers(new List<Marker> { this });
            }
        }
    }
    
    // Debug helper tools to visualize marker connections
    private void OnDrawGizmos() {

        if (Selection.activeObject == gameObject) {

            if (adjacentMarkers != null && adjacentMarkers.Count > 0) {

                foreach (var item in adjacentMarkers) {

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, item.Position);
                }
            }
            Gizmos.color = Color.white;
        }
    }
}
