using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CarPointer : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    public List<GameObject> taxiStops = new();
    public List<GameObject> destinations = new();

    [SerializeField] private Rigidbody rb;

    public NavMeshAgent agent;

    [Header("STATS")]

    public List<Marker> allMarkers = new();

    // Current destination marker for pathfinding
    public Marker currentMarker;
    public Marker destinationMarker;
    public GameObject destinationObject;

    public List<Marker> path = new();

    public bool arrived;

    public bool atTaxiStop = true;

    public GameObject currentStop;

    private void Start() {

        // Find the closest marker to the car's starting position
        currentMarker = FindClosestMarker();
    }

    private void Update() {

        if (path != null && path.Count > 0 && (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU))
        {
            MoveAlongPath();
        }
    }

    public void StartDrive(GameObject destination) {

        destinationObject = destination;
        
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

    // Move the car along the path
    private void MoveAlongPath()
    {
        if (path.Count == 0) return;

        // Move towards the next marker in the path
        Marker nextMarker = path[0];

        // Move the car towards the position of the next marker
        agent.SetDestination(nextMarker.transform.position);
        //transform.position = Vector3.MoveTowards(transform.position, nextMarker.Position, agent.speed * Time.deltaTime);

        //Debug.Log(Vector3.Distance(transform.position, nextMarker.Position));

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
        Marker closestMarker = null;
        float closestDistance = float.MaxValue;

        // Iterate through all markers to find the closest one in the right lane
        foreach (var marker in allMarkers)
        {
            // Only consider markers that are in the right lane
            float distance = Vector3.Distance(transform.position, marker.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestMarker = marker;
            }
        }

        return closestMarker;
    }

    // Find the destination marker based on gameplay (for example, using a target object in the scene)
    private Marker FindDestinationMarker()
    {
        // For example, find a destination marker based on a GameObject or tag
        if (destinationObject != null)
        {
            return FindClosestMarkerToObject(destinationObject);
        }

        return null;  // In case no destination was found (could also use a default marker)
    }

    // Helper function to find the closest marker to a specific GameObject (e.g., the destination)
    private Marker FindClosestMarkerToObject(GameObject target)
    {
        Marker closestMarker = null;
        float closestDistance = float.MaxValue;

        // Iterate through all markers to find the closest one to the target GameObject
        foreach (var marker in allMarkers)
        {
            float distance = Vector3.Distance(target.transform.position, marker.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestMarker = marker;
            }
        }

        return closestMarker;
    }

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

    private void FindNearestStop() {
        List<float> stopDistances = new();

        foreach (var stop in taxiStops) {
            float distance = Vector3.Distance(stop.transform.position, rb.position);
            stopDistances.Add(distance);
        }
        
        if (stopDistances.Count != 0) {
            agent.SetDestination(taxiStops[stopDistances.IndexOf(stopDistances.Min())].transform.position);
        }

        arrived = false;
    }
}
