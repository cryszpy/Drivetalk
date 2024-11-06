using UnityEngine;

public class GPSDestination : MonoBehaviour
{

    [SerializeField] private GameObject destinationObject;

    [SerializeField] private GPS gps;

    [SerializeField] private CarPointer carPointer;
    [SerializeField] private CarController car;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!carPointer) {
            carPointer = GameObject.FindGameObjectWithTag("CarPointer").GetComponent<CarPointer>();
            Debug.Log("Car pointer component unassigned! Reassigned.");
        }

        if (!car) {
            car = GetComponentInParent<CarController>();
            Debug.Log("Car component unassigned! Reassigned.");
        }
    }

    public void SetGPS() {
        if (car.currentRideNum < car.totalRideNum) {
            carPointer.StartDrive(destinationObject);
            StartCoroutine(gps.EndDollyMovement());
        }
    }
}
