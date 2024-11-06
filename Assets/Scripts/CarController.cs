using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    public CarPointer carPointer;

    public DialogueManager dialogueManager;

    public List<GameObject> taxiStops = new();
    public List<GameObject> destinations = new();

    [SerializeField] private List<GameObject> passengerSeats;
    [SerializeField] private GameObject shotgun;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Collider coll;

    public NavMeshAgent agent;

    public Passenger currentPassenger;

    [SerializeField] private Camera rearviewMirrorCam;

    public GameObject choicesBar;
    public GameObject choicePrefab;

    [Header("STATS")]

    public List<Marker> allMarkers = new();

    // Current destination marker for pathfinding
    public Marker currentMarker;
    public Marker destinationMarker;
    public GameObject destinationObject;

    public List<Marker> path = new();

    public bool arrived;

    public bool atTaxiStop = true;

    [SerializeField] private int minRides;
    [SerializeField] private int maxRides;

    public GameObject currentStop;

    public int currentRideNum;
    public int totalRideNum;

    private static float rating;
    public static float Rating { get => rating; set => rating = value; }

    private static float temperature;
    public static float Temperature { get => temperature; set => temperature = value;}

    public static List<Passenger> passengersDrivenList = new();

    private static int totalPassengersDriven;
    public static int TotalPassengersDriven { get => totalPassengersDriven; set => totalPassengersDriven = value; }

    private static int lastPassengerID;
    public static int LastPassengerID { get => lastPassengerID; set => lastPassengerID = value; }

    private static int lastSongPlayedID;
    public static int LastSongPlayedID { get => lastSongPlayedID; set => lastSongPlayedID = value; }

    public float ratingTracker;
    public float tempTracker;
    public float totalPassengersTracker;
    public float lastPassengerIDTracker;
    public float lastSongPlayedIDTracker;

    private void Start() {

        // Flip camera projection horizontally (for accurate mirror effect)
        /* Matrix4x4 mat = rearviewMirrorCam.projectionMatrix;
        mat *= Matrix4x4.Scale(new Vector3(1, -1, 1));
        rearviewMirrorCam.projectionMatrix = mat; */

        if (dialogueManager == null) {
            if (GameObject.FindGameObjectWithTag("DialogueManager").TryGetComponent<DialogueManager>(out var script)) {
                dialogueManager = script;
                Debug.LogWarning("Dialogue manager was not assigned! Reassigned.");
            } else {
                Debug.LogError("Could not find dialogue manager!");
            }
        }

        if (totalRideNum <= 0){
            GenerateDayRides();
        }
    }

    private void Update() {
        if (arrived && currentPassenger == null) {
            FindNearestStop();
        }

        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU)
        {
            agent.SetDestination(destinationObject.transform.position);
        }

        // DEV static variable tracking—REMOVE FOR BUILDS
        ratingTracker = Rating;
        tempTracker = Temperature;
        totalPassengersTracker = TotalPassengersDriven;
        lastPassengerIDTracker = LastPassengerID;
        lastSongPlayedIDTracker = LastSongPlayedID;
    }

    private void GenerateDayRides() {
        int rand = Random.Range(minRides, maxRides);

        totalRideNum = rand;

        currentRideNum = 0;

        Debug.Log("Generated rides for the day! " + totalRideNum);
    }

    private void FindNearestStop() {
        List<float> stopDistances = new();

        foreach (var stop in taxiStops) {
            float distance = Vector3.Distance(stop.transform.position, rb.position);
            stopDistances.Add(distance);
        }
        
        if (stopDistances.Count != 0) {
            carPointer.StartDrive(taxiStops[stopDistances.IndexOf(stopDistances.Min())]);
        }

        arrived = false;
    }

    public void PickUpPassenger(GameObject passenger) {

        if (passenger.TryGetComponent<Passenger>(out var script)) {

            Debug.Log("Found passenger!");
            
            currentPassenger = script;
            //dialogueManager.dashTicker = currentPassenger.dashRequestTickRate;
            dialogueManager.StartDialogue(currentPassenger.archetype.pickupGreeting, true);
            //dialogueManager.StartDialogue(currentPassenger.dialogue[currentPassenger.currentDialogueNum], false);

            currentPassenger.tag = "PickedUp";

            int index = UnityEngine.Random.Range(0, passengerSeats.Count);

            currentRideNum++;

            // Teleports passenger to seat
            currentPassenger.transform.position = passengerSeats[index].transform.position;

            // Parents passenger to seat
            currentPassenger.transform.parent = this.transform;
            Debug.Log("parented");

            // Resets rotation to face forward from seat
            currentPassenger.transform.localEulerAngles = new(0, 0, 0);

        } else {
            Debug.LogWarning("Could not find Passenger component on this passenger!");
        }
    }
}
