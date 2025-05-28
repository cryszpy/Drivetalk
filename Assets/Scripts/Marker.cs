using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [Tooltip("Reference to the car pointer script.")]
    protected CarPointer carPointer;

    [Tooltip("Reference to this marker's position.")]
    public Vector3 Position { get => transform.position; }

    [Tooltip("List of all pre-made valid connections for this marker.")]
    public List<Marker> adjacentMarkers;

    [Tooltip("Boolean flag; Whether this marker is open for new connections.")]
    [SerializeField] private bool openForConnections;
    public bool OpenForConnections {
        get { return openForConnections; }
    }
    
    private void Awake() {

        // Finds any missing script references
        carPointer = GameObject.FindGameObjectWithTag("CarPointer").GetComponent<CarPointer>();

        // Adds this marker to the list of all markers in the game's map
        if (!carPointer.allMarkers.Contains(this)) {
            carPointer.allMarkers.Add(this);
        }
    }

    private void OnDisable() {

        if (carPointer && GameStateManager.roadManager) {

            if (carPointer.allMarkers.Contains(this)) {
                carPointer.allMarkers.Remove(this);
            }

            if (carPointer.path.Contains(this)) {
                carPointer.path.Remove(this);
            }

            if (carPointer.gpsPathRef.Contains(this)) {
                carPointer.gpsPathRef.Remove(this);
            }

            foreach (var marker in carPointer.allMarkers) {
                if (marker.adjacentMarkers.Contains(this)) {
                    marker.adjacentMarkers.Remove(this);
                }
            }
        }
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

                // Make a connection between the two markers
                adjacentMarkers.Add(marker);

                // Make reverse connection
                marker.ConnectToAdjacentMarkers(new List<Marker> { this });
            }
        }
    }
    
    #if UNITY_EDITOR
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
    #endif
}
