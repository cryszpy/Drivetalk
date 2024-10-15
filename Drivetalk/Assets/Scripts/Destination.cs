using UnityEngine;

public class Destination : MonoBehaviour
{
    private CarController car;

    private void OnTriggerEnter(Collider collider) {
        
        if (collider.CompareTag("Car")) {
            Debug.Log("Arrived at destination!");

            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {
                car = script;
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

    private void DropOffPassenger() {
        car.currentPassenger.transform.parent = null;
        car.currentPassenger.transform.position = this.transform.position;

        car.currentPassenger = null;
    }
}
