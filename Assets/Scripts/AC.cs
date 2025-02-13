using System.Collections;
using TMPro;
using UnityEngine;

public enum ACSetting {
    OFF, COOL, WARM
}

public class AC : UIElementSlider
{

    [SerializeField] private float cutoffCool;
    [SerializeField] private float cutoffWarm;
    [SerializeField] private float zeroMin;
    [SerializeField] private float zeroMax;

    [SerializeField] private float maxRotSpeed;
    [SerializeField] private float rotSpeed;

    public override void Start() {
        base.Start();
        CarController.Temperature = 0.5f;
    }

    public override void Update()
    {
        base.Update();

        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {
            // Update the car's temperature statistic to use in passenger happiness calculations
            float oldRange = rotationMax - rotationMin;
            float newRange = 1 - 0;
            CarController.Temperature = (((dialObject.transform.localEulerAngles.y - rotationMin) * newRange) / oldRange) + 0;
        }
    }

    public override void Drag()
    {
        // Check whether the player has stopped dragging slider
        /* if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }

        mousePosDelta = Input.mousePosition - mousePreviousPos;

        // Get the mouse's difference in position applied to the slider's desired rotation axis
        rotation = Vector3.Dot(mousePosDelta, Camera.main.transform.up);

        float currentRot = transform.localEulerAngles.y;
        float newRot = currentRot + rotation;

        // Limit slider rotation to be between a certain minimum and maximum degree angle
        newRot = Mathf.Clamp(newRot, rotationMin, rotationMax);

        // Apply change in rotation based on mouse cursor movement
        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, newRot, dialObject.transform.localEulerAngles.z); */

        // OFF
        /* if (newRot > zeroMin && newRot < zeroMax) {
            dialObject.transform.localEulerAngles = new(transform.localEulerAngles.x, 90, transform.localEulerAngles.z);
            coolText.font = defMaterial;
            warmText.font = defMaterial;
            //CarController.Temperature = ACSetting.OFF;
        } 
        // COOL
        else if (newRot < cutoffCool) {
            dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, rotationMin + 1, dialObject.transform.localEulerAngles.z);
            coolText.font = coolMaterial;
            warmText.font = defMaterial;
            //CarController.Temperature = ACSetting.COOL;
        } 
        // WARM
        else if (newRot > cutoffWarm) {
            dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, rotationMax - 1, dialObject.transform.localEulerAngles.z);
            warmText.font = warmMaterial;
            coolText.font = defMaterial;
            //CarController.Temperature = ACSetting.WARM;
        } */

        base.Drag();
    }
}
