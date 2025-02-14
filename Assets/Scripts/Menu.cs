using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class Menu : MonoBehaviour
{
    [Tooltip("Reference to the global audio manager.")]
    [SerializeField] private AudioManager audioManager;

    [Tooltip("Reference to the list of all passengers in the game.")]
    public PassengerList passengerList;

    [Tooltip("Reference to the main Cinemachine camera.")]
    [SerializeField] private CinemachineCamera cinemachineCam;

    [Tooltip("Reference to the dolly spline component attached to the Cinemachine camera.")]
    [SerializeField] private CinemachineSplineDolly splineDolly;

    [Tooltip("Reference to the main menu spline dolly track.")]
    [SerializeField] private SplineContainer spline;

    [Tooltip("Reference to the main menu UI hierarchy stack.")]
    [SerializeField] private GameObject mainMenuUI;

    [Tooltip("Reference to the Animator component on this object.")]
    [SerializeField] private Animator transitionAnimator;

    [Tooltip("Reference to the camera focal point object.")]
    [SerializeField] private GameObject cameraLookAtObject;

    [Tooltip("Reference to the camera focal point script.")]
    private CameraLookAt cameraLookAt;
    
    [Tooltip("Reference to the pause menu hierarchy stack.")]
    [SerializeField] private GameObject pauseMenu;

    [Tooltip("Reference to the pause menu buttons.")]
    [SerializeField] private GameObject pauseScreen;

    [Tooltip("Reference to the pause menu settings buttons.")]
    [SerializeField] private GameObject pauseMenuSettings;

    [Tooltip("Reference to the transcript log object.")]
    [SerializeField] private GameObject transcriptScreen;

    [Tooltip("Reference to the transition time for going back to the main menu.")]
    [SerializeField] private float transitionTime;

    [Tooltip("Boolean flag; Whether the game is in the main menu or not.")]
    [SerializeField] private bool inMainMenu = true;

    private void Start() {

        // Starts in the main menu
        inMainMenu = true;

        // Switches the game's state to the main menu
        GameStateManager.SetState(GAMESTATE.MAINMENU);

        // Sets the camera dolly to main menu spline track
        splineDolly.Spline = spline;

        // Sets the camera's look at point to the focal point object
        cinemachineCam.LookAt = cameraLookAtObject.transform;

        // If the camera focal point script is accessible
        if (cameraLookAtObject.TryGetComponent<CameraLookAt>(out var script)) {

            // Sets the camera focal point script reference
            cameraLookAt = script;
        }

        // Ensure upon reloading of the scene that singleton pattern references are assigned
        GameStateManager.dialogueManager.FindReferences();

        // Hide menu screens on game start
        pauseMenu.SetActive(false);
        pauseScreen.SetActive(false);
        pauseMenuSettings.SetActive(false);
        transcriptScreen.SetActive(false);

        Debug.Log("OPENED GAME!");
    }

    private void Update() {

        // If the player hits the ESCAPE key, and the game is not in the main menu, and the pause menu exists—
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu != null && !inMainMenu) {

            // Toggles the pause menu
            TogglePauseMenu();
        }
    }

    // Starts the game, disables main menu
    private IEnumerator StartDollyMovement() {
        Debug.Log("Starting game!");

        // Disables main menu UI
        mainMenuUI.SetActive(false);

        // While the camera hasn't finished moving along dolly path—
        while (splineDolly.CameraPosition > 0) {

            // Moves the camera and dilates camera zoom parameter
            if (cameraLookAt.cameraTargetDivider > 10) {
                cameraLookAt.cameraTargetDivider -= 5f;
            }
            splineDolly.CameraPosition -= 0.01f;

            // Waits between movements
            yield return new WaitForSeconds(0.01f);
        }

        // Allows camera to follow mouse cursor movement
        //cameraLookAt.transform.localPosition = new(-0.3855f, 0.228f, 3.19347f);
        //cinemachineCam.LookAt = cameraLookAt.transform;

        // Sets game out of main menu when done
        inMainMenu = false;

        // Switches game state to playing
        GameStateManager.SetState(GAMESTATE.PLAYING);
    }

    // Fades screen to black when quitting to main menu
    private IEnumerator FadeToBlack() {
        //transitionScreen.SetActive(true);

        // Plays fade-to-black transition
        transitionAnimator.SetBool("Play", true);

        // Waits for the transition to finish
        yield return new WaitForSeconds(transitionTime);

        // Reloads the scene (back to main menu)
        SceneManager.LoadScene(0);
    }

    // Toggles pause menu
    public void TogglePauseMenu() {

        // UNPAUSE
        if (GameStateManager.Gamestate == GAMESTATE.PAUSED) {

            // Switches gamestate to playing
            GameStateManager.SetState(GAMESTATE.PLAYING);

            // Hides pause menu
            pauseMenu.SetActive(false);
            pauseScreen.SetActive(false);
            pauseMenuSettings.SetActive(false);
            transcriptScreen.SetActive(false);
        } 
        // PAUSE
        else if (GameStateManager.Gamestate == GAMESTATE.PLAYING){

            // Switches gamestate to paused
            GameStateManager.SetState(GAMESTATE.PAUSED);

            // Shows pause menu
            pauseScreen.SetActive(true);
            pauseMenu.SetActive(true);
            pauseMenuSettings.SetActive(false);
            transcriptScreen.SetActive(false);
        }
    }

    // Toggles pause menu settings page
    public void TogglePauseMenuSettings() {

        // SHOW
        if (!pauseMenuSettings.activeInHierarchy) {
            pauseScreen.SetActive(false);
            pauseMenuSettings.SetActive(true);
            transcriptScreen.SetActive(false);
        } 
        // HIDE
        else if (pauseMenuSettings.activeInHierarchy) {
            pauseMenuSettings.SetActive(false);
            pauseScreen.SetActive(true);
            transcriptScreen.SetActive(false);
        }
    }

    // Toggles transcript log screen
    public void ToggleTranscriptLog() {

        // SHOW
        if (!transcriptScreen.activeInHierarchy) {
            transcriptScreen.SetActive(true);
            pauseScreen.SetActive(false);
            pauseMenuSettings.SetActive(false);
        } 
        // HIDE
        else if (transcriptScreen.activeInHierarchy) {
            transcriptScreen.SetActive(false);
            pauseMenuSettings.SetActive(false);
            pauseScreen.SetActive(true);
        }
    }

    // Loads the main menu (called from button script assignment)
    public void LoadMainMenu() {
        TogglePauseMenu();
        ResetStats();
        StartCoroutine(FadeToBlack());
    }

    public void ResetStats() {
        CarController.Rating = 0;
        CarController.Temperature = 0.5f;
        CarController.PassengersDrivenIDs.Clear();
        CarController.PassengersDrivenRideNum.Clear();
        CarController.TotalPassengersDriven = 0;
        CarController.LastPassengerID = 0;
        CarController.CurrentRadioChannel = 0;
        CarController.RadioVolume = 0;
    }

    // Play button functionality (called from button script assignment)
    public void PlayButton() {

        // Reset stats
        passengerList.ResetListInOrder(passengerList.exhaustedStory, passengerList.storyPassengers);
        ResetStats();

        // Generate day rides
        GameStateManager.car.GenerateDayRides(passengerList.storyPassengers.Count);

        // Start camera dolly motion
        StartCoroutine(StartDollyMovement());

        // Play engine sound
        audioManager.PlaySoundByName("Engine");
        
        // Start initial drive
        GameStateManager.dialogueManager.car.StartInitialDrive();
    }

    // Quits the game (called from button script assignment)
    public void QuitButton() {
        Application.Quit();
        Debug.Log("QUIT!");
    }
}
