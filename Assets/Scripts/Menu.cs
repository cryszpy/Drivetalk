using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class Menu : MonoBehaviour
{
    [Tooltip("Reference to the main Cinemachine camera.")]
    [SerializeField] private CinemachineCamera cinemachineCam;
    [Tooltip("Reference to the dolly spline component attached to the Cinemachine camera.")]
    [SerializeField] private CinemachineSplineDolly splineDolly;
    [SerializeField] private SplineContainer spline;

    [SerializeField] private GameObject mainMenuUI;

    [SerializeField] private GameObject transitionScreen;
    [SerializeField] private Animator transitionAnimator;

    [SerializeField] private GameObject cameraLookAtObject;
    private CameraLookAt cameraLookAt;
    
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private float transitionTime;

    [SerializeField] private bool inMainMenu = true;

    private void Start() {
        inMainMenu = true;
        GameStateManager.SetState(GAMESTATE.MAINMENU);

        splineDolly.Spline = spline;
        cinemachineCam.LookAt = cameraLookAtObject.transform;
        if (cameraLookAtObject.TryGetComponent<CameraLookAt>(out var script)) {
            cameraLookAt = script;
        }

        Debug.Log("OPENED GAME!");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu != null && !inMainMenu) {
            TogglePauseMenu();
        }
    }

    // Starts the game, disables main menu
    private IEnumerator StartDollyMovement() {
        Debug.Log("Starting game!");

        // Disables main menu UI
        mainMenuUI.SetActive(false);

        while (splineDolly.CameraPosition < 1) {
            if (cameraLookAt.cameraTargetDivider > 10) {
                cameraLookAt.cameraTargetDivider -= 5;
            }
            splineDolly.CameraPosition += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        // Allows camera to follow mouse cursor movement
        //cameraLookAt.transform.localPosition = new(-0.3855f, 0.228f, 3.19347f);
        //cinemachineCam.LookAt = cameraLookAt.transform;

        inMainMenu = false;
        GameStateManager.SetState(GAMESTATE.PLAYING);
    }

    // Goes back into main menu
    private IEnumerator EndDollyMovement() {
        Debug.Log("Ending game!");

        inMainMenu = true;

        while (splineDolly.CameraPosition > 0) {
            if (cameraLookAt.cameraTargetDivider < 100) {
                cameraLookAt.cameraTargetDivider += 5;
            }
            splineDolly.CameraPosition -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        //cinemachineCam.LookAt = null;
        GameStateManager.SetState(GAMESTATE.MAINMENU);
        mainMenuUI.SetActive(true);
    }

    private IEnumerator FadeToBlack() {
        //transitionScreen.SetActive(true);

        transitionAnimator.SetBool("Play", true);

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(0);
    }

    public void TogglePauseMenu() {

        // UNPAUSE
        if (GameStateManager.Gamestate == GAMESTATE.PAUSED) {
            GameStateManager.SetState(GAMESTATE.PLAYING);
            pauseMenu.SetActive(false);
        } 
        // PAUSE
        else if (GameStateManager.Gamestate == GAMESTATE.PLAYING){
            GameStateManager.SetState(GAMESTATE.PAUSED);
            pauseMenu.SetActive(true);
        }
    }

    public void LoadMainMenu() {
        TogglePauseMenu();
        StartCoroutine(FadeToBlack());
    }

    public void PlayButton() {
        StartCoroutine(StartDollyMovement());
    }

    public void QuitButton() {
        Application.Quit();
        Debug.Log("QUIT!");
    }
}
