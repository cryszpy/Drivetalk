using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarPointerRadius : MonoBehaviour
{
    [Tooltip("Reference to the car pointer's script component.")]
    [SerializeField] private CarPointer carPointer;

    // This function runs on contact with colliders
    private void OnTriggerEnter (Collider collider) {

        if (collider.gameObject.CompareTag("Block") && !carPointer.currentBlocksList.Contains(collider.gameObject)) {
            carPointer.currentBlocksList.Add(collider.gameObject);
        }
    }

    private void OnTriggerExit (Collider collider) {
        
        if (collider.gameObject.CompareTag("Block") && carPointer.currentBlocksList.Contains(collider.gameObject)) {
            carPointer.currentBlocksList.Remove(collider.gameObject);
        }
    }
}
