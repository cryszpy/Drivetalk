using System.Collections;
using TMPro;
using UnityEngine;

public class Wheel : UIElementSlider
{

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
        
        // COOL
        if (newRot > straightCutoffLeft && newRot < straightCutoffRight) {
            Debug.Log("Wheel turned straight!");
        }
        else if (newRot < cutoffLeft) {
            Debug.Log("Wheel turned left!");
        } 
        // WARM
        else if (newRot > cutoffRight) {
            Debug.Log("Wheel turned right!");
        }
    }
}
