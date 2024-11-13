using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPointerRadius : MonoBehaviour
{
    [Tooltip("Reference to the car pointer's script component.")]
    [SerializeField] private CarPointer carPointer;

    // This function runs on contact with colliders
    private void OnTriggerEnter (Collider collider) {

        if (collider.CompareTag("Block")) {
            carPointer.currentBlocksList.Add(collider.gameObject);
        }
    }

    private void OnTriggerExit (Collider collider) {
        
        if (collider.CompareTag("Block")) {
            carPointer.currentBlocksList.Remove(collider.gameObject);
        }
    }
}
