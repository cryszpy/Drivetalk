using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum DialogueMode {
    DASH, RMM
}

public class DialogueManager : MonoBehaviour
{
    
    [Header("SCRIPT REFERENCES")]

    /* public DialogueMode mode; */
    public List<DashboardRequest> dashRequests = new();
    
    [Tooltip("Reference to the car pointer's script component.")]
    public CarPointer carPointer;
    
    [Tooltip("Reference to the car's script component.")]
    public CarController car;

    [Tooltip("Reference to the dialogue box's Animator component.")]
    [SerializeField] private Animator dialogueAnimator;

    [Tooltip("Reference to the dialogue text element.")]
    public TMP_Text dash_dialogueText;

    [Tooltip("Reference to the dialogue UI box object.")]
    public GameObject dash_dialogueBox;

    [Tooltip("Reference to the name box text element.")]
    public TMP_Text nameBoxText;

    [Tooltip("Reference to the flashing indicator on the GPS.")]
    public GameObject gpsIndicator;

    [Tooltip("Reference to the transcript log object.")]
    public TranscriptLog transcriptLog;

    [Tooltip("Reference to the continue button element.")]
    public GameObject continueButton;
    /* public TMP_Text RMM_dialogueText;
    public GameObject RMM_dialogueBox; */

    //[SerializeField] private Animator choiceNotifAnimator;

    [Tooltip("Queued list of all sentences to say from current dialogue piece.")]
    public Queue<DialogueLine> lines = new();

    [Tooltip("Reference to the current dialogue piece.")]
    public DialoguePiece currentDialogue;

    [Tooltip("The current line and expression from the current dialogue piece.")]
    public DialogueLine currentLine;

    [Tooltip("Reference to the dialogue piece directly before the most recent choices branch.")]
    public DialoguePiece preChoiceDialogue;

    [Tooltip("List of all choice buttons in a choice branch.")]
    [SerializeField] private List<GameObject> choiceButtonsList;

    [Tooltip("Reference to the skip dialogue indicator image.")]
    public GameObject skipIndicator;

    public Toggle autoDialogueToggle;

    public List<DialogueLine> bruh = new();
    /* public GameObject choiceNotif;

    [SerializeField] private RearviewMirror rearviewMirror; */
    
    [Header("STATS")]

    /* [SerializeField] private float choicesNotifSolid;
    [SerializeField] private float choicesNotifFlashing; */

    [Tooltip("Boolean flag; Checks if a sentence is being typed out.")]
    public bool typingSentence = false;

    [Tooltip("Boolean flag; Checks if the current dialogue piece is a choice branch.")]
    public bool playingChoices = false;

    [Tooltip("Static boolean flag; Checks whether automatic dialogue playing is enabled.")]
    public bool autoDialogue = false;

    [Tooltip("Boolean flag; Checks if the dialogue is waiting for a manual skip.")]
    public bool waitForSkip = false;

    [Tooltip("Boolean flag; Checks whether the passenger is waiting for the player to select a destination.")]
    public bool waitForRouting = false;

    public float expressionTimer = 0;
    public bool expressionTimerRunning = false;

    public bool startingExpressionDone = false;

    /* public bool timerPaused = false;
    public float choiceNotifTimer = 0;

    private bool interjected = false;
    private float dashRequestTimer = 0;
    
    public float dashTicker;
    public DashRequestRequirement currentDashReq; */

    private void OnEnable() {
        GameStateManager.EOnRideFinish += EndDialogue;
        GameStateManager.EOnDialogueGroupFinish += DialogueGroupWait;
        GameStateManager.EOnDestinationSet += TalkAfterRouting;
    }

    private void OnDisable() {
        GameStateManager.EOnRideFinish -= EndDialogue;
        GameStateManager.EOnDialogueGroupFinish -= DialogueGroupWait;
        GameStateManager.EOnDestinationSet -= TalkAfterRouting;
    }

