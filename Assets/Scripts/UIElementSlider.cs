using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class UIElementSlider : UIElementButton
{
    [Header("SCRIPT REFERENCES")]

    [SerializeField] protected GameObject dialObject;

    [Tooltip("Reference to the main Cinemachine camera.")]
    [SerializeField] protected CinemachineCamera cinemachineCam;

    [Tooltip("Reference to the dolly spline component attached to the Cinemachine camera.")]
    [SerializeField] protected CinemachineSplineDolly splineDolly;
    [SerializeField] protected SplineContainer spline;

    [SerializeField] protected GameObject screenUI;

    [SerializeField] protected GameObject cameraLookAt;

    [Header("STATS")]
    
    [Tooltip("Minimum angle in degrees that this slider / dial can rotate.")]
    [SerializeField] protected float rotationMin;
    [Tooltip("Maximum angle in degrees that this slider / dial can rotate.")]
    [SerializeField] protected float rotationMax;

    protected Vector3 mousePreviousPos = Vector3.zero;
    protected Vector3 mousePosDelta = Vector3.zero;
    protected float rotation;

    [Tooltip("Whether the slider is actively being dragged.")]
    [SerializeField] protected bool dragging;

    public override void Start() {
        base.Start();
        dragging = false;
    }

    public override void Update() {
        
        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU) {

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

        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU) {

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

    /* public virtual void StartDrag() {
        if (hoveredObject.activeSelf) {
            hoveredObject.SetActive(false);
        }
        dragging = true;
    } */

    public virtual void Drag() {

        // Check whether the player has stopped dragging slider
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }

        mousePosDelta = Input.mousePosition - mousePreviousPos;

        // Get the mouse's difference in position applied to the slider's desired rotation axis
        rotation = Vector3.Dot(mousePosDelta, Camera.main.transform.up);

        float currentRot = dialObject.transform.localEulerAngles.y;
        float newRot = currentRot + rotation;

        // Limit slider rotation to be between a certain minimum and maximum degree angle
        newRot = Mathf.Clamp(newRot, rotationMin, rotationMax);

        // Apply change in rotation based on mouse cursor movement
        dialObject.transform.localEulerAngles = new Vector3(dialObject.transform.localEulerAngles.x, newRot, dialObject.transform.localEulerAngles.z);

        //Debug.Log("Slider turned!");
    }

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

    public virtual IEnumerator StartDollyMovement() {
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
    }
}
