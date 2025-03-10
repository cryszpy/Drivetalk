using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class GPS : UIElementSlider
{
    [Header("GPS REFERENCES")] // --------------------------------------------------------------------------------------------

    [Tooltip("Reference to the dialogue manager.")]
    protected DialogueManager dialogueManager;

    [Tooltip("Reference to the object to look at when the GPS is clicked.")]
    [SerializeField] protected GameObject focusOn;

    [Tooltip("Reference to the main Cinemachine camera.")]
    [SerializeField] protected CinemachineCamera cinemachineCam;

    [Tooltip("Reference to the dolly spline component attached to the Cinemachine camera.")]
    [SerializeField] protected CinemachineSplineDolly splineDolly;

    [Tooltip("Reference to the spline dolly track to switch the camera to when the GPS is clicked.")]
    [SerializeField] protected CinemachineRotationComposer rotationComposer;

    [Tooltip("Reference to the map UI.")]
    [SerializeField] protected GPSScreen gpsScreen;

    [Tooltip("Reference to the camera focal point object.")]
    [SerializeField] protected GameObject cameraLookAt;

    [Tooltip("Reference to the GPS spline track.")]
    [SerializeField] protected SplineContainer spline;

    [Tooltip("The recent destination prefab.")]
    [SerializeField] private GameObject recentDestElement;

    private void OnEnable() {
        GameStateManager.EOnDestinationSet += StartDampingReset;
        //GameStateManager.EOnPassengerPickup += CachePassengerDest;
    }

    private void OnDisable() {
        GameStateManager.EOnDestinationSet -= StartDampingReset;
        //GameStateManager.EOnPassengerPickup -= CachePassengerDest;
    }

    public override void Start()
    {
        base.Start();

        dialogueManager = GameStateManager.dialogueManager;
    }

    /* private void CachePassengerDest() {

        // Gets the index number of the current passenger
        int index = CarController.PassengersDrivenIDs.IndexOf(carPointer.car.currentPassenger.id);

        // Gets the passenger's requested destination based on the current ride number
        GameObject selectedDestination = carPointer.car.currentPassenger.requestedDestinationTiles[CarController.PassengersDrivenRideNum[index] - 1];

        // Gets the selected button corresponding to the requested destination
        GameObject selectedButton = gpsButtons.Find(x => x.GetComponent<GPSRecentDestination>().destinationObject == selectedDestination);

        // Iterates through all buttons and turns them off except for the selected button
        foreach (GameObject button in gpsButtons) {

            if (button != selectedButton) {

                if (button.TryGetComponent<Image>(out var image)) {
                    image.color = lockedColor;
                } else {
                    Debug.LogError("Couldn't find Image component on button!");
                }

                if (button.TryGetComponent<Button>(out var script)) {
                    script.enabled = false;
                } else {
                    Debug.LogError("Couldn't find Button component on button!");
                }
            } else {

                if (button.TryGetComponent<Image>(out var image)) {
                    image.color = unlockedColor;
                } else {
                    Debug.LogError("Couldn't find Image component on button!");
                }

                if (button.TryGetComponent<Button>(out var script)) {
                    script.enabled = true;
                } else {
                    Debug.LogError("Couldn't find Button component on button!");
                }
            }
        }
    } */

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

            if (dragging) {
                OutlineObjects(hoveredLayer);
                Drag();
            }

            // Updates mouse position every frame
            mousePreviousPos = Input.mousePosition;
        }
    }

    public override void OutlineRaycast()
    {
        // If the game is not in a menu or in the main menu, and the car is at a taxi stop—
        if (GameStateManager.Gamestate == GAMESTATE.PLAYING && dialogueManager.car.atTaxiStop && dialogueManager.waitForRouting) {

            // Raycast from the UI element to the mouse cursor
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // If raycast successfully hits mouse cursor (meaning cursor is currently hovered over UI element), and the UI element belongs to this script—
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
            {
                RaycastCheck(hit);
            } 
            // If cursor is not hovered over element, reset to default state
            else {
                DefaultState();
            }
        }
    }

    public override void RaycastCheck(RaycastHit hit)
    {
        // If the raycast has hit this button—
        if (hit.collider.gameObject == gameObject) {

            // If there isn't any other button being hovered—
            if (carPointer.hoveredButton == null) {

                // Set this button to be hovered
                carPointer.hoveredButton = gameObject;
            }

            // If this button is the only button being hovered—
            if (carPointer.hoveredButton == gameObject) {

                // Trigger OnHover effects
                OnHover();
            }
            
        } else {
            DefaultState();
        }
    }

    // Function to be executed when the button is clicked
    public override void OnClick()
    {
        // Disable dialogue continue button
        //continueButton.SetActive(false);

        // Hide GPS indicator
        GameStateManager.dialogueManager.gpsIndicator.SetActive(false);

        base.OnClick();

        // Switches game's state to be in a menu
        GameStateManager.SetState(GAMESTATE.MENU);

        // Switches the camera's spline dolly track
        splineDolly.Spline = spline;

        // Reset camera position on spline dolly
        //splineDolly.CameraPosition = 0;

        // Enables smooth rotation
        rotationComposer.Damping = new(1, 1);

        // Focus on this element
        cinemachineCam.LookAt = focusOn.transform;

        // Activate the map when the camera is done moving
        //gpsScreen.SetActive(true);

        // Start moving the camera on the dolly spline track
        StartCoroutine(StartDollyMovement());
        
        Debug.Log("GPS clicked!");
    }

    // Starts the dolly movement on the current spline track
    public virtual IEnumerator StartDollyMovement() {

        // While the camera has not finished moving along spline track—
        while (splineDolly.CameraPosition < 1) {

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
        gpsScreen.gameObject.SetActive(true);
    }

    public void GPSBackButton() {
        StartCoroutine(EndDollyMovement());

        // Show GPS indicator
        GameStateManager.dialogueManager.gpsIndicator.SetActive(true);
    }

    public virtual IEnumerator EndDollyMovement() {

        DefaultState();

        if (carPointer.hoveredButton == gameObject) {
            carPointer.hoveredButton = null;
        }

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
        gpsScreen.gameObject.SetActive(false);

        // Reset input
        gpsScreen.inputField.text = "";
        gpsScreen.actualText.text = "";

        // Add destination to recent destinations if it's not there already
        if (!gpsScreen.recentDestTiles.Contains(gpsScreen.currentDestination.tile)) {

            // Spawn a recent location element
            GameObject recent = GPSRecentDestination.Create(recentDestElement, gpsScreen.pivot.transform, gpsScreen);

            // Add the current destination tile to the recent destinations list
            gpsScreen.recentDestTiles.Add(gpsScreen.currentDestination.tile);
        }

        // Disables smooth rotation
        float damp = 1;

        while (damp > 0) {
            rotationComposer.Damping = new(damp, damp);
            damp -= 0.03f;

            yield return new WaitForSeconds(0.01f);
        }

        rotationComposer.Damping = new(0, 0);
    }

    private void StartDampingReset() {
        StartCoroutine(EndDollyMovement());
    }
}