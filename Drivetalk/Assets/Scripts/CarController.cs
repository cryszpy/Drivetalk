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

    [SerializeField] private PassengerList passengerList;

    [SerializeField] private GameObject destination;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Collider coll;

    public NavMeshAgent agent;

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
        Debug.Log("todo lol");
    }
}
