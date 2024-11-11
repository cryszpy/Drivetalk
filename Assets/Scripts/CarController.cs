using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    [Tooltip("Reference to the road navigator AI that the car follows.")]
    public CarPointer carPointer;

    [Tooltip("Reference to the game's dialogue manager.")]
    public DialogueManager dialogueManager;

    [Tooltip("List of all taxi stops in the game.")]
    public List<GameObject> taxiStops = new();

    [Tooltip("List of all the car's available passenger seats.")]
    [SerializeField] private List<GameObject> passengerSeats;

    [Tooltip("Reference to the shotgun seat of the car.")]
    [SerializeField] private GameObject shotgun;

    [Tooltip("Reference to the car's Rigidbody component.")]
    [SerializeField] private Rigidbody rb;

    [Tooltip("Reference to the car's Navigation Mesh AI agent component.")]
    public NavMeshAgent agent;

    [Tooltip("Reference to the car's current passenger. UPDATED DYNAMICALLY.")]
    public Passenger currentPassenger;

    [Tooltip("Reference to the invisible horizontal bar where choice button UI pops up.")]
    public GameObject choicesBar;

    [Tooltip("Reference to the prefab for a UI choice button.")]
    public GameObject choicePrefab;

    [Tooltip("Reference to the car's current destination.")]
    public GameObject destinationObject;

    [Tooltip("Reference to the car's current taxi stop destination.")]
    public GameObject currentStop;

    [Header("STATS")]

    [Tooltip("Boolean flag; Checks whether the car has arrived at its destination or not.")]
    public bool arrived;

    [Tooltip("Boolean flag; Checks whether the car is currently at a taxi stop or not.")]
    public bool atTaxiStop = true;

    [Tooltip("The minimum possible amount of rides in a day.")]
    [SerializeField] private int minRides;

    [Tooltip("The maximum possible amount of rides in a day.")]
    [SerializeField] private int maxRides;

    [Tooltip("The current ride of the day.")]
    public int currentRideNum;

    [Tooltip("The total rides in the day.")]
    public int totalRideNum;

    [Tooltip("Might get rid of this.")]
    private static float rating;
    public static float Rating { get => rating; set => rating = value; }

    [Tooltip("The car's internal temperature variable.")]
    private static float temperature;
    public static float Temperature { get => temperature; set => temperature = value;}

    [Tooltip("List of all seen/driven passenger IDs thus far.")]
    public static List<int> PassengersDrivenIDs = new();
    public List<int> passengersDrivenIDsTracker = new();

    [Tooltip("List of all seen/driven passenger ride number indexes thus far. (how many rides they've taken)")]
    public static List<int> PassengersDrivenRideNum = new();
    public List<int> passengersDrivenRideNumTracker = new();

    [Tooltip("Might get rid of this.")]
    private static int totalPassengersDriven;
    public static int TotalPassengersDriven { get => totalPassengersDriven; set => totalPassengersDriven = value; }

    [Tooltip("The most recent passenger's ID number.")]
    private static int lastPassengerID;
    public static int LastPassengerID { get => lastPassengerID; set => lastPassengerID = value; }

    [Tooltip("The most recent played song's ID number.")]
    private static int lastSongPlayedID;
    public static int LastSongPlayedID { get => lastSongPlayedID; set => lastSongPlayedID = value; }

    [Header("STATIC VARIABLE TRACKERS")]

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

        // If dialogue manager is null, reassign it
        if (dialogueManager == null) {
            if (GameObject.FindGameObjectWithTag("DialogueManager").TryGetComponent<DialogueManager>(out var script)) {
                dialogueManager = script;
                Debug.LogWarning("Dialogue manager was not assigned! Reassigned.");
            } else {
                Debug.LogError("Could not find dialogue manager!");
            }
        }

        // If the day hasn't been started, generate random number of rides for today
        if (totalRideNum <= 0){
            GenerateDayRides();
        }
    }

    private void Update() {

        // If car has arrived and successfully dropped off passenger, reroute to the nearest taxi stop
        if (arrived && currentPassenger == null) {
            FindNearestStop();
        }

        // If the game isn't in a menu or paused—
        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU)
        {
            // Drive to the current destination
            agent.SetDestination(destinationObject.transform.position);
        }

        // DEV static variable tracking—REMOVE FOR BUILDS
        ratingTracker = Rating;
        tempTracker = Temperature;
        totalPassengersTracker = TotalPassengersDriven;
        lastPassengerIDTracker = LastPassengerID;
        lastSongPlayedIDTracker = LastSongPlayedID;
    }

    // Generates the day's rides
    private void GenerateDayRides() {

        // Get random number between min and max possible ride numbers
        int rand = Random.Range(minRides, maxRides);

        // Set the day's total rides to this number
        totalRideNum = rand;

        // Start the day from ride number 0
        currentRideNum = 0;

        Debug.Log("Generated rides for the day! " + totalRideNum);
    }

    // Finds the nearest taxi stop to the car's current location
    private void FindNearestStop() {

        // New list of the distances between all taxi stops and the car
        List<float> stopDistances = new();

        // For every taxi stop in the game
        foreach (var stop in taxiStops) {

            // Calculate distance between it and the car
            float distance = Vector3.Distance(stop.transform.position, rb.position);

            // Add it to the list
            stopDistances.Add(distance);
        }
        
        // Make sure distances were calculated, then—
        if (stopDistances.Count != 0) {

            // Route the car to the nearest taxi stop
            carPointer.StartDrive(taxiStops[stopDistances.IndexOf(stopDistances.Min())]);
        }

        arrived = false;
    }

    // Picks up a passenger when arriving at a taxi stop
    public void PickUpPassenger(GameObject passenger) {

        // Make sure that the passenger script can be accessed, then—
        if (passenger.TryGetComponent<Passenger>(out var script)) {

            Debug.Log("Found passenger!");
            
            // Set the car's current passenger
            currentPassenger = script;

            // Tell the car it has arrived at a taxi stop
            atTaxiStop = true;
            
            // Add passenger to total driven passengers list if they are a new passenger
            if (!PassengersDrivenIDs.Contains(currentPassenger.id)) {
                PassengersDrivenIDs.Add(currentPassenger.id);
                PassengersDrivenRideNum.Add(1);

                passengersDrivenIDsTracker.Add(currentPassenger.id);
                passengersDrivenRideNumTracker.Add(1);
            } 
            // Otherwise, increment their ride numbder
            else {
                int index = PassengersDrivenIDs.IndexOf(currentPassenger.id);
                PassengersDrivenRideNum[index]++;

                passengersDrivenRideNumTracker[index]++;
            }

            //dialogueManager.dashTicker = currentPassenger.dashRequestTickRate;

            // Start the passenger's pickup greeting if they have one
            dialogueManager.StartDialogue(currentPassenger.archetype.pickupGreeting);
            //dialogueManager.StartDialogue(currentPassenger.dialogue[currentPassenger.currentDialogueNum], false);

            // Set the passenger as having been picked up
            currentPassenger.tag = "PickedUp";

            // Increment the car's current ride number out of total rides in the day
            currentRideNum++;

            // Pick a random available seat in the car for the passenger
            int seatIndex = UnityEngine.Random.Range(0, passengerSeats.Count);

            // Teleports passenger to seat
            currentPassenger.transform.position = passengerSeats[seatIndex].transform.position;

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
