using System.Collections;
using UnityEngine;

public class Destination : MonoBehaviour
{
    [Tooltip("Reference to the car's script component.")]
    private CarController car;

    // This function runs on contact with colliders
    private void OnTriggerEnter(Collider collider) {
        
        // If the car has been collided with—
        if (collider.CompareTag("CarFrame")) {
            Debug.Log("Arrived at destination!");

            // If the car's script can be accessed
            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {

                // Set the car's script reference to the script pulled from collision
                car = script;

                if (car.currentPassenger == null && !car.carPointer.finishedDialogue) {
                    car.arrived = true;
                }

            } else {
                Debug.LogWarning("Could not find CarController script on car!");
            }

            // PUT TRIP SUMMARY CODE HERE? MAYBE?
        }
    }

    private void OnTriggerStay (Collider collider) {

        // If the car has been collided with—
        if (collider.CompareTag("CarFrame") && !car.dialogueManager.triggerDropoff) {

            if (car) {
                if (car.carPointer.finishedDialogue && car.currentPassenger) {

                    car.dialogueManager.triggerDropoff = true;

                    car.carPointer.destinationRadius.setBlock = false;

                    // Set the car's current stop to this stop
                    car.currentStop = gameObject;

                    car.arrived = true; // TODO: only set this when trip summary is over and it's time to get back to driving

                    // If the car has a passenger—
                    if (car.currentPassenger){

                        // Drops the current passenger off
                        DropOffPassenger();
                    }
                }
            }
        }
    }

    /* private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Car")) {
            Debug.Log("Leaving destination!");

            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {
                car.currentStop = null;

            } else {
                Debug.LogWarning("Could not find CarController script on car!");
            }
        }
    } */

    // Starts the passenger's dropoff dialogue and initiates dropoff sequence
    private void DropOffPassenger() {
        car.dialogueManager.StartDialogue(car.currentPassenger.archetype.dropoffSalute);
    }
}
