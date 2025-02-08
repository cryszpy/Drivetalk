using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Corner : Road
{
    private CarPointer carPointer;

    private void OnTriggerEnter(Collider collider) {
        
        // If the car pointer has been collided with—
        if (collider.CompareTag("CarFrame")) {

            // If the car pointer's script can be accessed
            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {

                // Set the car pointer's script reference to the script pulled from collision
                carPointer = script.carPointer;

                if (carPointer.wheel) {
                    StartCoroutine(carPointer.wheel.TurnWheel(carPointer.currentSteeringDirection));
                }

                /* if (carPointer.trackedIntersection == gameObject) {
                    carPointer.trackedIntersection = null;
                } */

            } else {
                Debug.LogWarning("Could not find CarPointer script on car pointer!");
                return;
            }
        }
    }

    private void OnTriggerExit(Collider collider) {
        
        if (collider.CompareTag("CarFrame")) {

            if (carPointer) {

                if (carPointer.wheel) {
                    StartCoroutine(carPointer.wheel.TurnWheel(SteeringDirection.FORWARD));
                }

                carPointer.calculatedDirections = false;

                // Recalculate GPS route in case player missed a turn
                /* if (carPointer.destinationObject) {
                    carPointer.SetGPSPath(carPointer.destinationObject);
                } */
            }
        }
    }
}

