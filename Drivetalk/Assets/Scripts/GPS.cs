using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class GPS : UIElementButton
{

    [Tooltip("Reference to the main Cinemachine camera.")]
    [SerializeField] protected CinemachineCamera cinemachineCam;
    [Tooltip("Reference to the dolly spline component attached to the Cinemachine camera.")]
    [SerializeField] protected CinemachineSplineDolly splineDolly;
    [SerializeField] protected SplineContainer spline;

    [SerializeField] protected GameObject screenUI;

    [SerializeField] protected GameObject cameraLookAt;
    
    public override void OnClick()
    {
        base.OnClick();
        GameStateManager.SetState(GAMESTATE.MENU);

        splineDolly.Spline = spline;

        // Focus on this element
        cinemachineCam.LookAt = gameObject.transform;

        // Start moving the camera on the dolly spline track
        StartCoroutine(StartDollyMovement());
        
        Debug.Log("GPS clicked!");
    }

    public virtual IEnumerator StartDollyMovement() {
        while (splineDolly.CameraPosition < 1) {
            splineDolly.CameraPosition += 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        screenUI.SetActive(true);
    }

    public virtual IEnumerator EndDollyMovement() {
        while (splineDolly.CameraPosition > 0) {
            splineDolly.CameraPosition -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        cinemachineCam.LookAt = cameraLookAt.transform;
        GameStateManager.SetState(GAMESTATE.PLAYING);
        screenUI.SetActive(false);
    }
}
