using System.Collections;
using UnityEngine;

public class Destination : MonoBehaviour
{
    [Tooltip("This destination's ID number.")]
    public int id;
    
    [Tooltip("Reference to the car's script component.")]
    private CarController car;

    [Tooltip("Boolean flag; Checks if the car is inside this radius.")]
    [SerializeField] private bool inRadius = false;

    private void Update() {

        // If the car reference is not null, and the car is inside this destination—
        if (car && inRadius && !car.arrivedAtDest && car.carPointer.destinationObject == this.transform.parent.gameObject) {

            // Stop looping of this function
            car.arrivedAtDest = true;

            // If this destination is the car's intended destination, dialogue has finished, and the passenger exists—
            if (car.currentPassenger) {

                GameStateManager.dialogueManager.ResetDialogue();
                Destroy(GameStateManager.dialogueManager.currentElement);
                Debug.Log("Destroyed");

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
            Debug.Log(car.arrivedAtDest);
            
            if (car.arrivedAtDest) {
                Debug.Log("did it");
                car.arrivedAtDest = false;

                // Re-raycast for directions
                car.carPointer.GetValidDirections();

                // Reset steering direction to forward if possible
                if (car.carPointer.validDirections.Contains(SteeringDirection.FORWARD)) {
                    car.carPointer.currentSteeringDirection = SteeringDirection.FORWARD;
                    car.carPointer.turnSignal.hovered = false;
                    car.carPointer.turnSignal.dragging = false;
                    StartCoroutine(car.carPointer.turnSignal.SignalClick(car.carPointer.currentSteeringDirection));
                }
            }
        }
    }
}