    /* private void Update() {

        // Enables and disables all RMM dialogue UI stuff
        if (mode == DialogueMode.RMM && currentDialogue && !playingChoices) {
            rearviewMirror.backButton.SetActive(true);
            RMM_dialogueBox.SetActive(true);
            dash_dialogueBox.SetActive(false);
        } else if (mode == DialogueMode.DASH && currentDialogue && !playingChoices) {
            RMM_dialogueBox.SetActive(false);
            dash_dialogueBox.SetActive(true);
        } else {
            RMM_dialogueBox.SetActive(false);
            dash_dialogueBox.SetActive(false);
        }

        // Choice notification timer
        if (!timerPaused && playingChoices && mode != DialogueMode.RMM) {
            choiceNotifTimer += Time.deltaTime;
            choiceNotif.SetActive(true);

            // Auto-pick "..." option
            if(choiceNotifTimer > (car.currentPassenger.choiceNotifSolidTime + car.currentPassenger.choiceNotifFlashTime)) {
                playingChoices = false;
                timerPaused = false;
                //Debug.Log("Done" + choiceNotifTimer);
                choiceNotifAnimator.SetBool("Flash", false);
                choiceNotifTimer = 0;
                StartDialogue(currentDialogue.choices[^1].nextDialogue, false);
            } 
            // Switch to flashing
            else if (choiceNotifTimer > car.currentPassenger.choiceNotifSolidTime) {
                //Debug.Log("Flashing" + choiceNotifTimer);
                choiceNotifAnimator.SetBool("Flash", true);
            } 
            // Enable and set to solid
            else if (choiceNotif.activeInHierarchy && choiceNotifTimer <= car.currentPassenger.choiceNotifSolidTime) {
                //Debug.Log("Solid" + choiceNotifTimer);
                choiceNotifAnimator.SetBool("Flash", false);
            }
        }

        // Dash request timer
        if (dashRequestRunning) {
            dashRequestTimer += Time.deltaTime;

            // If the player follows the dash request
            if (CheckDashRequirements(currentDashReq)) {

                // Disables continued running of dash request
                dashRequestRunning = false;
                // PLUS AFFINITY
                Debug.Log("GAINED AFFINITY AT: " + dashTicker + "s");

                if (currentDialogue == null) {
                    // play dash control response archetype
                }
            }

            // Every x seconds, lose 1 affinity
            if (dashRequestTimer >= dashTicker) {
                // MINUS AFFINITY
                Debug.Log("LOST AFFINITY AT: " + dashTicker + "s");
                dashTicker += car.currentPassenger.dashRequestTickRate;
            }

            // Disables dash request when timer reaches limit
            if (dashRequestTimer > car.currentPassenger.dashRequestTime) {
                dashRequestRunning = false;
                dashRequestTimer = 0;
            }
        }
    } */

    // Checks for dash request requirements
    /* public bool CheckDashRequirements(DashRequestRequirement requirement) {
        return requirement.reqType switch
        {
            DashRequestType.AC_SETTING => CarController.Temperature == requirement.acSetting,
            DashRequestType.HORN => false,
            DashRequestType.RADIO_VOLUME => false,
            DashRequestType.RADIO_SONG => CarController.LastSongPlayedID == requirement.statToCheck,
            DashRequestType.CIGARETTE => false,
            _ => false,
        };
    } */

    /* // Entering rearview mirror mode
    public void EnterRMM() {

        // Sets the mode
        mode = DialogueMode.RMM;

        // Pauses choice notification timer (so it doesn't count down)
        timerPaused = true;

        // Enables back button
        rearviewMirror.backButton.SetActive(true);

        // Enables choice notification
        choiceNotif.SetActive(false);
    } */

    /* // Exiting rearview mirror mode
    public void ExitRMM() {

        // Sets the mode
        mode = DialogueMode.DASH;

        // Resumes choice notification timer (so it counts down)
        timerPaused = false;

        // Disables back button
        rearviewMirror.backButton.SetActive(false);

        // If choices are playing, enable choice notification and disable choices
        if (playingChoices) {
            choiceNotif.SetActive(true);
            car.choicesBar.SetActive(false);
        }
    } */

