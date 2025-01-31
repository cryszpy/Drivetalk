using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInfo : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    [Tooltip("Reference to the currency UI element.")]
    [SerializeField] private CurrencyUI currencyUI;

    [SerializeField] private CarController car;

    [SerializeField] private GameObject windowLookAt;
    [SerializeField] private GameObject mainCameraLookAt;
    [SerializeField] private GameObject leftCameraLookAt;
    [SerializeField] private GameObject rightCameraLookAt;

    [SerializeField] private CinemachineCamera cinemachineCam;
    [SerializeField] private CinemachineRotationComposer rotationComposer;

    [Header("STATS")]

    [Tooltip("Player's currency statistic.")]
    private static float currency;
    public static float Currency { get => currency; set => currency = value; }

    // Used to update the player's currency statistic (NOT UI)
    public static void AddCurrency(int value) {
        Currency += value;
    }

    private bool switchView = false;

    private void OnEnable() {
        GameStateManager.EOnLeftWindow += ViewLeftWindow;
        GameStateManager.EOnRightWindow += ViewRightWindow;
    }

    private void OnDisable() {
        GameStateManager.EOnLeftWindow -= ViewLeftWindow;
        GameStateManager.EOnRightWindow -= ViewRightWindow;
    }

    private void Update() {

        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            switch (switchView) {

                case false:
                    /* if (Input.GetKey(KeyCode.A)) {
                        switchView = true;

                        GameStateManager.EOnLeftWindow?.Invoke();

                    } else if (Input.GetKey(KeyCode.D)) {
                        switchView = true;

                        GameStateManager.EOnRightWindow?.Invoke();
                    }
                    break; */
                    if (Input.GetMouseButton(1)) {
                        switchView = true;

                        IncreaseView();
                    }
                    break;
                case true:
                    /* if (cinemachineCam.LookAt != mainCameraLookAt && (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))) {
                        ResetView();
                    }
                    break; */
                    if (Input.GetMouseButtonUp(1)) {
                        ResetLookAt();
                    }
                    break;
            } 
        }
    }

    private void IncreaseView() {
        rotationComposer.Damping = new(0.5f, 0.5f);
        windowLookAt.SetActive(true);
        cinemachineCam.LookAt = windowLookAt.transform;
        mainCameraLookAt.SetActive(false);
        
        //StartCoroutine(IncreaseLookAt());
    }

    /* private IEnumerator IncreaseLookAt() {
        while (camLookAt.cameraTargetDivider > 2) {
            camLookAt.cameraTargetDivider -= 0.5f;
            yield return new WaitForSeconds(0.02f);
        }
        camLookAt.cameraTargetDivider = 2;
    } */

    private void ResetLookAt() {
        mainCameraLookAt.SetActive(true);
        cinemachineCam.LookAt = mainCameraLookAt.transform;
        windowLookAt.SetActive(false);
        StartCoroutine(DecreaseLookAt());
        //StartCoroutine(DecreaseLookAt());
    }

    private IEnumerator DecreaseLookAt() {
        
        // Disables smooth rotation
        float damp = 0.5f;

        while (damp > 0 && rotationComposer.Damping.x > 0 && rotationComposer.Damping.y > 0) {
            if (switchView) {
                rotationComposer.Damping = new(damp, damp);
                damp -= 0.02f;

                yield return new WaitForSeconds(0.02f);
            } else {
                break;
            }
        }
        switchView = false;
        /* while (camLookAt.cameraTargetDivider < 10) {
            if (camLookAt.cameraTargetDivider > 6) {
                camLookAt.cameraTargetDivider += 0.25f;
                yield return new WaitForSeconds(0.01f);
            } else {
                camLookAt.cameraTargetDivider += 0.125f;
                yield return new WaitForSeconds(0.001f);
            }
        }
        camLookAt.cameraTargetDivider = 10;
        switchView = false; */
    }

    private void ResetView() {

        rotationComposer.Damping = new(1, 1);

        mainCameraLookAt.SetActive(true);

        cinemachineCam.LookAt = mainCameraLookAt.transform;

        leftCameraLookAt.SetActive(false);
        rightCameraLookAt.SetActive(false);

        if (rotationComposer.Damping.x > 0 && rotationComposer.Damping.y > 0) {
            StartCoroutine(DecreaseDamping());
        } else {
            rotationComposer.Damping = new(0, 0);
        }

        switchView = false;
    }

    private void ViewLeftWindow() {

        rotationComposer.Damping = new(1, 1);

        if (!leftCameraLookAt.activeInHierarchy) {
            leftCameraLookAt.SetActive(true);
        }

        cinemachineCam.LookAt = leftCameraLookAt.transform;

        mainCameraLookAt.SetActive(false);
        rightCameraLookAt.SetActive(false);

        StartCoroutine(DecreaseDamping());
    }

    private void ViewRightWindow() {

        rotationComposer.Damping = new(1, 1);

        if (!rightCameraLookAt.activeInHierarchy) {
            rightCameraLookAt.SetActive(true);
        }

        cinemachineCam.LookAt = rightCameraLookAt.transform;

        mainCameraLookAt.SetActive(false);
        leftCameraLookAt.SetActive(false);

        StartCoroutine(DecreaseDamping());
    }

    private IEnumerator DecreaseDamping() {

        rotationComposer.Damping = new(1, 1);

        // Disables smooth rotation
        float damp = 1;

        while (damp > 0 && rotationComposer.Damping.x > 0 && rotationComposer.Damping.y > 0) {
            if (switchView) {
                rotationComposer.Damping = new(damp, damp);
                damp -= 0.02f;

                yield return new WaitForSeconds(0.02f);
            } else {
                break;
            }
        }

        damp = rotationComposer.Damping.x;

        while (!switchView && rotationComposer.Damping.x > 0 && rotationComposer.Damping.y > 0) {

            rotationComposer.Damping = new(damp, damp);

            if (damp - 0.02f < 0) {
                rotationComposer.Damping = new(0, 0);
                break;
            } else {
                damp -= 0.02f;
            }

            yield return new WaitForSeconds(0.02f);
        }
    }
}