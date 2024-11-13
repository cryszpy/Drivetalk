using UnityEngine;

public class GPSDestination : MonoBehaviour
{

    [Tooltip("Reference to this map location's real-city destination.")]
    [SerializeField] private GameObject destinationObject;

    [Tooltip("Reference to the main map script.")]
    [SerializeField] private GPS gps;

    [Tooltip("Reference to the car pointer script.")]
    [SerializeField] private CarPointer carPointer;

    [Tooltip("Reference to the car's script.")]
    [SerializeField] private CarController car;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assigns any missing script references
        if (!carPointer) {
            carPointer = GameObject.FindGameObjectWithTag("CarPointer").GetComponent<CarPointer>();
            Debug.Log("Car pointer component unassigned! Reassigned.");
        }

        if (!car) {
            car = GetComponentInParent<CarController>();
            Debug.Log("Car component unassigned! Reassigned.");
        }
    }

    // Sets the map upon clicking a map location
    public void SetGPS() {

        // If the car still has rides left in the day—
        if (car.currentRideNum < car.totalRideNum) {

            // If already inside destination radius, allow rerouting
            if (carPointer.destinationRadius) {
                carPointer.destinationRadius.setBlock = false;
            }

            // Routes the car pointer (and thus the car) to the selected destination
            carPointer.StartDrive(destinationObject);

            // Moves the camera back out of map view
            StartCoroutine(gps.EndDollyMovement());
        }
    }
}
