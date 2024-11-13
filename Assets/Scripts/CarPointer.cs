using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CarPointer : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    [SerializeField] private CarController car;

    [Tooltip("The car pointer's Navigation Mesh AI agent component.")]
    [SerializeField] private NavMeshAgent agent;

    [Tooltip("Reference to the current taxi stop.")]
    public GameObject currentStop;

    [Header("STATS")]

    [Tooltip("List of all road markers on the game's map.")]
    public List<Marker> allMarkers = new();

    [Tooltip("Reference to the currently tracked road marker for navigation purposes.")]
    public Marker currentMarker;

    [Tooltip("Reference to the destination target's closest road marker.")]
    public Marker destinationMarker;

    [Tooltip("Reference to the destination target object.")]
    public GameObject destinationObject;

    public DestinationRadius destinationRadius;

    [Tooltip("Reference to the last saved block that the car has been to.")]
    public GameObject savedBlock;

    [Tooltip("List of currently detected block markers to avoid.")]
    public List<GameObject> currentBlocksList;

    [Tooltip("The current car navigation route's path to the destination.")]
    public List<Marker> path = new();

    [Tooltip("Reference to the saved destination set before destination is switched due to unfinished dialogue. SET DYNAMICALLY")]
    public GameObject savedDestination;

    [Tooltip("Boolean flag; Checks whether dialogue has finished or not.")]
    public bool finishedDialogue;

    private void Start() {

        // Find the closest marker to the car's starting position
        currentMarker = FindClosestMarker();
    }

    private void Update() {

        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU) {

            // If the car pointer has calculated a route, and the game is not in a menu or paused—
            if (path != null && path.Count > 0)
            {
                // Move the car pointer along the route
                MoveAlongPath();
            }
        }
    }

    public void SwitchToFinalDestination() {
        
        if (destinationObject != savedDestination) {
            Debug.Log("Finished dialogue, driving to actual destination now!");

            destinationObject = savedDestination;

            StartDrive(destinationObject);
        }
    }

    // Calculate the closest route through the road layout to the destination passed in
    public void StartDrive(GameObject destination) {

        // Set the car pointer's destination
        destinationObject = destination;

        if (destinationObject.CompareTag("Destination")) {
            destinationRadius = destinationObject.GetComponentInChildren<DestinationRadius>();
            Debug.Log("Assigned destination radius!");
        }
        
        // Find the closest marker to the car's starting position
        currentMarker = FindClosestMarker();

        // You can set the destination marker manually or dynamically based on gameplay
        destinationMarker = FindDestinationMarker();

        // Find the path using A* pathfinding
        if (currentMarker != null && destinationMarker != null)
        {
            path = Pathfinding.FindPath(currentMarker, destinationMarker);
        } 
        else {
            Debug.LogError("Either currentMarker or destinationMarker are null!");
        }
    }

    // Move the car pointer along the path
    private void MoveAlongPath()
    {
        // If the route has ended, return from function and do nothing
        if (path.Count == 0) return;

        // Move towards the next marker in the path
        Marker nextMarker = path[0];

        // Move the car towards the position of the next marker
        agent.SetDestination(nextMarker.transform.position);

        // If the car reaches the next marker, remove it from the path
        if (Vector3.Distance(transform.position, nextMarker.Position) < 4f)
        {
            currentMarker = nextMarker;
            path.RemoveAt(0);  // Remove the marker from the path
        }
    }

    // Find the closest marker to the car's current position
    private Marker FindClosestMarker()
    {
        // Set temporary variables
        Marker closestMarker = null;
        float closestDistance = float.MaxValue;

        // Iterate through all markers to find the closest one
        foreach (var marker in allMarkers)
        {
            // Calculates the distance between current marker and iterated marker
            float distance = Vector3.Distance(transform.position, marker.Position);

            // If the marker's distance is lower than the previously calculated distance, set it to this distance
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestMarker = marker;
            }
        }

        // Returns the marker with the shortest distance to the current marker
        return closestMarker;
    }

    // Finds the closest marker to the set destination
    private Marker FindDestinationMarker()
    {
        // If the destination has been set—
        if (destinationObject != null)
        {
            // Find the closest marker to the destination
            return FindClosestMarkerToObject(destinationObject);
        }

        return null;  // In case no destination was found
    }

    // Finds the closest marker to a specific GameObject (e.g., the destination)
    private Marker FindClosestMarkerToObject(GameObject target)
    {
        // Set temporary variables
        Marker closestMarker = null;
        float closestDistance = float.MaxValue;

        // Iterate through all markers to find the closest one to the target GameObject
        foreach (var marker in allMarkers)
        {
            // Calculates the distance between current marker and iterated marker
            float distance = Vector3.Distance(target.transform.position, marker.Position);

            // If the marker's distance is lower than the previously calculated distance, set it to this distance
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestMarker = marker;
            }
        }

        // Returns the marker with the shortest distance to the current marker
        return closestMarker;
    }

    // Helper debug tools to visualize navigation's shortest calculated path
    private void OnDrawGizmos()
    {
        // Visualize the path using Gizmos
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].Position, path[i + 1].Position);
            }
        }
    }
}
