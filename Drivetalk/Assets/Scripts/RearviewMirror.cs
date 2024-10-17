using System.Collections;
using UnityEngine;

public class RearviewMirror : GPS
{
    [SerializeField] private DialogueManager dialogueManager;
    public GameObject backButton;

    public override void OnClick()
    {
        dialogueManager.EnterRMM();

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
            yield return new WaitForSeconds(0.01f);
        }
        if (dialogueManager.playingChoices) {
            dialogueManager.ShowChoices();
        } else {
            backButton.SetActive(true);
        }
    }

    public void BackButton() {
        backButton.SetActive(false);

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