    private void Update() {
        bruh = lines.ToList();

        if (waitForRouting) {
            // TODO: Add silly passenger quips about not moving
        }

        // Switch default expressions
        if (expressionTimerRunning && car.currentPassenger) {
            expressionTimer += Time.deltaTime;

            if (expressionTimer > UnityEngine.Random.Range(car.currentPassenger.minSwitchTime, car.currentPassenger.maxSwitchTime) && !typingSentence) {
                expressionTimerRunning = false;
                expressionTimer = 0;

                float rand = UnityEngine.Random.value;

                if (rand < 0.5f) {
                    SwitchExpression(PassengerExpression.DEFAULT);
                } else {
                    expressionTimerRunning = true;
                }
            } else if (typingSentence) {
                expressionTimerRunning = false;
                expressionTimer = 0;
            }
        } else {
            expressionTimer = 0;
        }

        // Dash request timer
        if (dashRequests.Count > 0) {

            // For every dashboard request—
            foreach (var request in dashRequests) {

                // Check to see if it has been satisfied
                if (CheckDashRequirements(request)) {

                    // If it has, increase its added value to comfortability over time until it hits max
                    request.value += Time.deltaTime;
                } 
                // If it hasn't, decrease its added value to comfortability over time until it hits 0
                else {
                    request.value -= Time.deltaTime;
                }

                // Prevent dashboard request values from going infinitely
                if (request.value <= request.min) {
                    request.value = request.min;
                } else if (request.value >= request.max) {
                    request.value = request.max;
                }
            }
        }
    }

    public bool CheckDashRequirements(DashboardRequest request) {

        switch (request.control) {

            case DashboardControl.AC:
            
                switch (request.floatCheckType) {

                    case FloatCheckType.LESSER:
                        if (CarController.Temperature < request.floatValue) {
                            return true;
                        } else {
                            return false;
                        }
                    case FloatCheckType.GREATER:
                        if (CarController.Temperature > request.floatValue) {
                            return true;
                        } else {
                            return false;
                        }
                }
                return false;
            case DashboardControl.RADIO_SONG:
                if (CarController.CurrentRadioChannel == request.floatValue) {
                    return true;
                } else {
                    return false;
                }
            case DashboardControl.RADIO_VOLUME:
                if (CarController.RadioVolume == request.floatValue) {
                    return true;
                } else {
                    return false;
                }
            case DashboardControl.HAZARDS:
                return false;
            case DashboardControl.WIPERS:
                return false;
            case DashboardControl.HORN:
                return false;
            case DashboardControl.DEFOG:
                return false;
            case DashboardControl.HEADLIGHTS:
                return false;
            case DashboardControl.WINDOWS:
                return false;
            default:
                return false;
        }
    }

