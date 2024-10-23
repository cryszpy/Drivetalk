using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public class CarController : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    [SerializeField] DialogueManager dialogueManager;

    public List<GameObject> taxiStops = new();

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

    public bool arrived;

    private static float rating;
    public static float Rating { get => rating; set => rating = value; }

    private static ACSetting temperature;
    public static ACSetting Temperature { get => temperature; set => temperature = value;}

    public static List<Passenger> passengersDrivenList = new();

    private static int totalPassengersDriven;
    public static int TotalPassengersDriven { get => totalPassengersDriven; set => totalPassengersDriven = value; }

    private static int lastPassengerID;
    public static int LastPassengerID { get => lastPassengerID; set => lastPassengerID = value; }

    private static int lastSongPlayedID;
    public static int LastSongPlayedID { get => lastSongPlayedID; set => lastSongPlayedID = value; }

    public float ratingTracker;
    public ACSetting tempTracker;
    public float totalPassengersTracker;
    public float lastPassengerIDTracker;
    public float lastSongPlayedIDTracker;

    private void Start() {
        // Flip camera projection horizontally (for accurate mirror effect)
        Matrix4x4 mat = rearviewMirrorCam.projectionMatrix;
        mat *= Matrix4x4.Scale(new Vector3(1, -1, 1));
        rearviewMirrorCam.projectionMatrix = mat;

        if (!dialogueManager) {
            if (GameObject.FindGameObjectWithTag("DialogueManager").TryGetComponent<DialogueManager>(out var script)) {
                dialogueManager = script;
                Debug.LogWarning("Dialogue manager was not assigned! Reassigned.");
            } else {
                Debug.LogError("Could not find dialogue manager!");
            }
        }
    }

    private void Update() {
        if (arrived) {
            FindNearestStop();
        }

        ratingTracker = Rating;
        tempTracker = Temperature;
        totalPassengersTracker = TotalPassengersDriven;
        lastPassengerIDTracker = LastPassengerID;
        lastSongPlayedIDTracker = LastSongPlayedID;
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

    public void PickUpPassenger() {

        currentPassenger.tag = "PickedUp";

        int index = UnityEngine.Random.Range(0, passengerSeats.Count);

        /* switch (index) {
            case 0:
                rearviewMirrorCam.transform.localPosition = new(-1.43f, 1.73f, -0.6f);
                rearviewMirrorCam.transform.localEulerAngles = new(-180, -18f, -90);
                rearviewMirrorCam.orthographicSize = 1.74f;
                break;
            case 1:
                rearviewMirrorCam.transform.localPosition = new(-0.56f, 1.73f, -0.6f);
                rearviewMirrorCam.transform.localEulerAngles = new(-180, -25, -90);
                rearviewMirrorCam.orthographicSize = 1.74f;
                break;
            case 2:
                rearviewMirrorCam.transform.localPosition = new(1.03f, 1.73f, -0.6f);
                rearviewMirrorCam.transform.localEulerAngles = new(-180, -25, -90);
                rearviewMirrorCam.orthographicSize = 1.74f;
                break;
        } */

        // Teleports passenger to seat
        currentPassenger.transform.position = passengerSeats[index].transform.position;

        // Parents passenger to seat
        currentPassenger.transform.parent = this.transform;
        Debug.Log("parented");

        // Resets rotation to face forward from seat
        currentPassenger.transform.localEulerAngles = new(0, 0, 0);
    }

    private void OnTriggerEnter(Collider collider) {

        if (collider.gameObject.layer == 9 && !collider.gameObject.CompareTag("PickedUp")) {
            Debug.Log("Found passenger!");

            if (collider.TryGetComponent<Passenger>(out var script)) {
                currentPassenger = script;
                dialogueManager.StartDialogue(currentPassenger.dialogue[currentPassenger.currentDialogueNum], false);

                PickUpPassenger();

            } else {
                Debug.LogWarning("Could not find Passenger component on this passenger!");
            }
        }
    }
}
