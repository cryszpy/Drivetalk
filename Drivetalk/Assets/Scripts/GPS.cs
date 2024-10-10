using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GPS : UIElement
{

    [SerializeField] PlayerCamera playerCam;
    [SerializeField] CinemachineCamera cinemachineCam;
    [SerializeField] CinemachineSplineDolly splineDolly;
    
    public override void OnClick()
    {
        base.OnClick();
        GameStateManager.SetState(GAMESTATE.MENU);
        cinemachineCam.LookAt = gameObject.transform;
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
