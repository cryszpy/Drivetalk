using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    
    [Header("SCRIPT REFERENCES")] // --------------------------------------------------------------------------------
    
    [Tooltip("Reference to the car pointer's script component.")]
    public CarPointer carPointer;
    
    [Tooltip("Reference to the car's script component.")]
    public CarController car;

    [Tooltip("Reference to the name box.")]
    public GameObject nameBox;
    private TMP_Text nameBoxText;

    [Tooltip("Reference to the dialogue text element.")]
    public TMP_Text currentDialogueText;

    /* [Tooltip("Reference to the dialogue UI box object.")]
    public GameObject dash_dialogueBox;

    [Tooltip("Reference to the name box text element.")]
    public TMP_Text nameBoxText; */

    [Tooltip("Reference to the flashing indicator on the GPS.")]
    public GameObject gpsIndicator;

    [Tooltip("Reference to the transcript log object.")]
    public TranscriptLog transcriptLog;

    [SerializeField] private GameObject dialoguePivot;

    [SerializeField] private GameObject dialogueElement;

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

    private Toggle autoDialogueToggle;

    public GameObject currentElement;

    public List<DialogueLine> bruh = new();
    
    [Header("STATS")] // --------------------------------------------------------------------------------

    [Tooltip("Boolean flag; Checks if a sentence is being typed out.")]
    [HideInInspector] public bool typingSentence = false;

    [Tooltip("Boolean flag; Checks if the current dialogue piece is a choice branch.")]
    public bool playingChoices = false;

    [Tooltip("Static boolean flag; Checks whether automatic dialogue playing is enabled.")]
    private bool autoDialogue = false;

    [Tooltip("Boolean flag; Checks whether the passenger is waiting for the player to select a destination.")]
    [HideInInspector] public bool waitForRouting = false;

    private float expressionTimer = 0;
    private bool expressionTimerRunning = false;

    [HideInInspector] public bool startingExpressionDone = false;

    public int maxDialogueElements;

    public float punctuationWaitTime;

    public float shortPauseTime;

    public List<string> punctuationList = new();

    public Queue<GameObject> activeDialogueBlocks = new();
    [HideInInspector] public List<GameObject> activeDialogueTracker = new();

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

    private void Update() {
        bruh = lines.ToList();

        activeDialogueTracker = activeDialogueBlocks.ToList();

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
                    SwitchExpression(car.currentPassenger.expressions[0]);
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
    }

    // Assigns any missing script references
    public void FindReferences() {
        StopAllCoroutines();

        if (!car) {
            if (GameObject.FindGameObjectWithTag("Car").TryGetComponent<CarController>(out var carScript)) {
                car = carScript;
                carPointer = carScript.carPointer;
            } else {
                Debug.LogError("Could not find CarController component!");
            }
        }

        if (!dialoguePivot) {
            dialoguePivot = GameObject.FindGameObjectWithTag("DialoguePivot");
            Debug.LogWarning("DialoguePivot is null! Reassigned.");
        }

        if (!gpsIndicator) {
            gpsIndicator = GameObject.FindGameObjectWithTag("GPSIndicator");
        }
        gpsIndicator.SetActive(false);

        if (!transcriptLog) {
            transcriptLog = GameObject.FindGameObjectWithTag("MainCanvas").GetComponentInChildren<TranscriptLog>();
        }

        if (!autoDialogueToggle) {
            autoDialogueToggle = GameObject.FindGameObjectWithTag("AutoDialogueToggle").GetComponent<Toggle>();
            autoDialogueToggle.onValueChanged.AddListener(delegate { SetAutoDialogue(autoDialogueToggle.isOn); } );
            autoDialogue = true;
            autoDialogueToggle.gameObject.SetActive(false);
        }

        if (!nameBoxText) {

            if (!nameBox) {
                nameBox = GameObject.FindGameObjectWithTag("NameBox");
            }

            nameBoxText = nameBox.GetComponentInChildren<TMP_Text>();

            Debug.LogWarning("nameBox reference on: " + name + " is null! Reassigned.");
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

        // Set the current dialogue
        currentDialogue = dialogue;

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

        if (currentDialogue) {

            // If there aren't any sentences to display (reached the end of this dialogue piece)
            if (lines.Count <= 0) {

                // If there are choices attached to this dialogue piece—
                if (currentDialogue.choices.Length > 0) {
                    Debug.Log("1");

                    // Set boolean flag to currently playing a choice branch
                    playingChoices = true;

                    // Set the previous non-choice dialogue piece to jump back to after choices
                    if (preChoiceDialogue == null) {
                        preChoiceDialogue = currentDialogue;
                    }

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
                else if (currentDialogue.choices.Length == 0 && currentDialogue == car.currentPassenger.archetype.dropoffSalute) {
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

            if (!car.currentPassenger) {
                Debug.Log("your mom");
                return;
            }

            // Start passenger starting expression before talking
            if (!startingExpressionDone && lines.First().startingExpression != null) {

                // Switch expression to starting expression of the next line
                SwitchExpression(lines.First().startingExpression);
                return;
            }
            startingExpressionDone = false;

            // Removes previously said sentence from sentences queue
            DialogueLine line = lines.Dequeue();

            // Starts typing the queued sentence
            StartCoroutine(TypeSentence(line));
        } else {
            EndDialogue();
        }
    }

    // Shows the choices at a choice branch
    public void ShowChoices() {

        // Disables continue button
        //continueButton.SetActive(false);

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
        //dialogueAnimator.SetBool("Play", false);

        // Hides the skip indicator
        //skipIndicator.SetActive(false);

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

        // Affect mood meter with appropriate change
        GameStateManager.comfortManager.currentComfortability += choice.moodChange;

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

        // Start fading previous dialogue line element
        if (activeDialogueBlocks.Count >= maxDialogueElements) {
            GameObject deadBlock = activeDialogueBlocks.Dequeue();

            if (deadBlock.TryGetComponent<DialogueUIElement>(out var deadScript)) {

                // Starts destruction of dialogue block
                deadScript.animator.SetTrigger("Out");
            }
        }

        // Sets the currentLine stats
        currentLine.sentence = line.sentence;
        currentLine.expression = line.expression;
        currentLine.startingExpression = line.startingExpression;
        currentLine.requestsStart = line.requestsStart;
        currentLine.requestsEnd = line.requestsEnd;
        currentLine.audioToPlay = line.audioToPlay;
        currentLine.dashboardObject = line.dashboardObject;
        currentLine.earlyDropoff = line.earlyDropoff;
        currentLine.spawnDestination = line.spawnDestination;
        currentLine.voiceLine = line.voiceLine;
        currentLine.longPauseTime = line.longPauseTime;
        currentLine.firstNameUsage = line.firstNameUsage;

        // Wait for the appropriate amount of long pause time before continuing with this line
        yield return new WaitForSeconds(currentLine.longPauseTime);

        // Start voice line and short pause time
        if (currentLine.voiceLine) {
            StartCoroutine(StartVoiceLine());
        }

        // Set whether destination is ready to spawn
        if (currentLine.spawnDestination) {
            carPointer.readyToSpawnDest = true;
        }

        // Play audio file if there is one
        if (currentLine.audioToPlay != null) {
            GameStateManager.audioManager.PlaySoundByFile(currentLine.audioToPlay);

            yield return new WaitForSeconds(currentLine.audioToPlay.length);
        }

        // Spawn new dialogue element
        if (dialogueElement.TryGetComponent<DialogueUIElement>(out var blockScript)) {
            currentElement = (GameObject)blockScript.Create(dialogueElement, dialoguePivot.transform, car);
            currentElement.transform.SetAsFirstSibling();
        }

        activeDialogueBlocks.Enqueue(currentElement);

        DialogueUIElement dScript = null;

        // Set dialogue element text to the line's text
        if (currentElement.TryGetComponent<DialogueUIElement>(out var script)) {
            dScript = script;
            currentDialogueText = script.elementText;
        } else {
            Debug.LogError("Could not find DialogueUIElement component on this dialogue element!");
        }

        // Set appropriate expression before talking
        if (currentLine.expression != null) {
            SwitchExpression(currentLine.expression);
        }

        /* bool isFirstLine = false;

        // If this line is the first line said by the passenger—
        if (currentDialogue.lines.First().sentence == line.sentence && currentDialogue == car.currentPassenger.archetype.pickupGreeting) {

            // Set appropriate boolean flag
            isFirstLine = true;
        } */

        // Log appropriate name to transcript
        if (car.currentPassenger.nameRevealed) {
            transcriptLog.LogText(line.sentence, car.currentPassenger.passengerName);
        } else {
            transcriptLog.LogText(line.sentence, car.currentPassenger.hiddenName);
        }

        // If line is first line said by passenger—
        /* if (isFirstLine) {

            // Log appropriate name to transcript
            if (car.currentPassenger.nameRevealed) {
                transcriptLog.LogText(line.sentence, car.currentPassenger.passengerName);
            } else {
                transcriptLog.LogText(line.sentence, car.currentPassenger.hiddenName);
            }

        } else {

            // Log sentence without name
            transcriptLog.LogText(line.sentence, null);
        } */

        // Set whether dashboard requests are active or not and activate mood meter
        if (currentLine.requestsEnd) {
            GameStateManager.comfortManager.comfortabilityRunning = false;

            car.moodMeterAnimator.SetTrigger("FadeOut");
        }

        if (currentLine.requestsStart) {
            GameStateManager.comfortManager.comfortabilityRunning = true;

            car.moodMeterAnimator.SetTrigger("FadeIn");
        }

        // Spawn dashboard object if it exists
        if (currentLine.dashboardObject && currentLine.dashboardObject.TryGetComponent<Gift>(out var gift)) {

            switch (gift.location) {
                case GiftLocation.DASHBOARD:

                    if (car.dashboardGiftSpawns.Count > 0) {

                        GameObject spawnedObject = null;

                        // Iterate through all dashboard gift spawnpoints to check for first available
                        for (int i = 0; i < car.dashboardGiftSpawns.Count; i++){

                            if (!car.dashboardGiftSpawns[i].taken) {
                                spawnedObject = Instantiate(currentLine.dashboardObject, car.dashboardGiftSpawns[i].spawnPoint.transform);
                                car.dashboardGiftSpawns[i].taken = true;
                                break;
                            }
                        }

                        if (spawnedObject == null) {
                            Debug.LogWarning("Could not find available dashboard gift spawnpoint!");
                        }

                        break;
                    }
                    break;
                case GiftLocation.REARVIEW:

                    if (!car.rearviewGiftSpawn.taken) {
                        GameObject spawnedObject = Instantiate(currentLine.dashboardObject, car.rearviewGiftSpawn.spawnPoint.transform);
                        car.rearviewGiftSpawn.taken = true;
                        break;
                    } else {
                        Debug.LogWarning("Rearview gift spawn already taken!");
                        break;
                    }
            }
        }

        string message = null;

        string name = null;

        // Enable name box if hidden
        if (!nameBox.activeInHierarchy) {
            nameBox.SetActive(true);
        }

        // If the passenger has/not revealed their name, set their name and color accordingly
        if (currentLine.firstNameUsage) {
            name = car.currentPassenger.passengerName;

            Debug.Log("Set passenger name: " + car.currentPassenger.passengerName);

            car.currentPassenger.nameRevealed = true;

            // Sets name box color
            nameBoxText.color = car.currentPassenger.nameColor;

        } 
        else if (!car.currentPassenger.nameRevealed) {
            name = car.currentPassenger.hiddenName;
            nameBoxText.color = Color.white;
        } else if (car.currentPassenger.nameRevealed) {
            name = car.currentPassenger.passengerName;
            nameBoxText.color = car.currentPassenger.nameColor;
        }

        //name += ": ";

        // Separate everything but the name
        string concat = line.sentence;

        // String together message including name
        message = concat;

        Debug.Log(name + message);

        // Sets UI name
        nameBoxText.text = name;

        // Initializes empty text as full message to start typing
        currentDialogueText.text = message;
        currentDialogueText.maxVisibleCharacters = 0;

        // Indicates that a sentence is being typed out
        typingSentence = true;

        if (line.sentence != "...") {
            car.currentPassenger.animator.SetTrigger("Speak");
        }

        // For every character in ONLY THE MESSAGE WITHOUT THE NAME
        foreach (char letter in concat.ToCharArray()) {

            if (typingSentence) {

                // Types the character and adds it to the current sentence display
                currentDialogueText.maxVisibleCharacters++;

                // Wait after typing any sort of punctuation
                if (punctuationList.Contains(currentDialogueText.text.ToArray()[currentDialogueText.maxVisibleCharacters - 1].ToString())) {
                    yield return new WaitForSeconds(punctuationWaitTime);
                }

                // Waits for the typing speed time
                yield return new WaitForSeconds(car.currentPassenger.textCPS);

            } else {
                Debug.Log("Skipped typing!");
                break;
            }
        }

        typingSentence = false;

        if (dScript) {
            dScript.finished = true;
        }

        // Switches expression after done talking
        if (line.expression != null) {
            SwitchExpression(line.expression);
        } else if (currentDialogue.fallbackExpression != null) {
            SwitchExpression(currentDialogue.fallbackExpression);
        } else {

            // Switch to first expression in list
            SwitchExpression(car.currentPassenger.expressions[0]);
            Debug.LogWarning("Passenger line does not have an expression or a starting expression!");
        }

        // Initiate an early dropoff if needed
        if (currentLine.earlyDropoff) {
            DropoffDialogue();
            GameStateManager.dialogueManager.ResetDialogue();

            GameStateManager.EOnPassengerDropoff?.Invoke();
        }

        // Play the next dialogue line only if auto-dialogue is enabled, there isn't currently a starting expression playing, and
        // there is no voice line to be played. 
        if (autoDialogue && !startingExpressionDone && !currentLine.voiceLine) {

            // Starts countdown to fade dialogue away
            StartCoroutine(WaitBeforeNextSentence());
        }
    }

    private IEnumerator StartVoiceLine() {

        // Plays voice line
        GameStateManager.audioManager.PlayVoiceLine(currentLine.voiceLine, car.currentPassenger.gameObject);

        // Waits until after voice line is done, and the next sentence is ready to be said
        yield return new WaitForSeconds(currentLine.voiceLine.length + shortPauseTime);

        // Displays next sentence
        DisplayNextSentence();
    }

    private IEnumerator WaitBeforeNextSentence() {

        // Waits for the passenger's hold-dialogue-on-screen time
        yield return new WaitForSeconds(shortPauseTime);

        // Displays next sentence if available
        DisplayNextSentence();
    }

    // Starts the first ride dialogue
    public void StartRideDialogue() {
        Debug.Log("Greeting finished, moving onto general ride dialogue.");

        // Clears the current dialogue piece
        currentDialogue = null;

        // Gets the index number of the current passenger
        int index = CarController.PassengersDrivenIDs.IndexOf(car.currentPassenger.id);

        // Starts playing the appropriate ride number dialogue for the current passenger
        StartDialogue(car.currentPassenger.ridesDialogue[CarController.PassengersDrivenRideNum[index] - 1]);
    }

    // Called AFTER a passenger's dropoff dialogue has concluded
    public void DropoffDialogue() {
        Debug.Log("Finished dropoff dialogue piece!");

        // Clear current dialogue
        currentDialogue = null;

        // Unparent passenger from car
        car.currentPassenger.transform.parent = null;

        // Set passenger position to the destination stop
        car.currentPassenger.transform.position = car.dropoffPosition.transform.position;

        // Clear current passenger
        car.currentPassenger = null;

        // Reset finished dialogue boolean check
        carPointer.finishedDialogue = false;

        carPointer.readyToSpawnDest = false;

        // Allows car to continue driving
        car.arrivedAtDest = false;

        // If the player has completed the last ride of the day—
        if (car.currentRideNum >= car.totalRideNum) {

            Debug.Log("Completed day's shift!");
            // TODO: Put after-day summary here!

        }
    }

    // Ends dialogue and starts wait before next sentence group
    public void EndDialogue() {
        Debug.Log("Ended current ride dialogue!");

        // Indicate the end of the ride dialogue
        carPointer.finishedDialogue = true;

        carPointer.readyToSpawnDest = true;

        // Clear current dialogue
        currentDialogue = null;
    }

    public void ResetDialogue() {
        Debug.Log("Reset dialogue!");

        typingSentence = false;
        StopAllCoroutines();

        carPointer.finishedDialogue = true;
        carPointer.readyToSpawnDest = true;

        lines.Clear();

        currentDialogue = null;
        preChoiceDialogue = null;
    }

    public void DialogueGroupWait() {
        StopAllCoroutines();
        StartCoroutine(WaitBetweenDialogue());
    }

    // Waits in between dialogue blocks
    private IEnumerator WaitBetweenDialogue() {
        Debug.Log("Waiting for next dialogue piece!");

        // Generates a random amount of time to wait from minimum and maximum possible wait times for the current passenger
        float waitTime = UnityEngine.Random.Range(car.currentPassenger.longPauseMin, car.currentPassenger.longPauseMax);
        
        // Waits for the generated amount of time
        yield return new WaitForSeconds(waitTime);

        if (currentDialogue.nextDialogue) {

            // Clears pre-choice dialogue
            preChoiceDialogue = null;

            StartDialogue(currentDialogue.nextDialogue);
        } else {
            DisplayNextSentence();
        }
    }

    public void SwitchExpression(PassengerExpression expression) {

        expressionTimerRunning = expression.runExpressionTimer switch
        {
            true => true,
            false => false
        };

        car.currentPassenger.animator.SetTrigger(expression.animatorTrigger.ToString());
    }
}