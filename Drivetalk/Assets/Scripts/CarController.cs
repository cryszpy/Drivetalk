using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public class CarController : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    public List<GameObject> taxiStops = new();

    [SerializeField] private GameObject destination;

    [SerializeField] private List<GameObject> passengerSeats;
    [SerializeField] private GameObject shotgun;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Collider coll;

    public NavMeshAgent agent;

    private Passenger currentPassenger;

    [SerializeField] Camera rearviewMirrorCam;

    [SerializeField] GameObject dialogueNotif;

    [Header("STATS")]

    public bool arrived;

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

    private void Start() {
        // Flip camera projection horizontally (for accurate mirror effect)
        Matrix4x4 mat = rearviewMirrorCam.projectionMatrix;
        mat *= Matrix4x4.Scale(new Vector3(1, -1, 1));
        rearviewMirrorCam.projectionMatrix = mat;
    }

    private void Update() {
        if (arrived) {
            FindNearestStop();
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

    public void PickUpPassenger() {

        int index = UnityEngine.Random.Range(0, passengerSeats.Count);

        switch (index) {
            case 0:
                rearviewMirrorCam.transform.localPosition = new(-2.5f, 5.57f, -0.6f);
                rearviewMirrorCam.transform.localEulerAngles = new(-180, -24.2f, -90);
                rearviewMirrorCam.orthographicSize = 1.9f;
                break;
            case 1:
                rearviewMirrorCam.transform.localPosition = new(-0.34f, 5.2f, -0.6f);
                rearviewMirrorCam.transform.localEulerAngles = new(-180, -22, -90);
                rearviewMirrorCam.orthographicSize = 2.04f;
                break;
            case 2:
                rearviewMirrorCam.transform.localPosition = new(1.25f, 5, -0.6f);
                rearviewMirrorCam.transform.localEulerAngles = new(-180, -25, -90);
                rearviewMirrorCam.orthographicSize = 1.75f;
                break;
        }

        // Teleports passenger to seat
        currentPassenger.transform.position = passengerSeats[index].transform.position;

        // Parents passenger to seat
        currentPassenger.transform.parent = this.transform;

        // Resets rotation to face forward from seat
        currentPassenger.transform.localEulerAngles = new(0, 0, 0);
    }

    private void OnTriggerEnter(Collider collider) {

        if (collider.gameObject.layer == 9) {
            Debug.Log("Found passenger!");

            if (collider.TryGetComponent<Passenger>(out var script)) {
                currentPassenger = script;
                PickUpPassenger();
            } else {
                Debug.LogWarning("Could not find Passenger component on this passenger!");
            }
        }
    }
}
