using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSignal : UIElementSlider
{
    [SerializeField] private Collider coll;

    [SerializeField] private float cutoffLeft;
    [SerializeField] private float cutoffRight;
    [SerializeField] private float straightCutoffLeft;
    [SerializeField] private float straightCutoffRight;

    [SerializeField] private GameObject leftSignal;
    [SerializeField] private GameObject rightSignal;

    public override void Update()
    {
        /* base.Update();

        // If car is not in intersection or at a taxi stop (is driving), then allow player to use turn signal
        if (!carPointer.inIntersection && !carPointer.car.atTaxiStop) {
            coll.enabled = true;
        } 
        // If car is at an intersection or a taxi stop, AND the collider is enabled, select a steering direction
        else if (coll.enabled == true) {
            coll.enabled = false;
            StartCoroutine(SignalClick(carPointer.currentSteeringDirection));
        } */
    }

    public override void Drag()
    {
        // Check whether the player has stopped dragging slider
        /* if (Input.GetMouseButtonUp(0)) {
            dragging = false;
            StartCoroutine(SignalClick(carPointer.currentSteeringDirection));
        }

        // Calculates the difference between the current mouse's position and mouse's previous position
        mousePosDelta = Input.mousePosition - mousePreviousPos;

        // Get the mouse's difference in position applied to the slider's desired rotation axis
        rotation = Vector3.Dot(mousePosDelta, new Vector3(0, 1, 0));

        float currentRot = dialObject.transform.localEulerAngles.z;
        float newRot = currentRot + rotation;

        // Limit slider rotation to be between a certain minimum and maximum degree angle
        newRot = Mathf.Clamp(newRot, rotationMin, rotationMax);

        // Apply change in rotation based on mouse cursor movement
        dialObject.transform.localEulerAngles = new Vector3(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, newRot);
        
        if (!carPointer.inIntersection && !carPointer.car.atTaxiStop && carPointer.car.currentPassenger) {

            // NONE
            if (newRot > cutoffLeft && newRot < cutoffRight) {
                if (carPointer.currentSteeringDirection != SteeringDirection.FORWARD && carPointer.validDirections.Contains(SteeringDirection.FORWARD)) {
                    carPointer.currentSteeringDirection = SteeringDirection.FORWARD;

                    leftSignal.SetActive(false);
                    rightSignal.SetActive(false);

                    //carPointer.SwitchDirection();
                    Debug.Log("Signaled none!");
                }
            }
            // LEFT
            else if (newRot < cutoffLeft) {
                if (carPointer.currentSteeringDirection != SteeringDirection.LEFT && carPointer.validDirections.Contains(SteeringDirection.LEFT)) {
                    carPointer.currentSteeringDirection = SteeringDirection.LEFT;

                    leftSignal.SetActive(true);
                    rightSignal.SetActive(false);

                    //carPointer.SwitchDirection();
                    Debug.Log("Signaled left!");
                }
            } 
            // RIGHT
            else if (newRot > cutoffRight) {
                if (carPointer.currentSteeringDirection != SteeringDirection.RIGHT && carPointer.validDirections.Contains(SteeringDirection.RIGHT)) {
                    carPointer.currentSteeringDirection = SteeringDirection.RIGHT;

                    leftSignal.SetActive(false);
                    rightSignal.SetActive(true);

                    //carPointer.SwitchDirection();
                    Debug.Log("Signaled right!");
                }
            }

            // Physically move turn signal handle
            StartCoroutine(SignalClick(carPointer.currentSteeringDirection));
        } */
    }

    public IEnumerator SignalClick(SteeringDirection direction) {
        /* if (dragging) {
            yield return null;
        } */

        float zValue = dialObject.transform.localEulerAngles.z;

        switch (direction) {
            case SteeringDirection.LEFT:
                leftSignal.SetActive(true);
                rightSignal.SetActive(false);

                if (zValue < 5) {
                    while (zValue < 5) {
                        zValue += 1;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, zValue);

                        yield return new WaitForSeconds(0.01f);
                    }
                } else if (zValue > 5) {
                    while (zValue > 5) {
                        zValue -= 1f;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, zValue);

                        yield return new WaitForSeconds(0f);
                    }
                }
                break;
            case SteeringDirection.RIGHT:
                leftSignal.SetActive(false);
                rightSignal.SetActive(true);

                if (zValue > 80) {
                    while (zValue > 80) {
                        zValue -= 1;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, zValue);

                        yield return new WaitForSeconds(0.01f);
                    }
                } else if (zValue < 80) {
                    while (zValue < 80) {
                        zValue += 1f;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, zValue);

                        yield return new WaitForSeconds(0f);
                    }
                }
                break;
            case SteeringDirection.FORWARD:

                // Reset turn signals
                leftSignal.SetActive(false);
                rightSignal.SetActive(false);
            
                if (zValue > 45) {
                    while (zValue > 45) {
                        zValue -= 1f;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, zValue);

                        yield return new WaitForSeconds(0f);
                    }
                } else if (zValue < 45) {
                    while (zValue < 45) {
                        zValue += 1f;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, zValue);

                        yield return new WaitForSeconds(0f);
                    }
                }
                break;
        }
    }
}
