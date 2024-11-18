using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationRadius : MonoBehaviour
{
    [Tooltip("Reference to the car's script component.")]
    [SerializeField] private CarPointer carPointer;

    [Tooltip("Reference to the car script.")]
    [SerializeField] private CarController car;

    [Tooltip("List of all block markers within this radius. SET DYNAMICALLY")]
    public List<GameObject> blocksList = new();

    [Tooltip("Boolean flag; Checks whether the car is inside this radius.")]
    public bool inDestinationRadius = false; 

    private void Update() {

        // If the car reference is assigned, and the car is inside this radius—
        if (car && inDestinationRadius) {

            // If the ride hasn't finished dialogue, the car has a passenger, the car is not awaiting
            // a passenger, this radius belongs to the car's current destination, and the car hasn't been rerouted yet, then reroute
            // the car to the nearest block marker
            if (!car.carPointer.finishedDialogue && car.currentPassenger && !carPointer.setInitialBlock && !car.atTaxiStop && carPointer.destinationRadius.gameObject == gameObject) {
                
                carPointer.setInitialBlock = true;

                Debug.Log("Dialogue not finished, rerouted car!");

                // Save the car's final destination before rerouting
                if (carPointer.destinationObject.CompareTag("Destination")) {
                    carPointer.savedDestination = carPointer.destinationObject;
                }

                // Reroute the car
                GetRandomBlock();
            }
        }
    }

    // This function runs on contact with colliders
    private void OnTriggerEnter(Collider collider) {
        
        // If the car pointer has been collided with—
        if (collider.CompareTag("CarFrame")) {

            // If the car pointer's script can be accessed
            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {

                car = script;

                // Set the car pointer's script reference to the script pulled from collision
                carPointer = script.carPointer;

            } else {
                Debug.LogWarning("Could not find CarController script on car!");
                return;
            }

            inDestinationRadius = true;
        }
    }

    // This function runs when a collider stops colliding with this
    private void OnTriggerExit (Collider collider) {

        // If the car leaves the radius, set the boolean flag to false
        if (collider.CompareTag("CarFrame")) {
            inDestinationRadius = false;
        }
    }

    private void GetRandomBlock() {

        int rand = UnityEngine.Random.Range(0, blocksList.Count);

        GameObject block = blocksList[rand];

        if (!carPointer.currentBlocksList.Contains(block)) {
            carPointer.StartDrive(block);
            carPointer.savedBlock = block;
        } else {
            GetRandomBlock();
        }
    }
}
