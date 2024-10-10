using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GPS : UIElementButton
{

    [Tooltip("Reference to the main Cinemachine camera.")]
    [SerializeField] CinemachineCamera cinemachineCam;
    [Tooltip("Reference to the dolly spline track to follow for this UI element on click.")]
    [SerializeField] CinemachineSplineDolly splineDolly;
    
    public override void OnClick()
    {
        base.OnClick();
        GameStateManager.SetState(GAMESTATE.MENU);

        // Focus on this element
        cinemachineCam.LookAt = gameObject.transform;

        // Start moving the camera on the dolly spline track
        StartCoroutine(StartDollyMovement());
        
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("GPS clicked!");
    }

    public override void OnHover()
    {
        base.OnHover();
    }

    private IEnumerator StartDollyMovement() {
        while (splineDolly.CameraPosition < 1) {
            splineDolly.CameraPosition += 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
