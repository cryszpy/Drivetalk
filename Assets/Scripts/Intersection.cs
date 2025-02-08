using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Intersection : Road
{
    private CarPointer carPointer;

    private void OnTriggerEnter(Collider collider) {
        
        // If the car pointer has been collided with—
        if (collider.CompareTag("CarFrame")) {

            // If the car pointer's script can be accessed
            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {

                // Set the car pointer's script reference to the script pulled from collision
                carPointer = script.carPointer;
                carPointer.inIntersection = true;

                /* if (carPointer.trackedIntersection == gameObject) {
                    carPointer.trackedIntersection = null;
                } */

                // Spawns new procedural road tile
                carPointer.SpawnRoadTile();

                // Set appropriate turn signal and wheel rotation only if at the appropriate intersection
                if (carPointer.wheel && carPointer.turnSignal && carPointer.roadQueue.First().center.transform.parent == transform) {
                    StartCoroutine(carPointer.turnSignal.SignalClick(carPointer.currentSteeringDirection));
                    StartCoroutine(carPointer.wheel.TurnWheel(carPointer.currentSteeringDirection));
                }

            } else {
                Debug.LogWarning("Could not find CarPointer script on car pointer!");
                return;
            }
        }
    }

    private void OnTriggerExit(Collider collider) {
        
        if (collider.CompareTag("CarFrame")) {

            if (carPointer) {

                // Reset turn signal and wheel rotation
                if (carPointer.wheel && carPointer.turnSignal) {
                    StartCoroutine(carPointer.turnSignal.SignalClick(SteeringDirection.FORWARD));
                    StartCoroutine(carPointer.wheel.TurnWheel(SteeringDirection.FORWARD));
                }

                carPointer.inIntersection = false;
                //carPointer.calculatedDirections = false;

                // Recalculate GPS route in case player missed a turn unless car doesn't have a passenger
                /* if (carPointer.destinationObject && carPointer.car.currentPassenger) {
                    carPointer.SetGPSPath(carPointer.destinationObject);
                } */
            }
        }
    }
}

