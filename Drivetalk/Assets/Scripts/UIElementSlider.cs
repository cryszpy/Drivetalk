using UnityEngine;

public class UIElementSlider : UIElementButton
{

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

                // If this UI element is clicked—
                if (Input.GetMouseButton(0))
                {
                    // Execute click function
                    unityEvent.Invoke();
                } else {
                    dragging = false;
                }
            }

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

    public virtual void Drag() {

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
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newRot, transform.localEulerAngles.z);

        //Debug.Log("Slider turned!");
    }

    public override void OnClick()
    {
        base.OnClick();
        dragging = true;
    }
}
