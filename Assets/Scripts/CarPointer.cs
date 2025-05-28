using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum SteeringDirection {
    FORWARD, LEFT, RIGHT
}

public class CarPointer : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")] // --------------------------------------------------------------------------------------

    public CarController car;

    [Tooltip("The car pointer's Navigation Mesh AI agent component.")]
    public NavMeshAgent agent;

    [Tooltip("Reference to the car pointer's rigidbody component.")]
    public Rigidbody rb;

    [Tooltip("Reference to the currently tracked road marker for navigation purposes.")]
    public Marker currentMarker;

    [Tooltip("Reference to the destination target's closest road marker.")]
    public Marker destinationMarker;

    [Tooltip("List of all road markers on the game's map.")]
    public List<Marker> allMarkers = new();

    [Tooltip("The pointer that the car should target for pathfinding.")]
    public GameObject pointer;

    [Tooltip("Reference to the current pathfinding target road.")]
    public GameObject pathfindingTarget;

    [Tooltip("The current passenger's requested destination.")]
    public GameObject destinationObject;

    public GameObject currentDestinationTile;

    [HideInInspector] public DestinationRadius destinationRadius;

    [Tooltip("Reference to the last saved block that the car has been to.")]
    [HideInInspector] public GameObject savedBlock;

    [Tooltip("List of currently detected block markers to avoid.")]
    [HideInInspector] public List<GameObject> currentBlocksList = new();

    [Tooltip("The current car navigation route's path to the destination.")]
    public List<Marker> path = new();

    [HideInInspector] public List<Marker> gpsPathRef = new();
    [HideInInspector] public List<Vector3> gpsPath = new();

    public TurnSignal turnSignal;
    public Wheel wheel;

    public LineController lc;

    [HideInInspector] public GameObject hoveredButton;

    [Header("STATS")] // --------------------------------------------------------------------------------------

    public bool readyToSpawnDest;

    public bool setInitialBlock = false;

    //public LayerMask layerMask;
    public LayerMask gpsMask;

    public SteeringDirection currentSteeringDirection;

    public bool inIntersection = false;
    public bool atStopSign = false;

    public bool calculatedDirections = false;

    [Header("PROCEDURAL GENERATION")] // --------------------------------------------------------------------------------------

    [Tooltip("The GameObject for the car's GPS icon.")]
    public GameObject gpsObject;

    public bool taxiStopsEnabled = true;

    public RoadList roadList;

    public ProceduralRoad initialRoad;

    public GameObject backgroundRoad;

    public ProceduralRoad currentRoad;

    public RoadConnectionPoint furthestConnectionPoint;

    public Queue<ProceduralRoad> roadQueue = new();
    public Queue<int> directionQueue = new();
    public List<ProceduralRoad> roadQueueTracker = new();
    public List<int> directionQueueTracker = new();

    [HideInInspector] public bool destinationSpawned = false;

    public int defaultRotation = 0;

    private void OnEnable() {
        GameStateManager.EOnRoadConnected += PathfindToTarget;
    }

    private void OnDisable() {
        DisconnectEvents();
    }

    private void DisconnectEvents() {
        GameStateManager.EOnRoadConnected -= PathfindToTarget;
    }

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
        roadQueueTracker = roadQueue.ToList();
        directionQueueTracker = directionQueue.ToList();

        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            // If the car pointer has calculated a route, and the game is not in a menu or paused—
            if (path != null && path.Count > 0 && !car.atTaxiStop && !car.arrivedAtDest)
            {
                if (agent.speed != car.agent.speed && !atStopSign) agent.speed = car.agent.speed;
                
                // Move the car pointer along the route
                MoveAlongPath();

                // Don't enable GPS when there's no passenger
                if (car.currentPassenger) {
                    
                    // Set GPS path
                    SetGPSPath();
                }
            } else {
                agent.speed = 0;
            }
        }
    }

    // Routes the car to the passenger's actual destination if dialogue has finished
    /* public void SwitchToFinalDestination() {
        
        if (pathfindingTarget != savedDestination) {
            Debug.Log("Finished dialogue, driving to actual destination now!");

            pathfindingTarget = savedDestination;

            StartDrive(pathfindingTarget);
        }
    } */

    // Calculate the closest route through the road layout to the destination passed in
    public void StartDrive(GameObject destination) {

        // Set the car pointer's destination
        pathfindingTarget = destination;
        
        // Find the closest marker to the car's starting position
        currentMarker = FindClosestMarker();

        // Find the closest marker to the destination position
        destinationMarker = FindDestinationMarker(destination);

        // Find the path using A* pathfinding
        if (currentMarker != null && destinationMarker != null)
        {
            path = Pathfinding.FindPath(currentMarker, destinationMarker);
        } 
        else {
            Debug.LogError("Either currentMarker or destinationMarker are null!");
        }
    }

    // Calculate the closest route through the road layout to the destination passed in
    public void SetGPSPath() {

        // Reset both GPS path lists
        ResetLineRenderer();

        // Adds carPointer location to start line from
        gpsPath.Add(new(gpsObject.transform.position.x, gpsObject.transform.position.y - 10f, gpsObject.transform.position.z));

        // For each road marker in previously grabbed path—
        foreach (Marker point in path) {

            // Raycast into the ground
            if (Physics.Raycast(point.transform.position, Vector3.down, out RaycastHit hit, 500f, gpsMask)) {
                
                // If the raycast hits a *new* GPS tile—
                if (!gpsPath.Contains(hit.collider.gameObject.transform.position)) {
                    
                    // Adds hit gps path marker to the current path
                    gpsPath.Add(hit.collider.gameObject.transform.position);
                }
            }
        }

        // Add calculated gps path to line renderer
        lc.SetUpLine(gpsPath.ToArray());
    }

    public void ResetLineRenderer() {
        gpsPathRef.Clear();
        gpsPath.Clear();
    }

    /* private Vector3 SnapDirection(Vector3 direction) {

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
    } */

    /* public void GetValidDirections() {
        validDirections.Clear();

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
    } */

    public void SpawnRoadTile() {

        // Reset stats
        calculatedDirections = false;
        currentRoad = null;

        // Adds initial road to queue if not already
        if (initialRoad && !roadQueue.Contains(initialRoad)) {
            roadQueue.Enqueue(initialRoad);

            directionQueue.Enqueue(0);
        }
        
        // Remove previous road tile
        if (roadQueue.Count >= 3) {
            ProceduralRoad discardRoad = roadQueue.Dequeue();
            int discardDirection = directionQueue.Dequeue();

            // Remove the background tile when we can no longer see it
            GameStateManager.instance.backgroundRoad.SetActive(false);

            // Sets the steering direction for the upcoming turn to enable turn signal
            if (roadQueue.Count >= 2) {
                currentSteeringDirection = directionQueue.First() switch
                {
                    0 => SteeringDirection.FORWARD,
                    90 => SteeringDirection.RIGHT,
                    -90 => SteeringDirection.LEFT,
                    _ => SteeringDirection.FORWARD,
                };
            }

            // If the initial road is discarded, set to null to avoid errors
            if (discardRoad == initialRoad) {
                initialRoad = null;
            }

            Destroy(discardRoad.gameObject);
        }

        // Picks random tile to spawn (or destination)
        GameObject roadFromList = null;
        GameObject selectedTile = null;

        // If dialogue is finished, spawn destination tile
        if (readyToSpawnDest && !destinationSpawned) {

            if (currentDestinationTile) {
                roadFromList = currentDestinationTile;
                selectedTile = Instantiate(roadFromList, furthestConnectionPoint.transform.position, Quaternion.identity);
                
                destinationObject = selectedTile;

                destinationSpawned = true;
            } else {
                Debug.LogError("Tried to spawn non-existing destination!");
            }

        } 
        // Otherwise just pick a random tile
        else {
            roadFromList = roadList.allRoadsList[UnityEngine.Random.Range(0, roadList.allRoadsList.Count)];
            selectedTile = Instantiate(roadFromList, furthestConnectionPoint.transform.position, Quaternion.identity);
        }

        // Checks to make sure roads were properly assigned
        if (!roadFromList || !selectedTile) {
            Debug.LogError("Tried to spawn null tile!");
        }

        if (selectedTile.TryGetComponent<ProceduralRoad>(out var script)) {

            currentRoad = script;

            // Add the current road to the queue
            roadQueue.Enqueue(script);
            
            // Creates a temporary list that is an exact copy of the selected road tile's connection points
            List<RoadConnectionPoint> tempList = new(script.roadConnections);

            // Gets the initial connection point of the road tile
            RoadConnectionPoint discardPoint = tempList.Find(x => x.initialConnection == true);

            // Removes the initial connection point from the list (don't want to go backwards)
            tempList.Remove(discardPoint);

            // Randomly picks one direction from list of valid directions - A
            RoadConnectionPoint randomPoint = tempList[UnityEngine.Random.Range(0, tempList.Count)];
            directionQueue.Enqueue(randomPoint.connectionRotation);

            // Rotates spawned road tile to correct rotation
            selectedTile.transform.Rotate(new(0, defaultRotation, 0));

            // Updates the default rotation based on orientation of new road tile
            defaultRotation += randomPoint.connectionRotation;

            // Sets the furthest connection point to the selected connection point
            furthestConnectionPoint = randomPoint;
            
            // Connect new road markers to existing road markers
            script.initialMarker.ConnectByRaycast();

        } else {
            Debug.LogError("Could not find ProceduralRoad component on road!");
        }
    }

    public void PathfindToTarget() {
        
        if (currentRoad) {

            // Starts driving towards that intersection
            StartDrive(currentRoad.center);
        } else {
            Debug.LogWarning("Cannot pathfind, as currentRoad is null!");
        }
    }

    /* public void SwitchDirection() {

        // Generates valid directions for the next intersection
        GetValidDirections();

        // Reset to FORWARD if possible, otherwise adjust turn signal accordingly for LEFT/RIGHT
        if (!calculatedDirections) {
            currentSteeringDirection = backupSteeringDirection;
            turnSignal.hovered = false;
            turnSignal.dragging = false;
            //StartCoroutine(turnSignal.SignalClick(currentSteeringDirection));
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
    } */

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
    public Marker FindClosestMarker()
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
    private Marker FindDestinationMarker(GameObject target)
    {
        // If the destination has been set—
        if (target != null)
        {
            // Find the closest marker to the destination
            return FindClosestMarkerToObject(target);
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

        if (gpsPathRef != null && gpsPathRef.Count > 0)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < gpsPathRef.Count - 1; i++)
            {
                Gizmos.DrawLine(gpsPathRef[i].Position, gpsPathRef[i + 1].Position);
            }
        }
    }
    #endif
}
