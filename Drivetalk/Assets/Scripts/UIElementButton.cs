using UnityEngine;
using UnityEngine.Events;

public class UIElementButton : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    public UnityEvent unityEvent = new();

    [Tooltip("Version of GameObject to appear when hovered over.")]
    public GameObject hoveredObject;

    [Tooltip("Reference to main camera.")]
    public Camera mainCamera;

    public virtual void Start() {
        if (!mainCamera) {
            mainCamera = Camera.main;
            Debug.LogWarning("Main camera not assigned! Reassigned.");
        }
    }

    public virtual void Update() {

        if (GameStateManager.Gamestate != GAMESTATE.MENU) {

            // Raycast from the UI element to the mouse cursor
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // Ignore objects in layer 7 (DiegeticUIIgnore)
            int layerMask = ~(1 << 7);

            // If raycast successfully hits mouse cursor (meaning cursor is currently hovered over UI element), and the UI element belongs to this script—
            if (Physics.Raycast(ray, out RaycastHit hit, 25f, layerMask) && hit.collider.gameObject == gameObject)
            {
                // Execute hover function
                OnHover();

                // If this UI element is clicked—
                if (Input.GetMouseButtonDown(0))
                {
                    // Execute click function
                    unityEvent.Invoke();
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
        // Enable hovered version of GameObject
        if (!hoveredObject.activeSelf) {
            hoveredObject.SetActive(true);
        }
    }

    public virtual void DefaultState() {
        // Disable hovered version of GameObject
        if (hoveredObject.activeSelf) {
            hoveredObject.SetActive(false);
        }
    }

}
