using System.Collections;
using UnityEngine;

public class Destination : MonoBehaviour
{
    [Tooltip("Reference to the car's script component.")]
    private CarController car;

    [Tooltip("Boolean flag; Checks if the car is inside this radius.")]
    [SerializeField] private bool inRadius = false;

    private bool droppedOff = false;

    private void Update() {

        // If the car reference is not null, and the car is inside this destination—
        if (car && inRadius && !droppedOff) {

            // Stop looping of this function
            droppedOff = true;

            // If this destination is the car's intended destination, dialogue has finished, and the passenger exists—
            if (car.carPointer.destinationObject == transform.parent.gameObject && car.carPointer.finishedDialogue && car.currentPassenger) {

                GameStateManager.EOnPassengerDropoff?.Invoke();
            }
        }
    }

    // This function runs on contact with colliders
    private void OnTriggerEnter(Collider collider) {
        
        // If the car has been collided with—
        if (collider.CompareTag("CarFrame")) {

            if (!car) {
                // If the car's script can be accessed
                if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {

                    // Set the car's script reference to the script pulled from collision
                    car = script;

                } else {
                    Debug.LogWarning("Could not find CarController script on car!");
                }
            }

            inRadius = true;
        }
    }

    // This function runs when a collider stops colliding with this
    private void OnTriggerExit(Collider collider) {

        if (collider.CompareTag("CarFrame")) {
            inRadius = false;
            droppedOff = false;
        }
    }
}
