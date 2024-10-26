using System.Collections;
using TMPro;
using UnityEngine;

public enum ACSetting {
    OFF, COOL, WARM
}

public class AC : UIElementSlider
{
    [SerializeField] private GameObject dialObject;

    [SerializeField] private TMP_Text coolText;
    [SerializeField] private TMP_Text warmText;
    [SerializeField] private TMP_FontAsset defMaterial;
    [SerializeField] private TMP_FontAsset coolMaterial;
    [SerializeField] private TMP_FontAsset warmMaterial;

    [SerializeField] private float cutoffCool;
    [SerializeField] private float cutoffWarm;
    [SerializeField] private float zeroMin;
    [SerializeField] private float zeroMax;

    [SerializeField] private float maxRotSpeed;
    [SerializeField] private float rotSpeed;

    public override void Update()
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
    }

    public override void Drag()
    {
        // Check whether the player has stopped dragging slider
        if (Input.GetMouseButtonUp(0)) {
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
        transform.localEulerAngles = new(transform.localEulerAngles.x, newRot, transform.localEulerAngles.z);

        // OFF
        if (newRot > zeroMin && newRot < zeroMax) {
            dialObject.transform.localEulerAngles = new(transform.localEulerAngles.x, 90, transform.localEulerAngles.z);
            //coolText.font = defMaterial;
            //warmText.font = defMaterial;
            //CarController.Temperature = ACSetting.OFF;
        } 
        // COOL
        else if (newRot < cutoffCool) {
            dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, rotationMin + 1, dialObject.transform.localEulerAngles.z);
            //coolText.font = coolMaterial;
            //warmText.font = defMaterial;
            //CarController.Temperature = ACSetting.COOL;
        } 
        // WARM
        else if (newRot > cutoffWarm) {
            dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, rotationMax - 1, dialObject.transform.localEulerAngles.z);
            //warmText.font = warmMaterial;
            //coolText.font = defMaterial;
            //CarController.Temperature = ACSetting.WARM;
        }

        /* // Update the player's temperature stat
        float oldRange = rotationMax - rotationMin;
        float newRange = 1 - 0;
        CarController.Temperature = (((transform.localEulerAngles.y - rotationMin) * newRange) / oldRange) + 0; */
    }

    public override IEnumerator StartDollyMovement() {
        while (splineDolly.CameraPosition < 1 || cinemachineCam.Lens.FieldOfView > 30) {
            if (cinemachineCam.Lens.FieldOfView > 30) {
                toonCamera.fieldOfView -= 0.6f;
                cinemachineCam.Lens.FieldOfView -= 0.6f;
            }
            if (splineDolly.CameraPosition < 1f) {
                if (splineDolly.CameraPosition > 0.7f){
                    splineDolly.CameraPosition += 0.01f;
                } else {
                    splineDolly.CameraPosition += 0.02f;
                }
            }
            yield return new WaitForSeconds(0.005f);
        }
        screenUI.SetActive(true);

        dialObject.transform.localEulerAngles = new(dialObject.transform.localEulerAngles.x, rotationMin, dialObject.transform.localEulerAngles.z);
        insideMinigame = true;
    }

    public override IEnumerator EndDollyMovement() {
        insideMinigame = false;
        while (splineDolly.CameraPosition > 0 || cinemachineCam.Lens.FieldOfView < 60) {
            if (cinemachineCam.Lens.FieldOfView < 60) {
                toonCamera.fieldOfView += 0.6f;
                cinemachineCam.Lens.FieldOfView += 0.6f;
            }
            if (splineDolly.CameraPosition > 0) {
                if (splineDolly.CameraPosition < 0.3f){
                    splineDolly.CameraPosition -= 0.01f;
                } else {
                    splineDolly.CameraPosition -= 0.02f;
                }
            }
            yield return new WaitForSeconds(0.005f);
        }
        cinemachineCam.LookAt = cameraLookAt.transform;
        GameStateManager.SetState(GAMESTATE.PLAYING);
        screenUI.SetActive(false);
    }
}
