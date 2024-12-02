using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class UIElementSlider : UIElementButton
{
    [Header("SCRIPT REFERENCES")]

    [Tooltip("Reference to the slider's physical object.")]
    [SerializeField] protected GameObject dialObject;

    [Header("STATS")]
    
    [Tooltip("Minimum angle in degrees that this slider / dial can rotate.")]
    [SerializeField] protected float rotationMin;
    [Tooltip("Maximum angle in degrees that this slider / dial can rotate.")]
    [SerializeField] protected float rotationMax;

    [Tooltip("The mouse cursor's previous position, set dynamically in-script.")]
    protected Vector3 mousePreviousPos = Vector3.zero;

    [Tooltip("The difference between mouse positions, set dynamically in-script.")]
    protected Vector3 mousePosDelta = Vector3.zero;

    [Tooltip("Rotation value of this slider.")]
    protected float rotation;

    [Tooltip("Whether the slider is actively being dragged.")]
    [SerializeField] protected bool dragging;

    public override void Start() {
        base.Start();
        dragging = false;
    }

    public override void Update() {
        
        // If the game's state is not in menu or main menu—
        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            // If this slider is being hovered over—
            if (hovered) {

                /* // Start minigame
                if (Input.GetMouseButtonDown(0) && dialogueManager.dashRequestRunning)
                {
                    // Execute click function
                    unityEvent.Invoke();
                } 
                // Allow player to fiddle
                else if (Input.GetMouseButton(0) && !dialogueManager.dashRequestRunning) {
                    StartDrag();
                }
                else {
                    dragging = false;
                } */

                if (Input.GetMouseButton(0)) {
                    unityEvent.Invoke();
                } else {
                    dragging = false;
                }
            }

            // If player is fiddling with dial and no dash request is running
            /* if (dragging && !dialogueManager.dashRequestRunning) {
                Drag();
            } */
            if (dragging) {
                Drag();
            }

            // Updates mouse position every frame
            mousePreviousPos = Input.mousePosition;
        }
    }

    public override void FixedUpdate() {

        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            // Raycast from the UI element to the mouse cursor
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // If raycast successfully hits mouse cursor (meaning cursor is currently hovered over UI element), and the UI element belongs to this script—
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
            {
                if (hit.collider.transform == transform && !Input.GetMouseButtonUp(0)) {
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

    public virtual void Drag() {

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
    }

    // Function to be executed when slider is clicked
    public override void OnClick()
    {
        dragging = true;
        base.OnClick();

        /* GameStateManager.SetState(GAMESTATE.MENU);

        splineDolly.Spline = spline;

        // Reset camera position on spline dolly
        splineDolly.CameraPosition = 0;

        // Focus on this gameObject
        cinemachineCam.LookAt = gameObject.transform;

        // Start moving the camera on the dolly spline track
        StartCoroutine(StartDollyMovement());
        
        Debug.Log("Slider clicked!");  */
    }

    /* public virtual IEnumerator StartDollyMovement() {
        while (splineDolly.CameraPosition < 1) {
            splineDolly.CameraPosition += 0.03f;
            yield return new WaitForSeconds(0.01f);
        }
        screenUI.SetActive(true);
        insideMinigame = true;
    }

    public virtual IEnumerator EndDollyMovement() {
        insideMinigame = false;
        while (splineDolly.CameraPosition > 0) {
            splineDolly.CameraPosition -= 0.03f;
            yield return new WaitForSeconds(0.01f);
        }
        cinemachineCam.LookAt = cameraLookAt.transform;
        GameStateManager.SetState(GAMESTATE.PLAYING);
        screenUI.SetActive(false);
    } */
}
