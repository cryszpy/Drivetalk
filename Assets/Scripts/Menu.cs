using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class Menu : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")] // ------------------------------------------------------------------------------------------------------------

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

    [Tooltip("Reference to the in-game GUI screen.")]
    [SerializeField] private GameObject gameScreen;

    [Tooltip("Reference to the passenger select screen.")]
    [SerializeField] private GameObject selectScreen;
    
    [Tooltip("Reference to the pause menu hierarchy stack.")]
    [SerializeField] private GameObject pauseScreen;

    [Tooltip("Reference to the pause menu settings buttons.")]
    [SerializeField] private GameObject settingsScreen;

    [Tooltip("Reference to the transcript log object.")]
    [SerializeField] private GameObject transcriptScreen;

    [Tooltip("Reference to the credits menu.")]
    [SerializeField] private GameObject creditsScreen;

    [Header("STATS")] // ------------------------------------------------------------------------------------------------------------

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

        // Reset GameStateManager
        GameStateManager.instance.FindReferences();

        // Ensure upon reloading of the scene that singleton pattern references are assigned
        GameStateManager.dialogueManager.FindReferences();

        // Hide menu screens on game start
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
        transcriptScreen.SetActive(false);
        gameScreen.SetActive(false);
        GameStateManager.dialogueManager.demoOverScreen.SetActive(false);

        Debug.Log("OPENED GAME!");
    }

    private void Update() {

        // If the player hits the ESCAPE key, and the game is not in the main menu, and the pause menu exists—
        if (Input.GetKeyDown(KeyCode.Escape) && pauseScreen != null && !inMainMenu) {

            if (transcriptScreen.activeInHierarchy) {
                ToggleTranscriptLog();
            } else {

                // Toggles the pause menu
                TogglePauseMenu();
            }
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
            yield return new WaitForSeconds(0.03f);
        }

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

        yield return new WaitForSeconds(0.5f);

        GameStateManager.instance.ExitToMainMenu();

        // Waits for the transition to finish
        yield return new WaitForSeconds(transitionTime);

        // Reloads the scene (back to main menu)
        //SceneManager.LoadScene(0);

        // Sets the camera dolly to main menu spline track
        splineDolly.Spline = spline;

        // Sets the camera's look at point to the focal point object
        cinemachineCam.LookAt = cameraLookAtObject.transform;

        splineDolly.CameraPosition = 1;

        mainMenuUI.SetActive(true);

        transitionAnimator.SetBool("Play", false);
    }

    // Toggles pause menu
    public void TogglePauseMenu() {

        // UNPAUSE
        if (GameStateManager.Gamestate == GAMESTATE.PAUSED) {

            // Switches gamestate to playing
            GameStateManager.SetState(GAMESTATE.PLAYING);

            // Hides pause menu
            pauseScreen.SetActive(false);

            // Re-enables game screen
            gameScreen.SetActive(true);
        } 
        // PAUSE
        else if (GameStateManager.Gamestate == GAMESTATE.PLAYING){

            // Switches gamestate to paused
            GameStateManager.SetState(GAMESTATE.PAUSED);

            // Shows pause menu
            pauseScreen.SetActive(true);

            // Hides game screen
            gameScreen.SetActive(false);
        }

        settingsScreen.SetActive(false);
        transcriptScreen.SetActive(false);
    }

    // Toggles pause menu settings page
    public void ToggleSettingsScreen() {

        // Toggle
        settingsScreen.SetActive(!settingsScreen.activeInHierarchy);

        transcriptScreen.SetActive(false);
    }

    // Toggles pause menu settings page
    public void ToggleMMSettingsScreen() {

        // Toggle
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(!settingsScreen.activeInHierarchy);

        transcriptScreen.SetActive(false);
    }

    // Toggles transcript log screen
    public void ToggleTranscriptLog() {

        // Toggle
        transcriptScreen.SetActive(!transcriptScreen.activeInHierarchy);

        // replace with boolean

        settingsScreen.SetActive(false);
    }

    // Toggles credits menu
    public void ToggleCreditsMenu() {

        // SHOW
        if (!creditsScreen.activeInHierarchy) {

            creditsScreen.SetActive(true);

            transcriptScreen.SetActive(false);
            pauseScreen.SetActive(false);
            settingsScreen.SetActive(false);
        } 
        // HIDE
        else if (creditsScreen.activeInHierarchy) {

            creditsScreen.SetActive(false);

            transcriptScreen.SetActive(false);
            settingsScreen.SetActive(false);
            pauseScreen.SetActive(false);
        }
    }

    // Loads the main menu (called from button script assignment)
    public void LoadMainMenu() {
        GameStateManager.SetState(GAMESTATE.MAINMENU);
        GameStateManager.dialogueManager.StopAllCoroutines();

        // Reset canvas screens
        pauseScreen.SetActive(false);
        gameScreen.SetActive(false);
        GameStateManager.dialogueManager.demoOverScreen.SetActive(false);
        
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
        CarController.RadioPower = true;

        if (GameStateManager.dialogueManager.volumeProfile.TryGet<Vignette>(out var vignette)) {
            vignette.intensity.value = GameStateManager.dialogueManager.vignetteDefault;
        }
    }

    public void TogglePassengerSelect() {
        selectScreen.SetActive(!selectScreen.activeInHierarchy);
    }

    // Play button functionality (called from button script assignment)
    public void PlayButton() {

        // Reset stats
        passengerList.storyPassengers.Clear();
        passengerList.exhaustedStory.Clear();
        passengerList.storyPassengers = new(passengerList.backupList);
        ResetStats();

        InitiatePlay();
    }

    // Play button functionality (called from button script assignment)
    public void SelectMaxine() {

        // Reset stats
        passengerList.ResetListInOrder(passengerList.exhaustedStory, passengerList.storyPassengers);
        passengerList.storyPassengers.Clear();
        passengerList.storyPassengers.Add(passengerList.backupList.Find(x => x.id == 1)); // Adds Maxine only
        ResetStats();

        InitiatePlay();
    }

    // Play button functionality (called from button script assignment)
    public void SelectJulie() {

        // Reset stats
        passengerList.ResetListInOrder(passengerList.exhaustedStory, passengerList.storyPassengers);
        passengerList.storyPassengers.Clear();
        passengerList.storyPassengers.Add(passengerList.backupList.Find(x => x.id == 7)); // Adds Julie only
        ResetStats();

        InitiatePlay();
    }

    // Play button functionality (called from button script assignment)
    public void SelectDaniel() {

        // Reset stats
        passengerList.ResetListInOrder(passengerList.exhaustedStory, passengerList.storyPassengers);
        passengerList.storyPassengers.Clear();
        passengerList.storyPassengers.Add(passengerList.backupList.Find(x => x.id == 3)); // Adds Daniel only
        ResetStats();

        InitiatePlay();
    }

    // Play button functionality (called from button script assignment)
    public void SelectLucy() {

        // Reset stats
        passengerList.ResetListInOrder(passengerList.exhaustedStory, passengerList.storyPassengers);
        passengerList.storyPassengers.Clear();
        passengerList.storyPassengers.Add(passengerList.backupList.Find(x => x.id == 4)); // Adds Lucy only
        ResetStats();

        InitiatePlay();
    }

    // Play button functionality (called from button script assignment)
    public void SelectRomeo() {

        // Reset stats
        passengerList.ResetListInOrder(passengerList.exhaustedStory, passengerList.storyPassengers);
        passengerList.storyPassengers.Clear();
        passengerList.storyPassengers.Add(passengerList.backupList.Find(x => x.id == 6)); // Adds Romeo only
        ResetStats();

        InitiatePlay();
    }

    // Play button functionality (called from button script assignment)
    public void SelectQuinton() {

        // Reset stats
        passengerList.ResetListInOrder(passengerList.exhaustedStory, passengerList.storyPassengers);
        passengerList.storyPassengers.Clear();
        passengerList.storyPassengers.Add(passengerList.backupList.Find(x => x.id == 5)); // Adds Quinton only
        ResetStats();

        InitiatePlay();
    }

    // Play button functionality (called from button script assignment)
    public void SelectMcGee() {

        // Reset stats
        passengerList.ResetListInOrder(passengerList.exhaustedStory, passengerList.storyPassengers);
        passengerList.storyPassengers.Clear();
        passengerList.storyPassengers.Add(passengerList.backupList.Find(x => x.id == 2)); // Adds McGee and Maxine only
        passengerList.storyPassengers.Add(passengerList.backupList.Find(x => x.id == 1));
        ResetStats();

        InitiatePlay();
    }

    public void InitiatePlay() {

        // Enables game screen
        gameScreen.SetActive(true);

        // Generate day rides
        GameStateManager.car.GenerateDayRides(passengerList.storyPassengers.Count);

        // Start camera dolly motion
        StartCoroutine(StartDollyMovement());

        // Play engine sound
        GameStateManager.audioManager.PlaySoundByName("Engine");

        GameStateManager.car.carPointer.agent.enabled = true;
        GameStateManager.car.agent.enabled = true;
        
        // Start initial drive
        GameStateManager.car.StartInitialDrive();
    }

    // Quits the game (called from button script assignment)
    public void QuitButton() {
        Application.Quit();
        Debug.Log("QUIT!");
    }
}