using System.Collections;
using UnityEngine;

public class Destination : MonoBehaviour
{
    private CarController car;

    private void OnTriggerEnter(Collider collider) {
        
        if (collider.CompareTag("Car")) {
            Debug.Log("Arrived at destination!");

            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {
                car = script;
                car.currentStop = gameObject;
                car.arrived = true; // TODO: only set this when trip summary is over and it's time to get back to driving

                if (car.currentPassenger){
                    DropOffPassenger();
                }

            } else {
                Debug.LogWarning("Could not find CarController script on car!");
            }

            // PUT TRIP SUMMARY CODE HERE? MAYBE?
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

    private void DropOffPassenger() {
        car.dialogueManager.StartDialogue(car.currentPassenger.archetype.dropoffSalute, true);
    }
}
