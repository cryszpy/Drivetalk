using UnityEngine;

public class Destination : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Car")) {
            Debug.Log("Arrived at destination!");
        }
    }
}