    public void ContinueButton() {

        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            // Skip typing on click
            if (currentLine.sentence != null && typingSentence && !autoDialogue) {
                dash_dialogueText.maxVisibleCharacters = dash_dialogueText.text.Length;
                typingSentence = false;
            } 
            // If sentence is typed out, play next sentence on click
            else if (waitForSkip) {
                waitForSkip = false;
                DisplayNextSentence();
            }
        }
    }
    
    // Assigns any missing script references
    public void FindReferences() {

        if (!car) {
            if (GameObject.FindGameObjectWithTag("Car").TryGetComponent<CarController>(out var carScript)) {
                car = carScript;
                carPointer = carScript.carPointer;
            } else {
                Debug.LogError("Could not find CarController component!");
            }
        }

        if (!dash_dialogueText) {
            if (GameObject.FindGameObjectWithTag("DialogueText").TryGetComponent<TMP_Text>(out var dialogueTextScript)) {
                dash_dialogueText = dialogueTextScript;
            } else {
                Debug.LogError("Could not find dialogue text TMP_Text component!");
            }
        }

        if (!continueButton) {
            continueButton = GameObject.FindGameObjectWithTag("ContinueButton");
            if (continueButton.TryGetComponent<Button>(out var button)) {
                button.onClick.AddListener(ContinueButton);
                Debug.Log("Assigned continueButton onClick event!");
            }
            Debug.Log("DialogueManager continueButton is null! Reassigned.");
        }

        if (!dash_dialogueBox) {
            dash_dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox");
        }

        if (!nameBoxText) {
            nameBoxText = GameObject.FindGameObjectWithTag("NameBox").GetComponentInChildren<TMP_Text>();
        }

        if (!gpsIndicator) {
            gpsIndicator = GameObject.FindGameObjectWithTag("GPSIndicator");
        }

        if (!dialogueAnimator) {
            dialogueAnimator = dash_dialogueBox.GetComponent<Animator>();
        }

        if (!skipIndicator) {
            skipIndicator = GameObject.FindGameObjectWithTag("SkipIndicator");
        }

        if (!transcriptLog) {
            transcriptLog = GameObject.FindGameObjectWithTag("MainCanvas").GetComponentInChildren<TranscriptLog>();
        }

        if (!autoDialogueToggle) {
            autoDialogueToggle = GameObject.FindGameObjectWithTag("AutoDialogueToggle").GetComponent<Toggle>();
            autoDialogueToggle.onValueChanged.AddListener(delegate { SetAutoDialogue(autoDialogueToggle.isOn); } );
        }
    }

    public void SetAutoDialogue(bool value) {
        autoDialogue = value;
    }

    public void TalkAfterRouting() {
        StartCoroutine(WaitAfterRouting());
    }

    private IEnumerator WaitAfterRouting() {

        // Wait for the passenger-specific time to start talking
        yield return new WaitForSeconds(car.currentPassenger.waitAfterRouteTime);

        Debug.Log("Started talking after setting destination!");

        // Set the passenger to have started the main ride dialogue
        car.currentPassenger.hasStartedRideDialogue = true;

        // Starts the main ride dialogue
        StartRideDialogue();
    }
    
    // Starts a piece of dialogue
    public void StartDialogue(DialoguePiece dialogue) {

        // Assigns any missing script references
        FindReferences();

        // Get this dialogue piece's index in passenger's dialogue list
        int index = car.currentPassenger.ridesDialogue.IndexOf(dialogue);

        int nameIndex = car.currentPassenger.ridesDialogue.FindIndex(x => x.firstNameUsage == true);

        // Check whether this dialogue piece is before the passenger reveals their name or not
        bool nameCheck = index <= nameIndex;

        // If the passenger has/not revealed their name, set UI accordingly
        if (dialogue.firstNameUsage) {
            nameBoxText.text = car.currentPassenger.passengerName;
            Debug.Log("Set passenger name: " + car.currentPassenger.passengerName);
        } else if (nameCheck || nameIndex < 0) {
            nameBoxText.text = car.currentPassenger.hiddenName;
        }

        // Hides the "show dialogue" animation
        dialogueAnimator.SetBool("Play", false);

        // If the dialogue played is the start of a new dialogue block, reset the interjection limit
        /* if (car.currentPassenger.dialogue.Contains(dialogue)) {
            interjected = false;
        } */

        // Set the current dialogue
        currentDialogue = dialogue;

        //priorityDialogue.Remove(dialogue);

        // Clears any previous sentences before starting a new one
        lines.Clear();

        // Remove existing buttons
        if (choiceButtonsList.Count > 0) {

            // For every active choice button—
            foreach (GameObject button in choiceButtonsList) {

                // Deletes the button
                Destroy(button);
            }

            // Clears choice buttons list
            choiceButtonsList.Clear();
        }

        // For every sentence in the current dialogue piece—
        foreach (var line in dialogue.lines) {

            // Adds the sentence to the sentence queue list
            lines.Enqueue(line);
        }

        DisplayNextSentence();
    }

    // Displays the next sentence
    public void DisplayNextSentence() {
        
        // If there aren't any sentences to display (reached the end of this dialogue piece)
        if (lines.Count <= 0) {

            // If there are choices attached to this dialogue piece—
            if (currentDialogue.choices.Length > 0) {
                Debug.Log("1");

                // Set boolean flag to currently playing a choice branch
                playingChoices = true;

                // Set the previous non-choice dialogue piece to jump back to after choices
                preChoiceDialogue = currentDialogue;

                /* if (mode == DialogueMode.RMM) {
                    ShowChoices();
                } */

                // Show choices
                ShowChoices();
                
                return;
            }
            // If just coming out of a post-choice dialogue, and pre-choice dialogue has more to say—
            else if (preChoiceDialogue != null && preChoiceDialogue.nextDialogue && !currentDialogue.nextDialogue) {
                Debug.Log("2");
                // Set current dialogue piece to the pre-choice branch dialogue piece
                currentDialogue = preChoiceDialogue;

                // Clear the pre-choice branch dialogue piece variable
                preChoiceDialogue = null;

                // Waits, then plays the next dialogue piece
                GameStateManager.EOnDialogueGroupFinish?.Invoke();
                return;
            }
            // If the passenger has said greeting, but not started main ride dialogue—
            else if (!car.currentPassenger.hasStartedRideDialogue) {
                Debug.Log("3");
                // TODO: Add dependence on selecting a GPS destination
                waitForRouting = true;

                // Show GPS indicator
                if (!gpsIndicator.activeInHierarchy) {
                    gpsIndicator.SetActive(true);
                }

                // Hide skip indicator if on
                if (skipIndicator.activeInHierarchy) {
                    skipIndicator.SetActive(false);
                }

                // Plays the "hide dialogue UI box" animation
                dialogueAnimator.SetBool("Play", false);

                return;
            }
            // Go to next regular dialogue piece if no choices
            else if (currentDialogue.nextDialogue) {
                Debug.Log("4");
                // Waits, then plays the next dialogue piece
                GameStateManager.EOnDialogueGroupFinish?.Invoke();
                return;
            }
            // Dropoff dialogue
            else if (currentDialogue.choices.Length == 0 && currentDialogue == car.currentPassenger.archetype.dropoffSalute && carPointer.finishedDialogue) {
                Debug.Log("5");
                // Starts the passenger's dropoff dialogue
                DropoffDialogue();
                return;
            }
            // End dialogue if no choices and nothing else to say
            else if (currentDialogue.choices.Length == 0) {
                Debug.Log("6");
                // Ends the ride's dialogue
                GameStateManager.EOnRideFinish?.Invoke();
                return;
            }
        } 
        // If this dialogue piece has a dashboard request to play—
        else if (currentDialogue.request.control != DashboardControl.NONE && !dashRequests.Contains(currentDialogue.request)) {

            // Reset response boolean
            currentDialogue.request.hasResponded = false;

            // Add the request to the list of active requests
            dashRequests.Add(currentDialogue.request);
            Debug.Log("Started dashboard request of type: " + currentDialogue.request.control);

            if (CheckDashRequirements(currentDialogue.request)) {
                Debug.Log("Dashboard request has been pre-completed!");

                dialogueAnimator.SetBool("Play", false);

                currentDialogue.request.hasResponded = true;
                preChoiceDialogue = currentDialogue;
                currentDialogue = currentDialogue.request.preCompletedResponse;

                if (currentDialogue.lines[0].startingExpression != PassengerExpression.NONE) {

                    SwitchExpression(currentDialogue.lines[0].startingExpression);
                } else {
                    
                    // Starts the pre-completed request response
                    StartDialogue(currentDialogue);
                }
                return;
            }
        }

        // Start passenger starting expression before talking
        if (!startingExpressionDone && lines.First().startingExpression != PassengerExpression.NONE) {

            // Hide skip indicator and dialogue box
            skipIndicator.SetActive(false);
            dialogueAnimator.SetBool("Play", false);

            // Switch expression to starting expression of the next line
            SwitchExpression(lines.First().startingExpression);
            return;
        }
        startingExpressionDone = false;

        // Removes previously said sentence from sentences queue
        DialogueLine line = lines.Dequeue();

        // Stops any extra sentence coroutines that may be activated
        //StopAllCoroutines();

        // Starts typing the queued sentence
        StartCoroutine(TypeSentence(line));
    }

    // Shows the choices at a choice branch
    public void ShowChoices() {

        // Disables continue button
        continueButton.SetActive(false);

        // Remove existing buttons
        if (choiceButtonsList.Count > 0) {
            // For every active choice button—
            foreach (GameObject button in choiceButtonsList) {

                // Deletes the button
                Destroy(button);
            }

            // Clears choice buttons list
            choiceButtonsList.Clear();
        }

        // Hides the "show dialogue box" animation
        dialogueAnimator.SetBool("Play", false);

        // Hides the skip indicator
        skipIndicator.SetActive(false);

        // Enables the choices bar in preparation for displaying choice buttons
        car.choicesBar.SetActive(true);

        // If there are choices to be displayed—
        if (currentDialogue) {

            // For every choice in the current list—
            foreach (DialogueChoice choice in currentDialogue.choices) {

                // Create button in UI
                TMP_Text choiceText = Instantiate(car.choicePrefab, car.choicesBar.transform).GetComponentInChildren<TMP_Text>();

                // Set UI button text to the choice text
                choiceText.text = choice.choiceText;

                // Add button object to a list for future destruction
                choiceButtonsList.Add(choiceText.transform.parent.gameObject);

                // Add on-click button action (triggers certain dialogue)
                if (choiceText.transform.parent.TryGetComponent<Button>(out var button)) {
                    button.onClick.AddListener(() => ChoiceFollowUp(choice));
                }
            }
        }
    }

    // Function that tells each choice what to do when clicked
    public void ChoiceFollowUp(DialogueChoice choice) {

        // Switches from the "playing choices" state
        playingChoices = false;

        // Enables the continue button
        continueButton.SetActive(true);

        // If choice response exists, play it
        if (choice.lines.Length > 0) {
            StartDialogue(choice);
        } 
        // Else, go to next dialogue piece
        else if (currentDialogue.nextDialogue) {
            StartDialogue(currentDialogue.nextDialogue);
        }
    }

    // Visually types the current sentence
    public IEnumerator TypeSentence(DialogueLine line) {

        currentLine.sentence = line.sentence;
        currentLine.expression = line.expression;
        currentLine.startingExpression = line.startingExpression;

        // Set appropriate expression before talking
        if (currentLine.expression != PassengerExpression.NONE) {
            SwitchExpression(currentLine.expression);
        }

        skipIndicator.SetActive(false);

        Debug.Log(line.sentence);

        transcriptLog.LogText(line.sentence, car.currentPassenger.passengerName);

        // Plays the "show dialogue UI" animation
        dialogueAnimator.SetBool("Play", true);

        //RMM_dialogueText.text = "";

        // Initializes empty text to start typing
        dash_dialogueText.text = line.sentence;
        dash_dialogueText.maxVisibleCharacters = 0;

        // Indicates that a sentence is being typed out
        typingSentence = true;

        if (line.sentence != "...") {
            car.currentPassenger.animator.SetTrigger("Speak");
        }

        // For every character in the sentence
        foreach (char letter in line.sentence.ToCharArray()) {

            if (typingSentence) {

                // Types the character and adds it to the current sentence display
                dash_dialogueText.maxVisibleCharacters++;
                //RMM_dialogueText.text += letter;

                // Waits for the typing speed time
                yield return new WaitForSeconds(car.currentPassenger.textCPS);

            } else {
                Debug.Log("Skipped typing!");
                break;
            }
        }

        typingSentence = false;

        // Switches expression after done talking
        if (line.expression != PassengerExpression.NONE) {
            SwitchExpression(line.expression);
        } else if (currentDialogue.fallbackExpression != PassengerExpression.NONE) {
            SwitchExpression(currentDialogue.fallbackExpression);
        } else {
            SwitchExpression(PassengerExpression.DEFAULT);
            Debug.LogWarning("Passenger line does not have an expression or a starting expression!");
        }

        if (autoDialogue) {

            // Waits for the passenger's hold-dialogue-on-screen time
            yield return new WaitForSeconds(car.currentPassenger.holdTime);

            // Displays next sentence if available
            DisplayNextSentence();
        } else {

            // Switches continue button functionality in ContinueButton()
            waitForSkip = true;

            // Enables the skip indicator
            skipIndicator.SetActive(true);
        }
    }

    // Starts the first ride dialogue
    public void StartRideDialogue() {
        Debug.Log("Greeting finished, moving onto general ride dialogue.");

        // Plays the "hide dialogue UI" animation
        dialogueAnimator.SetBool("Play", false);

        // Clears the current dialogue piece
        currentDialogue = null;

        // Gets the index number of the current passenger
        int index = CarController.PassengersDrivenIDs.IndexOf(car.currentPassenger.id);

        // Starts playing the appropriate ride number dialogue for the current passenger
        StartDialogue(car.currentPassenger.ridesDialogue[CarController.PassengersDrivenRideNum[index] - 1]);
    }

    // Dropoff goodbye salute dialogue and dropoff of passenger
    public void DropoffDialogue() {
        Debug.Log("Finished dropoff dialogue piece!");

        // Hide skip indicator if on
        if (skipIndicator.activeInHierarchy) {
            skipIndicator.SetActive(false);
        }

        // Hide dialogue box UI
        dialogueAnimator.SetBool("Play", false);

        // Clear current dialogue
        currentDialogue = null;

        // Unparent passenger from car
        car.currentPassenger.transform.parent = null;

        // Set passenger position to the destination stop
        car.currentPassenger.transform.position = car.currentStop.transform.position;

        // Clear current taxi stop
        car.currentStop = null;

        // Clear current passenger
        car.currentPassenger = null;

        // Reset finished dialogue boolean check
        carPointer.finishedDialogue = false;

        // If the player has completed the last ride of the day—
        if (car.currentRideNum >= car.totalRideNum) {

            Debug.Log("Completed day's shift!");
            // TODO: Put after-day summary here!

        } else {

            // Route the car to the nearest taxi stop to pick up another passenger
            if (!car.currentPassenger) {
                car.FindNearestStop();
            }
        }
    }

    // Ends dialogue and starts wait before next sentence group
    public void EndDialogue() {
        Debug.Log("Ended current ride dialogue!");

        // Indicate the end of the ride dialogue
        carPointer.finishedDialogue = true;

        // If driving around aimlessly, start driving towards destination
        if (!car.atTaxiStop && car.carPointer.destinationObject.CompareTag("Block")) {
            carPointer.SwitchToFinalDestination();
        }

        // Hide skip indicator if on
        if (skipIndicator.activeInHierarchy) {
            skipIndicator.SetActive(false);
        }

        // Hide dialogue box UI
        dialogueAnimator.SetBool("Play", false);

        // Clear current dialogue
        currentDialogue = null;
    }

    public void DialogueGroupWait() {
        StopAllCoroutines();
        StartCoroutine(WaitBetweenDialogue());
    }

    // Waits in between dialogue blocks
    private IEnumerator WaitBetweenDialogue() {
        Debug.Log("Waiting for next dialogue piece!");

        // Hide skip indicator if on
        if (skipIndicator.activeInHierarchy) {
            skipIndicator.SetActive(false);
        }

        // Plays the "hide dialogue UI box" animation
        dialogueAnimator.SetBool("Play", false);

        // Generates a random amount of time to wait from minimum and maximum possible wait times for the current passenger
        float waitTime = UnityEngine.Random.Range(car.currentPassenger.waitTimeMin, car.currentPassenger.waitTimeMax);
        
        // Waits for the generated amount of time
        yield return new WaitForSeconds(waitTime);

        // Set completed dash request response as current dialogue, save previous dialogue to continue after
        if (dashRequests.Count > 0 && CheckDashRequirements(dashRequests[^1]) && !dashRequests[^1].hasResponded) {
            dashRequests[^1].hasResponded = true;
            preChoiceDialogue = currentDialogue;
            currentDialogue = dashRequests[^1].completedResponse;

            if (currentLine.startingExpression != PassengerExpression.NONE) {

                SwitchExpression(currentLine.startingExpression);
            } else {

                // Starts the completed request response
                StartDialogue(currentDialogue);
            }
            yield break;
        }

        if (currentDialogue.nextDialogue) {

            // Clears pre-choice dialogue
            preChoiceDialogue = null;

            StartDialogue(currentDialogue.nextDialogue);
        } else {
            DisplayNextSentence();
        }
        
        // If an interjection hasn't already played, and passenger narrative dialogue isn't exhausted—
        /* if (!interjected && !(car.currentPassenger.dialogueLeftToFinish == car.currentPassenger.dialogue.Count)) {

            float rand = UnityEngine.Random.value;

            // If rolling for an interjection succeeds, then interject
            if (rand <= car.currentPassenger.interjectionChance && car.currentPassenger.dialogueLeftToFinish > 0) {
                Debug.Log("Interjection success!");
                Interject();
            }
            // Else, continue with dialogue story
            else {
                Debug.Log("Interjection failed!");
                StartDialogue(car.currentPassenger.dialogue[car.currentPassenger.currentDialogueNum], false);
            }
        } else {
            Debug.Log("Already interjected once this break, or this is the pickup dialogue greeting!");
            StartDialogue(car.currentPassenger.dialogue[car.currentPassenger.currentDialogueNum], false);
        } */
    }

    public void SwitchExpression(PassengerExpression expression) {

        switch (expression) {
            case PassengerExpression.DEFAULT:
                expressionTimerRunning = true;
                car.currentPassenger.animator.SetTrigger("Default");
                break;
            case PassengerExpression.SIDE_DEFAULT:
                expressionTimerRunning = true;
                car.currentPassenger.animator.SetTrigger("SideDefault");
                break;
            case PassengerExpression.POUTY:
                expressionTimerRunning = false;
                car.currentPassenger.animator.SetTrigger("Pouty");
                break;
            case PassengerExpression.CLOSE_EYED_SMILE:
                Debug.Log("YAYY");
                expressionTimerRunning = false;
                car.currentPassenger.animator.SetTrigger("CESmile");
                break;
            case PassengerExpression.LAUGH_HEARTY:
                expressionTimerRunning = false;
                car.currentPassenger.animator.SetTrigger("LaughHearty");
                break;
            case PassengerExpression.LAUGH_FLOWY:
                expressionTimerRunning = false;
                car.currentPassenger.animator.SetTrigger("LaughFlowy");
                break;
            default:
                expressionTimerRunning = true;
                car.currentPassenger.animator.SetTrigger("Default");
                break;
        }
    }

    /* private void Interject() {
        interjected = true;

        List<DialoguePiece> smallTalkList = new();
        List<DialoguePiece> dashRequestList = new();
        
        AddInterjections(smallTalkList, dashRequestList);

        // If all small talk dialogue has already been played, reset and choose again
        if (smallTalkList.Count <= 0) {
            ResetInterjectionType(InterjectionType.SMALL_TALK);
            AddInterjections(smallTalkList, dashRequestList);
        }
        
        // If all dash request dialogue has already been played, reset and choose again
        if (dashRequestList.Count <= 0) {
            ResetInterjectionType(InterjectionType.DASH_REQUEST);
            AddInterjections(smallTalkList, dashRequestList);
        }

        float rand2 = UnityEngine.Random.value;

        // Small talk
        if (rand2 <= car.currentPassenger.interjectionPreferenceThreshold) {
            int rand3 = UnityEngine.Random.Range(0, smallTalkList.Count - 1);

            DialoguePiece chosen = smallTalkList[rand3];
            chosen.seen = true;

            StartDialogue(chosen, true);
        } 
        // Dash request
        else {
            int rand3 = UnityEngine.Random.Range(0, dashRequestList.Count - 1);

            DialoguePiece chosen = dashRequestList[rand3];
            chosen.seen = true;

            StartDialogue(chosen, true);
            dashRequestRunning = true; // CHECK IF REQUEST IS ALREADY FULFILLED THEN SWITCH
            currentDashReq = chosen.dashRequirement;
        }
    }

    private void AddInterjections(List<DialoguePiece> smallTalkList, List<DialoguePiece> dashAdjustList) {

        foreach (DialoguePiece interjection in car.currentPassenger.interjections) {

            if (!interjection.seen) {

                switch (interjection.interjectionType) {
                    case InterjectionType.SMALL_TALK:
                        smallTalkList.Add(interjection);
                        break;
                    case InterjectionType.DASH_REQUEST:
                        dashAdjustList.Add(interjection);
                        break;
                }
            }
        }
    }

    private void ResetInterjectionType(InterjectionType type) { 

        foreach (DialoguePiece piece in car.currentPassenger.interjections){
            if (piece.interjectionType == type) {
                piece.seen = false;
            }
        }
    } */
}
