using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public enum MarkerType {
    NONE, LEFT, RIGHT
}

public class Marker : MonoBehaviour
{
    private CarPointer carPointer;

    public MarkerType type;

    public Vector3 Position { get => transform.position; }

    public List<Marker> adjacentMarkers;

    [SerializeField] private bool openForConnections;

    // Add cost and heuristic fields for A* algorithm
    public float gCost; // Cost to reach this node from the start
    public float hCost; // Heuristic: estimated cost to reach the destination
    public float fCost => gCost + hCost; // fCost is the total cost (gCost + hCost)

    private void Start() {
        carPointer = GameObject.FindGameObjectWithTag("CarPointer").GetComponent<CarPointer>();
        carPointer.allMarkers.Add(this);
    }

    public bool OpenForConnections {
        get { return openForConnections; }
    }

    // Connect this marker to the closest marker of the next road piece
    public void ConnectToAdjacentMarkers(List<Marker> markersToConnect)
    {
        foreach (var marker in markersToConnect)
        {
            if (!adjacentMarkers.Contains(marker) && marker.OpenForConnections)
            {
                //Debug.DrawLine(Vector3.zero, marker.Position, Color.cyan, 10);
                adjacentMarkers.Add(marker);
                marker.ConnectToAdjacentMarkers(new List<Marker> { this }); // Add reverse connection
            }
        }
    }
    
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
