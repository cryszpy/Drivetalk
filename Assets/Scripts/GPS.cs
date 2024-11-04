using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class GPS : UIElementButton
{

    [SerializeField] protected GameObject focusOn;

    [Tooltip("Reference to the main Cinemachine camera.")]
    [SerializeField] protected CinemachineCamera cinemachineCam;
    [Tooltip("Reference to the dolly spline component attached to the Cinemachine camera.")]
    [SerializeField] protected CinemachineSplineDolly splineDolly;
    [SerializeField] protected SplineContainer spline;

    [SerializeField] protected GameObject screenUI;

    [SerializeField] protected GameObject cameraLookAt;

    public override void Update() {
        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU && dialogueManager.car.atTaxiStop) {
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

        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.MAINMENU && dialogueManager.car.atTaxiStop) {

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
    
    public override void OnClick()
    {
        dialogueManager.car.atTaxiStop = false;

        base.OnClick();
        GameStateManager.SetState(GAMESTATE.MENU);

        splineDolly.Spline = spline;

        // Reset camera position on spline dolly
        splineDolly.CameraPosition = 0;

        // Focus on this element
        cinemachineCam.LookAt = focusOn.transform;

        // Start moving the camera on the dolly spline track
        StartCoroutine(StartDollyMovement());
        
        Debug.Log("GPS clicked!");
    }

    public virtual IEnumerator StartDollyMovement() {
        while (splineDolly.CameraPosition < 1 /* || cinemachineCam.Lens.FieldOfView > 30 */) {
            /* if (cinemachineCam.Lens.FieldOfView > 30) {
                toonCamera.fieldOfView -= 0.6f;
                cinemachineCam.Lens.FieldOfView -= 0.6f;
            } */
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
    }

    public virtual IEnumerator EndDollyMovement() {
        while (splineDolly.CameraPosition > 0 /* || cinemachineCam.Lens.FieldOfView < 60 */) {
            /* if (cinemachineCam.Lens.FieldOfView < 60) {
                toonCamera.fieldOfView += 0.6f;
                cinemachineCam.Lens.FieldOfView += 0.6f;
            } */
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
