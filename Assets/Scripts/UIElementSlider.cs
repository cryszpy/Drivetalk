using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIElementSlider : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    [HideInInspector] public CarPointer carPointer;

    [Tooltip("Reference to the slider's physical object.")]
    public GameObject dialObject;

    [Tooltip("List of all objects to highlight when hovering.")]
    public List<GameObject> objectsToOutline = new();

    [Tooltip("New on-click function event.")]
    public UnityEvent unityEvent = new();

    [Tooltip("Reference to main camera.")]
    [HideInInspector] public Camera mainCamera;

    [Header("STATS")]

    [Tooltip("This button's layer mask. Selected layers will be the *only* layers raycasted on.")]
    [SerializeField] protected LayerMask layerMask;

    [Tooltip("Boolean flag; Checks whether this button is hovered over.")]
    [HideInInspector] public bool hovered;

    [Tooltip("The layer number for the default state of the object.")]
    public int regularLayer;
    [Tooltip("The layer number for the hovered state of the object.")]
    [SerializeField] protected int hoveredLayer;
    
    [Tooltip("Minimum angle in degrees that this slider / dial can rotate.")]
    public float rotationMin;
    [Tooltip("Maximum angle in degrees that this slider / dial can rotate.")]
    public float rotationMax;

    [Tooltip("The mouse cursor's previous position, set dynamically in-script.")]
    protected Vector3 mousePreviousPos = Vector3.zero;

    [Tooltip("The difference between mouse positions, set dynamically in-script.")]
    protected Vector3 mousePosDelta = Vector3.zero;

    [Tooltip("Rotation value of this slider.")]
    protected float change;

    [Tooltip("Whether the slider is actively being dragged.")]
    public bool dragging;

    public virtual void Start() {
        
        // Assigns references to any missing script references
        if (!mainCamera) {
            mainCamera = Camera.main;
            Debug.LogWarning("Main camera not assigned! Reassigned.");
        }

        dragging = false;

        if (!carPointer) {
            carPointer = GameObject.FindGameObjectWithTag("CarPointer").GetComponent<CarPointer>();
            Debug.Log("CarPointer component null! Reassigned.");
        }
    }

    public virtual void Update() {
        
        // If the game's state is not in menu or main menu—
        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            // If this slider is being hovered over—
            if (hovered) {
                OnHover();

                if (Input.GetMouseButtonDown(0)) {
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

    public virtual void FixedUpdate() {
        OutlineRaycast();
    }

    public virtual void OutlineRaycast() {

        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            // Raycast from the UI element to the mouse cursor
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // If raycast successfully hits mouse cursor (meaning cursor is currently hovered over UI element), and the UI element belongs to this script—
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, layerMask)) 
            {
                RaycastCheck(hit);
                // If cursor is over button, hovered == true
                // if cursor is not over button, hovered == false
                // BUT if cursor is over button, and button is held down, hovered == true
                // if button is clicked or held down, dragging = true, otherwise false
                
            } 
            // If cursor is not hovered over element, reset to default state
            else {
                DefaultState();
            }
        }
    }

    public virtual void RaycastCheck(RaycastHit hit) {

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

    public virtual void Drag() {

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
            mousePosDelta = Input.mousePosition - mousePreviousPos;

            // Get the mouse's difference in position applied to the slider's desired rotation axis
            change = Vector3.Dot(mousePosDelta, new Vector3(1, 0, 0));

            float currentRot = dialObject.transform.localEulerAngles.y;
            float newRot = currentRot + change;

            // Limit slider rotation to be between a certain minimum and maximum degree angle
            newRot = Mathf.Clamp(newRot, rotationMin, rotationMax);

            // Apply change in rotation based on mouse cursor movement
            dialObject.transform.localEulerAngles = new Vector3(dialObject.transform.localEulerAngles.x, newRot, dialObject.transform.localEulerAngles.z);
        }
    }

    public virtual void OutlineObjects(int newLayer) {
        foreach (var obj in objectsToOutline) {
            obj.layer = newLayer;
        }
    }

    // Function to be executed when slider is clicked
    public virtual void OnClick()
    {
        dragging = true;
        OutlineObjects(regularLayer);
    }

    // Function to be executed when button is hovered over
    public virtual void OnHover() {
        OutlineObjects(hoveredLayer);
        hovered = true;
    }

    public virtual void DefaultState()
    {
        OutlineObjects(regularLayer);
        hovered = false;

        // If this button isn't being held down or hovered, but is still set as the hovered button, reset
        if (!dragging && carPointer.hoveredButton == gameObject) {
            carPointer.hoveredButton = null;
        }
    }
}