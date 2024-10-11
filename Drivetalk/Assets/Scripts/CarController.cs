using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    [SerializeField] private GameObject destination;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Collider coll;

    public NavMeshAgent agent;

    private void Update() {
        if (Input.GetMouseButtonDown(1) && destination) {
            agent.SetDestination(destination.transform.position);
        }
    }
}
