using UnityEngine;

public enum ACSetting {
    OFF, COOL, WARM
}

public class AC : UIElementSlider
{

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
            CarController.Temperature = (((dialObject.transform.localPosition.x - rotationMin) * newRange) / oldRange) + 0;
        }
    }

    public override void Drag()
    {
        // Check whether the player has stopped dragging slider
        if (Input.GetMouseButtonUp(0)) {
            if (carPointer.hoveredButton == gameObject) {
                carPointer.hoveredButton = null;
                hovered = false;
                dragging = false;
            }
        }

        if (dialObject) {
            
            // Calculates the difference between the current mouse's position and mouse's previous position
            /* mousePosDelta = Input.mousePosition - mousePreviousPos;

            // Get the mouse's difference in position applied to the slider's desired rotation axis
            change = Vector3.Dot(mousePosDelta, new Vector3(1, 0, 0));

            float currentPos = dialObject.transform.localPosition.x;
            float newPos = currentPos + change; */

            // Limit slider rotation to be between a certain minimum and maximum degree angle
            float newPos = Mathf.Clamp(mainCamera.ScreenToViewportPoint(Input.mousePosition).x - 0.655f, rotationMin, rotationMax); //Mathf.Clamp(newPos, rotationMin, rotationMax);

            // Apply change in rotation based on mouse cursor movement
            dialObject.transform.localPosition = new Vector3(newPos, dialObject.transform.localPosition.y, dialObject.transform.localPosition.z);
        }
    }
}