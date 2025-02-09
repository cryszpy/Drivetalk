using System.Collections;
using UnityEngine;

public class GPSDestination : MonoBehaviour
{
    [Tooltip("The destination tile linked to this button.")]
    public GameObject destinationObject;

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

        gps.dragging = false;
        gps.gameObject.layer = gps.regularLayer;

        GameStateManager.dialogueManager.waitForRouting = false; // Set this when correct destination is picked

        // Sets car's boolean flag to no longer be at a taxi stop
        car.atTaxiStop = false;

        // If the car still has rides left in the day—
        if (car.currentRideNum < car.totalRideNum) {

            // Sets the passenger destination object
            carPointer.currentDestinationTile = destinationObject;

            // If already inside destination radius, allow rerouting
            if (carPointer.destinationRadius) {
                carPointer.setInitialBlock = false;
            }

            // Start waiting until the passenger talks
            GameStateManager.EOnDestinationSet?.Invoke();
        }
    }
}