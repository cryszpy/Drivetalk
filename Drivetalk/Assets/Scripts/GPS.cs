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

        // Reset camera position on spline dolly
        splineDolly.CameraPosition = 0;

        // Focus on this element
        cinemachineCam.LookAt = gameObject.transform;

        // Start moving the camera on the dolly spline track
        StartCoroutine(StartDollyMovement());
        
        Debug.Log("GPS clicked!");
    }

    public virtual IEnumerator StartDollyMovement() {
        while (splineDolly.CameraPosition < 1 || cinemachineCam.Lens.FieldOfView > 30) {
            if (cinemachineCam.Lens.FieldOfView > 30) {
                toonCamera.fieldOfView -= 0.6f;
                cinemachineCam.Lens.FieldOfView -= 0.6f;
            }
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
        while (splineDolly.CameraPosition > 0 || cinemachineCam.Lens.FieldOfView < 60) {
            if (cinemachineCam.Lens.FieldOfView < 60) {
                toonCamera.fieldOfView += 0.6f;
                cinemachineCam.Lens.FieldOfView += 0.6f;
            }
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
