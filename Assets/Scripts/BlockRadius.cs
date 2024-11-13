using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRadius : MonoBehaviour
{
    [Tooltip("Reference to the car's script component.")]
    private CarPointer carPointer;

    private void Start() {
        carPointer = GameObject.FindGameObjectWithTag("CarPointer").GetComponent<CarPointer>();
    }

    // This function runs on contact with colliders
    private void OnTriggerEnter(Collider collider) {

        if (collider.CompareTag("DestinationRadius")) {
            if (collider.gameObject.TryGetComponent<DestinationRadius>(out var script)) {
                script.blocksList.Add(gameObject);
            } else {
                Debug.LogError("Could not find DestinationRadius script on destination object!");
            }
        }
        
        // If the car pointer has been collided with—
        if (collider.CompareTag("CarFrame")) {

            /* // If the car pointer's script can be accessed
            if (collider.transform.parent.TryGetComponent<CarPointer>(out var script)) {

                // Set the car pointer's script reference to the script pulled from collision
                carPointer = script;

                destinationRadius = carPointer.destinationObject.GetComponentInChildren<DestinationRadius>();

            } else {
                Debug.LogWarning("Could not find CarController script on car!");
                return;
            } */

            if (!carPointer.finishedDialogue && carPointer.destinationObject != null && carPointer.destinationObject == gameObject) {
                Debug.Log("Reached temporary block destination!");

                carPointer.savedBlock = gameObject;

                GetRandomBlock();
            }
        }
    }

    private void GetRandomBlock() {

        int rand = UnityEngine.Random.Range(0, carPointer.destinationRadius.blocksList.Count);

        GameObject block = carPointer.destinationRadius.blocksList[rand];

        if (block != carPointer.savedBlock && !carPointer.currentBlocksList.Contains(block)) {
            carPointer.StartDrive(block);
        } else {
            GetRandomBlock();
        }
    }
}
