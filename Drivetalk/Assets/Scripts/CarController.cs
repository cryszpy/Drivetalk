using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    public List<GameObject> taxiStops = new();

    [SerializeField] private GameObject destination;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Collider coll;

    public NavMeshAgent agent;

    public bool arrived;

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
}
