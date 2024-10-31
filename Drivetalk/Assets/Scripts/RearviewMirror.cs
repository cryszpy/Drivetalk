using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RearviewMirror : GPS
{
    public GameObject person;
    public GameObject backButton;

    public override void OnClick()
    {
        //dialogueManager.EnterRMM();

        person = GameObject.FindGameObjectWithTag("PickedUp");

        base.OnClick();
        GameStateManager.SetState(GAMESTATE.MENU);

        splineDolly.Spline = spline;

        splineDolly.CameraPosition = 0;

        // Focus on this element
        cinemachineCam.LookAt = gameObject.transform;

        // Start moving the camera on the dolly spline track
        StartCoroutine(StartDollyMovement());
        
        Debug.Log("Rearview mirror clicked!");
    }

    public void BackButton() {
        //dialogueManager.ExitRMM();

        StartCoroutine(EndDollyMovement());
    }

    public override IEnumerator StartDollyMovement() {
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
            yield return new WaitForSeconds(0.0175f);
        }
        if (dialogueManager.playingChoices) {
            dialogueManager.ShowChoices();
        }
    }

    public override IEnumerator EndDollyMovement() {
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
            yield return new WaitForSeconds(0.01f);
        }
        cinemachineCam.LookAt = cameraLookAt.transform;
        GameStateManager.SetState(GAMESTATE.PLAYING);
    }
}
