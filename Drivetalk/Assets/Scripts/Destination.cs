using UnityEngine;

public class Destination : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider) {
        
        if (collider.CompareTag("Car")) {
            Debug.Log("Arrived at destination!");

            // PUT TRIP SUMMARY CODE HERE? MAYBE?

            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {
                script.arrived = true; // TODO: only set this when trip summary is over and it's time to get back to driving
            }
        }
    }
}
