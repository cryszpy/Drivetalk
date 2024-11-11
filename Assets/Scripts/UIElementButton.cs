using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class UIElementButton : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    [Tooltip("Reference to the toon shader camera.")]
    [SerializeField] protected Camera toonCamera;

    [Tooltip("Reference to the dialogue manager.")]
    [SerializeField] protected DialogueManager dialogueManager;

    [Tooltip("New on-click function event.")]
    public UnityEvent unityEvent = new();

    [Tooltip("Version of GameObject to appear when hovered over.")]
    public GameObject hoveredObject;

    [Tooltip("Reference to main camera.")]
    public Camera mainCamera;

    [Tooltip("This button's layer mask. Selected layers will be the *only* layers raycasted on.")]
    [SerializeField] protected LayerMask layerMask;

    [Tooltip("Boolean flag; Checks whether this button is hovered over.")]
    [SerializeField] protected bool hovered;

    public virtual void Start() {

        // Assigns references to any missing script references
        if (!mainCamera) {
            mainCamera = Camera.main;
            Debug.LogWarning("Main camera not assigned! Reassigned.");
        }
        if (!dialogueManager) {
            dialogueManager = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
            Debug.LogWarning("Dialogue manager not assigned! Reassigned.");
        }
    }

    public virtual void Update() {

        // If the game's state is not in a menu or main menu—
        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU) {

            // If the button is currently hovered over—
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

    public virtual void FixedUpdate() {

        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU) {

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

    // Function to be executed when button is clicked
    public virtual void OnClick() {
        // Disable hovered version of GameObject
        if (hoveredObject.activeSelf) {
            hoveredObject.SetActive(false);
        }
    }

    // Function to be executed when button is hovered over
    public virtual void OnHover() {
        hovered = true;
        // Enable hovered version of GameObject
        if (!hoveredObject.activeSelf) {
            hoveredObject.SetActive(true);
        }
    }

    // Default state of the button
    public virtual void DefaultState() {
        hovered = false;
        // Disable hovered version of GameObject
        if (hoveredObject.activeSelf) {
            hoveredObject.SetActive(false);
        }
    }

}
