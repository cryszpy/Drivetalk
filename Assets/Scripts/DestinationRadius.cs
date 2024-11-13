using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationRadius : MonoBehaviour
{
    [Tooltip("Reference to the car's script component.")]
    [SerializeField] private CarPointer carPointer;

    [Tooltip("Reference to the car script.")]
    [SerializeField] private CarController car;

    [Tooltip("List of all block markers within this radius. SET DYNAMICALLY")]
    public List<GameObject> blocksList = new();

    public bool setBlock = false;

    // This function runs on contact with colliders
    private void OnTriggerEnter(Collider collider) {

        /* if (collider.CompareTag("Block") && !blocksList.Contains(collider.gameObject)) {
            Debug.Log(collider.gameObject.name);
            blocksList.Add(collider.gameObject);

            if (collider.gameObject.TryGetComponent<BlockRadius>(out var script)) {
                Debug.Log("bro");
                script.destinationRadius = this;
            } else {
                Debug.LogError("Could not find BlockRadius script component on this Block!");
            }
        } */
        
        // If the car pointer has been collided with—
        if (collider.CompareTag("CarFrame")) {

            // If the car pointer's script can be accessed
            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {

                car = script;

                // Set the car pointer's script reference to the script pulled from collision
                carPointer = script.carPointer;

            } else {
                Debug.LogWarning("Could not find CarController script on car!");
                return;
            }
        }
    }

    private void OnTriggerStay(Collider collider) {

        // If the car pointer has been collided with—
        if (collider.CompareTag("CarFrame") && car && carPointer) {

            if (!carPointer.finishedDialogue && car.currentPassenger != null && !setBlock && !car.atTaxiStop && carPointer.destinationRadius == this) {

                setBlock = true;

                Debug.Log("Dialogue not finished, rerouted car!");

                if (carPointer.destinationObject.CompareTag("Destination")) {
                    carPointer.savedDestination = carPointer.destinationObject;
                }

                GetRandomBlock();
            }
        }
    }

    private void GetRandomBlock() {

        int rand = UnityEngine.Random.Range(0, blocksList.Count);

        GameObject block = blocksList[rand];

        if (!carPointer.currentBlocksList.Contains(block)) {
            carPointer.StartDrive(block);
            carPointer.savedBlock = blocksList[rand];
        } else {
            GetRandomBlock();
        }
    }
}
