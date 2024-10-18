using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RearviewMirror : GPS
{
    public GameObject person;
    [SerializeField] private DialogueManager dialogueManager;
    public GameObject backButton;

    public override void OnClick()
    {
        dialogueManager.EnterRMM();

        person = GameObject.FindGameObjectWithTag("PickedUp");

        base.OnClick();
        GameStateManager.SetState(GAMESTATE.MENU);

        splineDolly.Spline = spline;

        // Focus on this element
        cinemachineCam.LookAt = gameObject.transform;

        // Start moving the camera on the dolly spline track
        StartCoroutine(StartDollyMovement());
        
        Debug.Log("Rearview mirror clicked!");
    }

    public override IEnumerator StartDollyMovement() {
        while (splineDolly.CameraPosition < 1) {
            splineDolly.CameraPosition += 0.007f;
            //person.transform.localScale *= 1.005f;
            yield return new WaitForSeconds(0.01f);
        }
        if (dialogueManager.playingChoices) {
            dialogueManager.ShowChoices();
        }
    }

    public void BackButton() {
        dialogueManager.ExitRMM();

        StartCoroutine(EndDollyMovement());
    }

    public override IEnumerator EndDollyMovement() {
        while (splineDolly.CameraPosition > 0) {
            splineDolly.CameraPosition -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        cinemachineCam.LookAt = cameraLookAt.transform;
        GameStateManager.SetState(GAMESTATE.PLAYING);
    }
}
