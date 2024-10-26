using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class UIElementButton : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    [SerializeField] protected Camera toonCamera;

    [SerializeField] protected DialogueManager dialogueManager;

    public UnityEvent unityEvent = new();

    [Tooltip("Version of GameObject to appear when hovered over.")]
    public GameObject hoveredObject;

    [Tooltip("Reference to main camera.")]
    public Camera mainCamera;

    [SerializeField] protected LayerMask layerMask;

    [SerializeField] protected bool hovered;

    [SerializeField] protected bool insideMinigame = false;

    public virtual void Start() {
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
        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU) {
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

    public virtual void OnClick() {
        // Disable hovered version of GameObject
        if (hoveredObject.activeSelf) {
            hoveredObject.SetActive(false);
        }
    }

    public virtual void OnHover() {
        hovered = true;
        // Enable hovered version of GameObject
        if (!hoveredObject.activeSelf) {
            hoveredObject.SetActive(true);
        }
    }

    public virtual void DefaultState() {
        hovered = false;
        // Disable hovered version of GameObject
        if (hoveredObject.activeSelf) {
            hoveredObject.SetActive(false);
        }
    }

}
