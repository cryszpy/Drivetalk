using UnityEngine;

public class GPSDestination : MonoBehaviour
{

    [SerializeField] private GameObject destinationObject;

    [SerializeField] private GPS gps;

    [SerializeField] private CarController car;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!car) {
            car = GetComponentInParent<CarController>();
            Debug.Log("Car component unassigned! Reassigned.");
        }
    }

    public void SetGPS() {
        if (car.currentRideNum < car.totalRideNum) {
            car.agent.SetDestination(destinationObject.transform.position);
            StartCoroutine(gps.EndDollyMovement());
        }
    }
}
