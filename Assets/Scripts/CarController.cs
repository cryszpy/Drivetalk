using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")] // -------------------------------------------------------------------------------------------

    [Tooltip("Reference to the road navigator AI that the car follows.")]
    public CarPointer carPointer;

    [Tooltip("Reference to the game's dialogue manager.")]
    public DialogueManager dialogueManager;

    [Tooltip("Reference to the mood meter animator.")]
    public Animator moodMeterAnimator;

    [Tooltip("Reference to the sprite renderer component of the passenger's head on the mood meter.")]
    public Image moodMeterHandle;

    /* [Tooltip("List of all taxi stops in the game.")]
    public List<GameObject> taxiStops = new(); */

    [Tooltip("List of all the car's available passenger seats.")]
    [SerializeField] private List<GameObject> passengerSeats;

    [Tooltip("List of all dashboard gift spawn locations.")]
    public List<GiftSpawn> dashboardGiftSpawns = new();

    [Tooltip("Reference to the rearview mirror gift spawn location.")]
    public GiftSpawn rearviewGiftSpawn;

    [Tooltip("Reference to the car's Navigation Mesh AI agent component.")]
    public NavMeshAgent agent;

    [Tooltip("Reference to the car's current passenger. UPDATED DYNAMICALLY.")]
    public Passenger currentPassenger;

    [Tooltip("Reference to the invisible horizontal bar where choice button UI pops up.")]
    public GameObject choicesBar;

    [Tooltip("Reference to the prefab for a UI choice button.")]
    public GameObject choicePrefab;

    [Tooltip("Reference to the car's dropoff position for passengers.")]
    public GameObject dropoffPosition;

    public AudioSource radioSource;

    [Header("STATS")] // -------------------------------------------------------------------------------------------

    [Tooltip("Boolean flag; Checks whether the passenger has arrived at destination or not.")]
    public bool arrivedAtDest = false;

    [Tooltip("Boolean flag; Checks whether the car is currently at a taxi stop or not.")]
    public bool atTaxiStop = false;

    /* [Tooltip("The minimum possible amount of rides in a day.")]
    [SerializeField] private int minRides;

    [Tooltip("The maximum possible amount of rides in a day.")]
    [SerializeField] private int maxRides; */

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

    [Tooltip("The ID of the currently played radio channel.")]
    private static int currentRadioChannel;
    public static int CurrentRadioChannel { get => currentRadioChannel; set => currentRadioChannel = value; }

    [Tooltip("The current power status of the radio.")]
    public static bool RadioPower = true;

    [Tooltip("Current activation status of the hazard lights.")]
    public static bool HazardsActive = false;

    [Header("STATIC VARIABLE TRACKERS")] // -------------------------------------------------------------------------------------------

    public float ratingTracker;
    public float tempTracker;
    public float totalPassengersTracker;
    public float lastPassengerIDTracker;
    public float currentRadioChannelTracker;
    public bool radioPowerTracker;
    public bool hazardsTracker;

    private void OnEnable() {
        GameStateManager.EOnPassengerDropoff += DropOffPassenger;
    }

    private void OnDisable() {
        GameStateManager.EOnPassengerDropoff -= DropOffPassenger;
    }

    private void Start() {

        FindReferences();

        /* // If the day hasn't been started, generate random number of rides for today
        if (totalRideNum <= 0){
            GenerateDayRides();
        } */
    }

    private void Update() {

        // If the game isn't in a menu or paused—
        if (GameStateManager.Gamestate == GAMESTATE.PLAYING)
        {
            // Drive to the current destination
            agent.SetDestination(carPointer.pointer.transform.position);
        }

# if UNITY_EDITOR
        // DEV static variable tracking—REMOVE FOR BUILDS
        ratingTracker = Rating;
        tempTracker = Temperature;
        totalPassengersTracker = TotalPassengersDriven;
        lastPassengerIDTracker = LastPassengerID;
        currentRadioChannelTracker = CurrentRadioChannel;
        radioPowerTracker = RadioPower;
        hazardsTracker = HazardsActive;
# endif
    }

    private void FindReferences() {

        // If dialogue manager is null, reassign it
        if (!dialogueManager) {
            dialogueManager = GameStateManager.dialogueManager;
            Debug.LogWarning("dialogueManager reference on: " + name + " is null! Reassigned.");
        }

        if (!moodMeterAnimator) {
            moodMeterAnimator = GameObject.FindGameObjectWithTag("MoodMeter").GetComponent<Animator>();
            Debug.LogWarning("moodMeterAnimator reference on: " + name + " is null! Reassigned.");
        }

        if (!moodMeterHandle) {
            moodMeterHandle = GameObject.FindGameObjectWithTag("MoodMeterHandle").GetComponent<Image>();
            Debug.LogWarning("moodMeterHandle reference on: " + name + " is null! Reassigned.");
        }
    }

    // Generates the day's rides
    public void GenerateDayRides(int value) {

        // Get random number between min and max possible ride numbers
        //int rand = Random.Range(minRides, maxRides);

        // Set the day's total rides to this number
        totalRideNum = value;

        // Start the day from ride number 0
        currentRideNum = 0;

        Debug.Log("Generated rides for the day! " + totalRideNum);
    }

    /* // Finds the nearest taxi stop to the car's current location
    public void FindNearestStop() {

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
    } */

    public void StartInitialDrive() {
        carPointer.SpawnRoadTile();
    }

    // Picks up a passenger when arriving at a taxi stop
    public void PickUpPassenger(GameObject passenger) {

        // Make sure that the passenger script can be accessed, then—
        if (passenger.TryGetComponent<Passenger>(out var script)) {

            Debug.Log("Found passenger!");
            
            // Set the car's current passenger
            currentPassenger = script;

            // Tell the car it has arrived at a taxi stop
            carPointer.taxiStopsEnabled = false;
            atTaxiStop = true;
            arrivedAtDest = false;

            // Assign correct headshot of passenger to the mood meter
            moodMeterHandle.sprite = script.headshot;
            
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

            GameStateManager.EOnPassengerPickup?.Invoke();

            // Start the passenger's pickup greeting if they have one
            dialogueManager.StartDialogue(currentPassenger.archetype.pickupGreeting);

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

            // Resets rotation to face forward from seat
            currentPassenger.transform.localEulerAngles = new(0, 0, 0);

        } else {
            Debug.LogWarning("Could not find Passenger component on this passenger!");
        }
    }

    // Drops off a passenger when arriving at a destination and dialogue is finished
    public void DropOffPassenger() {
        Debug.Log("Arrived at destination!");

        carPointer.taxiStopsEnabled = true;

        // Reset line renderer
        carPointer.SetGPSPath();

        // Clears all previous dashboard requests
        GameStateManager.comfortManager.activeRequests.Clear();

        // Reset initial block routing trigger
        carPointer.setInitialBlock = false;

        // Reset destination
        carPointer.destinationObject = null;
        carPointer.destinationSpawned = false;

        // Drops off the current passenger
        if (currentPassenger) {
            dialogueManager.StartDialogue(currentPassenger.archetype.dropoffSalute);
        }
    }
}