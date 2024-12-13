using System.Collections;
using TMPro;
using UnityEngine;

public class Wheel : UIElementSlider
{
    [SerializeField] private Collider coll;

    [SerializeField] private float cutoffLeft;
    [SerializeField] private float cutoffRight;
    [SerializeField] private float straightCutoffLeft;
    [SerializeField] private float straightCutoffRight;

    /* public override void Update()
    {
        base.Update();

        if (insideMinigame) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
                insideMinigame = false;
                Debug.Log("LANDED");

                StartCoroutine(EndDollyMovement());
            }

            if (rotSpeed < maxRotSpeed) {
                rotSpeed++;
            }

            dialObject.transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
        }
    } */

    /* public override void Update() {
        base.Update();

        if (!carPointer.inIntersection && !carPointer.car.atTaxiStop) {
            coll.enabled = true;
        } else if (coll.enabled == true) {
            coll.enabled = false;
            StartCoroutine(TurnWheel(carPointer.currentSteeringDirection));
        }
    } */

    public override void Drag()
    {
        // Check whether the player has stopped dragging slider
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }

        // Calculates the difference between the current mouse's position and mouse's previous position
        mousePosDelta = Input.mousePosition - mousePreviousPos;

        // Get the mouse's difference in position applied to the slider's desired rotation axis
        rotation = Vector3.Dot(mousePosDelta, new Vector3(1, 0, 0));

        float currentRot = dialObject.transform.localEulerAngles.y;
        float newRot = currentRot + rotation;

        // Limit slider rotation to be between a certain minimum and maximum degree angle
        newRot = Mathf.Clamp(newRot, rotationMin, rotationMax);

        // Apply change in rotation based on mouse cursor movement
        dialObject.transform.localEulerAngles = new Vector3(dialObject.transform.localEulerAngles.x, newRot, dialObject.transform.localEulerAngles.z);
        
        if (!carPointer.inIntersection && !carPointer.car.atTaxiStop && carPointer.car.currentPassenger) {

            // FORWARD
            if (newRot > straightCutoffLeft && newRot < straightCutoffRight) {
                if (carPointer.currentSteeringDirection != SteeringDirection.FORWARD && carPointer.validDirections.Contains(SteeringDirection.FORWARD)) {
                    carPointer.currentSteeringDirection = SteeringDirection.FORWARD;
                    carPointer.SwitchDirection();
                    Debug.Log("Wheel turned straight!");
                }
            }
            // LEFT
            else if (newRot < cutoffLeft) {
                if (carPointer.currentSteeringDirection != SteeringDirection.LEFT && carPointer.validDirections.Contains(SteeringDirection.LEFT)) {
                    carPointer.currentSteeringDirection = SteeringDirection.LEFT;
                    carPointer.SwitchDirection();
                    Debug.Log("Wheel turned left!");
                }
            } 
            // RIGHT
            else if (newRot > cutoffRight) {
                if (carPointer.currentSteeringDirection != SteeringDirection.RIGHT && carPointer.validDirections.Contains(SteeringDirection.RIGHT)) {
                    carPointer.currentSteeringDirection = SteeringDirection.RIGHT;
                    carPointer.SwitchDirection();
                    Debug.Log("Wheel turned right!");
                }
            }
        }
    }

    public IEnumerator TurnWheel(SteeringDirection direction) {

        float yValue = dialObject.transform.localEulerAngles.y;

        switch (direction) {
            case SteeringDirection.LEFT:
                if (yValue < 5) {
                    while (yValue < 5) {
                        yValue += 1;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, yValue, dialObject.transform.localEulerAngles.z);

                        yield return new WaitForSeconds(0.01f);
                    }
                } else if (yValue > 5) {
                    while (yValue > 5) {
                        yValue -= 1;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, yValue, dialObject.transform.localEulerAngles.z);

                        yield return new WaitForSeconds(0.01f);
                    }
                }
                break;
            case SteeringDirection.RIGHT:
                if (yValue > 175) {
                    while (yValue > 175) {
                        yValue -= 1;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, yValue, dialObject.transform.localEulerAngles.z);

                        yield return new WaitForSeconds(0.01f);
                    }
                } else if (yValue < 175) {
                    while (yValue < 175) {
                        yValue += 1;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, yValue, dialObject.transform.localEulerAngles.z);

                        yield return new WaitForSeconds(0.01f);
                    }
                }
                break;
            case SteeringDirection.FORWARD:
                if (yValue > 90) {
                    while (yValue > 90) {
                        yValue -= 3;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, yValue, dialObject.transform.localEulerAngles.z);

                        yield return new WaitForSeconds(0.001f);
                    }
                } else if (yValue < 90) {
                    while (yValue < 90) {
                        yValue += 1;
                        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, yValue, dialObject.transform.localEulerAngles.z);

                        yield return new WaitForSeconds(0.01f);
                    }
                }
                break;
        }
    }
}
