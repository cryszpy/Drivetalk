using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum SteeringDirection {
    FORWARD, LEFT, RIGHT
}

public class CarPointer : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    public CarController car;

    [Tooltip("The car pointer's Navigation Mesh AI agent component.")]
    [SerializeField] private NavMeshAgent agent;

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
    public List<GameObject> currentBlocksList = new();

    [Tooltip("The current car navigation route's path to the destination.")]
    public List<Marker> path = new();

    [Tooltip("Reference to the saved destination set before destination is switched due to unfinished dialogue. SET DYNAMICALLY")]
    public GameObject savedDestination;

    public Road trackedIntersection;

    [SerializeField] private TurnSignal turnSignal;
    public Wheel wheel;

    [Header("STATS")]

    public GameObject hoveredButton;

    [Tooltip("Boolean flag; Checks whether dialogue has finished or not.")]
    public bool finishedDialogue;

    public bool setInitialBlock = false;

    public LayerMask layerMask;

    public SteeringDirection currentSteeringDirection;

    public bool inIntersection = false;

    public bool calculatedDirections = false;
    public SteeringDirection backupSteeringDirection;

    public List<SteeringDirection> validDirections = new();

    private void Start() {

        // Find the closest marker to the car's starting position
        currentMarker = FindClosestMarker();

        if (!turnSignal) {
            turnSignal = GameObject.FindGameObjectWithTag("TurnSignal").GetComponent<TurnSignal>();
            Debug.LogWarning("TurnSignal component null! Reassigned.");
        }
        if (!wheel) {
            wheel = GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
            Debug.LogWarning("Wheel component null! Reassigned.");
        }
    }

    private void Update() {

        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            // If the car pointer has calculated a route, and the game is not in a menu or paused—
            if (path != null && path.Count > 0)
            {
                // Move the car pointer along the route
                MoveAlongPath();
            }

            // If the car has not calculated valid directions, and it is not at a taxi stop (it's driving) and there is a passenger to give a ride to–
            if (!calculatedDirections && !car.atTaxiStop && car.currentPassenger) {

                // Calculate valid directions and steer towards one
                SwitchDirection();
            }
        }
    }

    // Routes the car to the passenger's actual destination if dialogue has finished
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

        // Find the closest marker to the destination position
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

    public Vector3 SnapDirection(Vector3 direction) {

        // List of Vector3 components
        List<float> vals = new()
        {
            direction.x,
            direction.y,
            direction.z
        };

        // List of absolute values of Vector3 components
        List<float> abs = new() {
            Mathf.Abs(direction.x),
            Mathf.Abs(direction.y),
            Mathf.Abs(direction.z),
        };

        // Gets the index of the largest absolute value component
        int problemIndex = abs.IndexOf(abs.Max());

        // Keeps the original sign of the largest absolute value component
        float sign = Mathf.Sign(vals[problemIndex]);

        // Changes the largest absolute value component into 1 with the original sign applied to it, and all others to 0
        switch (problemIndex) {
            case 0:
                vals[problemIndex] = 1 * sign;
                vals[1] = 0;
                vals[2] = 0;
                break;
            case 1:
                vals[problemIndex] = 1 * sign;
                vals[0] = 0;
                vals[2] = 0;
                break;
            case 2:
                vals[problemIndex] = 1 * sign;
                vals[0] = 0;
                vals[1] = 0;
                break;
        }

        // Returns snapped Vector3
        return new(vals[0], vals[1], vals[2]);
    }

    public void GetValidDirections() {
        validDirections.Clear();

        //Debug.Log(transform.TransformDirection(Vector3.forward) + " | " + SnapDirection(transform.TransformDirection(Vector3.forward)));

        Vector3 forward = SnapDirection(transform.TransformDirection(Vector3.forward));
        Vector3 left = SnapDirection(transform.TransformDirection(Vector3.left));
        Vector3 right = SnapDirection(transform.TransformDirection(Vector3.right));

        // Raycasts to find the closest intersection ahead of the car
        if (Physics.Raycast(transform.position, forward, out RaycastHit hit, 1000, layerMask)) {

            if (hit.collider.gameObject.TryGetComponent<Road>(out var road)) {
                trackedIntersection = road;
            } else {
                Debug.LogError("Couldn't find Intersection component on this intersection!");
            }

        }
        else {
            Debug.LogWarning("Could not find any intersections ahead!");
        }

        // FORWARD

        if (Physics.Raycast(trackedIntersection.transform.position, forward, out RaycastHit hitForward, 1000, layerMask)) {
            validDirections.Add(SteeringDirection.FORWARD);
        }
        else {
            Debug.LogWarning("Could not find any intersections in the FORWARD direction!");
        }

        // LEFT

        if (Physics.Raycast(trackedIntersection.transform.position, left, out RaycastHit hitLeft, 1000, layerMask)) {
            validDirections.Add(SteeringDirection.LEFT);
        }
        else {
            Debug.LogWarning("Could not find any intersections in the LEFT direction!");
        }

        // RIGHT

        if (Physics.Raycast(trackedIntersection.transform.position, right, out RaycastHit hitRight, 1000, layerMask)) {
            validDirections.Add(SteeringDirection.RIGHT);
        }
        else {
            Debug.LogWarning("Could not find any intersections in the RIGHT direction!");
        }

        // Generates a backup steering direction for every intersection (FORWARD if possible)
        if (backupSteeringDirection != SteeringDirection.FORWARD && validDirections.Contains(SteeringDirection.FORWARD)) {
            backupSteeringDirection = SteeringDirection.FORWARD;
            
        } else if (!validDirections.Contains(backupSteeringDirection)) {
            backupSteeringDirection = validDirections[Random.Range(0, validDirections.Count)];
        }
    }

    public void SwitchDirection() {

        // Generates valid directions for the next intersection
        GetValidDirections();

        // Reset to FORWARD if possible, otherwise adjust turn signal accordingly for LEFT/RIGHT
        if (!calculatedDirections) {
            currentSteeringDirection = backupSteeringDirection;
            turnSignal.hovered = false;
            turnSignal.dragging = false;
            StartCoroutine(turnSignal.SignalClick(currentSteeringDirection));
        }

        // If the next intersection has been identified—
        if (trackedIntersection) {

            var direction = currentSteeringDirection switch
            {
                SteeringDirection.FORWARD => SnapDirection(transform.TransformDirection(Vector3.forward)),
                SteeringDirection.LEFT => SnapDirection(transform.TransformDirection(Vector3.left)),
                SteeringDirection.RIGHT => SnapDirection(transform.TransformDirection(Vector3.right)),
                _ => SnapDirection(transform.TransformDirection(Vector3.forward)),
            };

            // ONLY FOR DEBUG PURPOSES -------------------------------
            GameObject closestRoad = null;
            float closestMatch = 0;

            foreach (var adjacentRoad in trackedIntersection.adjacentRoads) {

                var vec = adjacentRoad.transform.position - trackedIntersection.transform.position;

                if (Vector3.Dot(vec, direction) > closestMatch) {
                    closestMatch = Vector3.Dot(vec, direction);
                    closestRoad = adjacentRoad;
                }
                trackedIntersection.dotProducts.Add(Vector3.Dot(vec, direction));
            }
            // --------------------------------------------------------

            Vector3 linePos = new(trackedIntersection.transform.position.x, trackedIntersection.transform.position.y + 6, trackedIntersection.transform.position.z);
            Debug.DrawRay(linePos, direction * 4, Color.magenta, 1000);

            // Raycasts from next intersection in specified direction to set destination for next intersection
            if (Physics.Raycast(trackedIntersection.transform.position, direction, out RaycastHit rayHit, 1000, layerMask)) {

                // Starts driving towards that intersection
                StartDrive(rayHit.collider.gameObject);
                Debug.Log("Set destination to next intersection.");
            }
            else {
                Debug.LogError("Could not find any intersections in the " + direction + " direction!");
            }
        }

        calculatedDirections = true;
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

            var heading = transform.position - marker.Position;

            float dot = Vector3.Dot(heading, transform.forward);

            // If the marker's distance is lower than the previously calculated distance and the marker is in front of the car, set it to this distance
            if (distance < closestDistance && dot < 0)
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

    #if UNITY_EDITOR
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
    #endif
}
