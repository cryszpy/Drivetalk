using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class GPS : UIElementButton
{

    [Tooltip("Reference to the object to look at when the GPS is clicked.")]
    [SerializeField] protected GameObject focusOn;

    [Tooltip("Reference to the main Cinemachine camera.")]
    [SerializeField] protected CinemachineCamera cinemachineCam;

    [Tooltip("Reference to the dolly spline component attached to the Cinemachine camera.")]
    [SerializeField] protected CinemachineSplineDolly splineDolly;

    [Tooltip("Reference to the spline dolly track to switch the camera to when the GPS is clicked.")]
    [SerializeField] protected SplineContainer spline;

    [Tooltip("Reference to the map UI.")]
    [SerializeField] protected GameObject screenUI;

    [Tooltip("Reference to the dialogue continue button.")]
    public GameObject continueButton;

    [Tooltip("Reference to the camera focal point object.")]
    [SerializeField] protected GameObject cameraLookAt;

    public override void Update() {

        // If the game isn't in a menu or paused, and the car is at a taxi stop—
        if (GameStateManager.Gamestate == GAMESTATE.PLAYING && dialogueManager.car.atTaxiStop && dialogueManager.waitForRouting) {

            // If the GPS is hovered over—
            if (hovered) {

                // If this UI element is clicked—
                if (Input.GetMouseButtonDown(0))
                {
                    // Execute click function
                    unityEvent.Invoke();
                }
            }
        }
    }

    public override void FixedUpdate() {

        // If the game is not in a menu or in the main menu, and the car is at a taxi stop—
        if (GameStateManager.Gamestate == GAMESTATE.PLAYING && dialogueManager.car.atTaxiStop && dialogueManager.waitForRouting) {

            // Raycast from the UI element to the mouse cursor
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // If raycast successfully hits mouse cursor (meaning cursor is currently hovered over UI element), and the UI element belongs to this script—
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
            {
                if (hit.collider.transform == transform) {

                    // Execute hover function
                    OnHover();
                }
                
            } 
            // If cursor is not hovered over element, reset to default state
            else {
                DefaultState();
            }
            
        }
    }
    
    // Function to be executed when the button is clicked
    public override void OnClick()
    {
        // Disable dialogue continue button
        continueButton.SetActive(false);

        base.OnClick();

        // Switches game's state to be in a menu
        GameStateManager.SetState(GAMESTATE.MENU);

        // Switches the camera's spline dolly track
        splineDolly.Spline = spline;

        // Reset camera position on spline dolly
        splineDolly.CameraPosition = 0;

        // Focus on this element
        cinemachineCam.LookAt = focusOn.transform;

        // Start moving the camera on the dolly spline track
        StartCoroutine(StartDollyMovement());
        
        Debug.Log("GPS clicked!");
    }

    // Starts the dolly movement on the current spline track
    public virtual IEnumerator StartDollyMovement() {

        // While the camera has not finished moving along spline track—
        while (splineDolly.CameraPosition < 1 /* || cinemachineCam.Lens.FieldOfView > 30 */) {
            /* if (cinemachineCam.Lens.FieldOfView > 30) {
                toonCamera.fieldOfView -= 0.6f;
                cinemachineCam.Lens.FieldOfView -= 0.6f;
            } */

            // If the camera isn't finished moving along the spline track, move it along at different rates
            if (splineDolly.CameraPosition < 1f) {
                if (splineDolly.CameraPosition > 0.7f){
                    splineDolly.CameraPosition += 0.01f;
                } else {
                    splineDolly.CameraPosition += 0.02f;
                }
            }

            // Wait in between moving
            yield return new WaitForSeconds(0.005f);
        }

        // Activate the map when the camera is done moving
        screenUI.SetActive(true);
    }

    public void GPSBackButton() {
        StartCoroutine(EndDollyMovement());
    }

    public virtual IEnumerator EndDollyMovement() {

        // While the camera has not finished moving back along spline track—
        while (splineDolly.CameraPosition > 0) {

            // If the camera isn't finished moving back along the spline track, move it along at different rates
            if (splineDolly.CameraPosition > 0) {
                if (splineDolly.CameraPosition < 0.3f){
                    splineDolly.CameraPosition -= 0.01f;
                } else {
                    splineDolly.CameraPosition -= 0.02f;
                }
            }

            // Wait in between moving
            yield return new WaitForSeconds(0.005f);
        }

        // Reset camera focal point
        cinemachineCam.LookAt = cameraLookAt.transform;

        // Switch game's state to playing
        GameStateManager.SetState(GAMESTATE.PLAYING);

        // Disable map UI
        screenUI.SetActive(false);

        // Enable dialogue continue button if disabled
        if (!continueButton.activeInHierarchy) {
            continueButton.SetActive(true);
        }
    }
}
