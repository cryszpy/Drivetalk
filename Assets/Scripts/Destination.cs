using System.Collections;
using UnityEngine;

public class Destination : MonoBehaviour
{
    
    [Tooltip("Reference to the car's script component.")]
    private CarController car;

    [Tooltip("Boolean flag; Checks if the car is inside this radius.")]
    [SerializeField] private bool inRadius = false;

    private void Start() {

        if (!car) {
            car = GameObject.FindGameObjectWithTag("CarFrame").transform.parent.GetComponent<CarController>();
            Debug.LogWarning("CarController unassigned on Destination, reassigned!");
        }
    }

    private void Update() {

        //Debug.Log(car + " | " + inRadius + " | " + !car.arrivedAtDest + " | " + (car.carPointer.destinationObject == this.transform.parent.parent.parent.gameObject));

        // If the car reference is not null, and the car is inside this destination—
        if (car && inRadius && !car.arrivedAtDest && car.carPointer.destinationObject == this.transform.parent.parent.parent.gameObject) {

            // Stop looping of this function
            car.arrivedAtDest = true;

            // If the car is carrying a passenger—
            if (car.currentPassenger) {

                //GameStateManager.dialogueManager.ResetDialogue();
                //Destroy(GameStateManager.dialogueManager.currentElement);

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
        }
    }
}
