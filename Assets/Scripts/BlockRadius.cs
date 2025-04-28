using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRadius : MonoBehaviour
{
    [Tooltip("Reference to the car's script component.")]
    private CarController car;

    [Tooltip("Reference to the car pointer script.")]
    private CarPointer carPointer;

    [Tooltip("Boolean flag; Checks whether the car is in this radius.")]
    private bool inBlockRadius;

    private void Update() {

        // If car and pointer are assigned, and car is in this radius—
        if (car && carPointer && inBlockRadius) {

            // If ride has not finished dialogue, the car is routed to a destination, and that destination is this block—
            if (!GameStateManager.dialogueManager.stopDialogue && carPointer.pathfindingTarget != null && carPointer.pathfindingTarget == gameObject) {
                Debug.Log("Reached temporary block destination!");

                // Save this block as the car's previous block destination
                carPointer.savedBlock = gameObject;

                // Choose a new random block to drive to while dialogue is not finished
                GetRandomBlock();
            }
        }
    }

    // This function runs on contact with colliders
    private void OnTriggerEnter(Collider collider) {

        if (collider.CompareTag("DestinationRadius")) {
            if (collider.gameObject.TryGetComponent<DestinationRadius>(out var script)) {
                script.blocksList.Add(gameObject);
            } else {
                Debug.LogError("Could not find DestinationRadius script on destination object!");
            }

            //GameStateManager.EOnBlockRadiusDetection?.Invoke();
        }
        
        // If the car pointer has been collided with—
        if (collider.CompareTag("CarFrame")) {

            // If the car pointer's script can be accessed
            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {

                // Set the car pointer's script reference to the script pulled from collision
                car = script;

                carPointer = car.carPointer;

            } else {
                Debug.LogError("Could not find CarController script on car!");
                return;
            }

            inBlockRadius = true;
        }
    }

    // This function runs when a collider stops colliding with this
    private void OnTriggerExit (Collider collider) {

        // If the car leaves the radius, set the boolean flag to false
        if (collider.CompareTag("CarFrame")) {
            inBlockRadius = false;

            if (carPointer) {
                carPointer.currentBlocksList.Remove(gameObject);
                //carPointer.CurrentBlocksListRemove(gameObject);
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
