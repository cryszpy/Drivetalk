using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum DialogueMode {
    DASH, RMM
}

public class DialogueManager : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    /* public DialogueMode mode; */
    
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

    /* public TMP_Text RMM_dialogueText;
    public GameObject RMM_dialogueBox; */

    //[SerializeField] private Animator choiceNotifAnimator;

    [Tooltip("Queued list of all sentences to say from current dialogue piece.")]
    public Queue<string> sentences;

    [Tooltip("Reference to the current dialogue piece.")]
    public DialoguePiece currentDialogue;

    [Tooltip("Reference to the dialogue piece directly before the most recent choices branch.")]
    private DialoguePiece preChoiceDialogue;

    [Tooltip("List of all choice buttons in a choice branch.")]
    [SerializeField] private List<GameObject> choiceButtonsList;

    /* public GameObject choiceNotif;

    [SerializeField] private RearviewMirror rearviewMirror; */
    
    [Header("STATS")]

    /* [SerializeField] private float choicesNotifSolid;
    [SerializeField] private float choicesNotifFlashing; */

    [Tooltip("Boolean flag; Checks if the current dialogue piece is a choice branch.")]
    public bool playingChoices = false;

    /* public bool timerPaused = false;
    public float choiceNotifTimer = 0;

    private bool interjected = false;
    private float dashRequestTimer = 0;
    public bool dashRequestRunning = false;
    public float dashTicker;
    public DashRequestRequirement currentDashReq; */

    private void Start() {

        // Initializes sentences queue
        sentences = new Queue<string>();

        // Assigns any missing script references
        FindReferences();
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
    
    // Assigns any missing script references
    private void FindReferences() {

        if (car == null) {
            if (GameObject.FindGameObjectWithTag("Car").TryGetComponent<CarController>(out var script)) {
                car = script;
                Debug.LogWarning("CarController component was null! Reassigned.");
            } else {
                Debug.LogError("Could not find CarController component!");
            }
        }

        if (dash_dialogueText == null) {
            if (GameObject.FindGameObjectWithTag("DialogueText").TryGetComponent<TMP_Text>(out var script)) {
                dash_dialogueText = script;
                Debug.LogWarning("Dialogue text TMP_Text component was null! Reassigned.");
            } else {
                Debug.LogError("Could not find dialogue text TMP_Text component!");
            }
        }

        if (dash_dialogueBox == null) {
            dash_dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox");
            Debug.LogWarning("Dash dialogue box GameObject was null! Reassigned.");
        }

        if (nameBoxText == null) {
            nameBoxText = GameObject.FindGameObjectWithTag("NameBox").GetComponentInChildren<TMP_Text>();
        }

        if (dialogueAnimator == null) {
            dialogueAnimator = dash_dialogueBox.GetComponent<Animator>();
            Debug.LogWarning("Dialogue box Animator component was null! Reassigned.");
        }
    }
    
    // Starts a piece of dialogue
    public void StartDialogue(DialoguePiece dialogue) {

        // Assigns any missing script references
        FindReferences();

        // Get this dialogue piece's index in passenger's dialogue list
        int index = car.currentPassenger.dialogue.IndexOf(dialogue);

        int nameIndex = car.currentPassenger.dialogue.FindIndex(x => x.firstNameUsage == true);
        Debug.Log(nameIndex);

        // Check whether this dialogue piece is before the passenger reveals their name or not
        bool nameCheck = index <= nameIndex;

        // If the passenger has/not revealed their name, set UI accordingly
        if (dialogue.firstNameUsage) {
            nameBoxText.text = car.currentPassenger.passengerName;
            Debug.Log("Set passenger name: " + car.currentPassenger.passengerName);
        } else if (nameCheck || nameIndex < 0) {
            nameBoxText.text = car.currentPassenger.hiddenName;
        }

        // Plays the "show dialogue" animation
        dialogueAnimator.SetBool("Play", true);

        // If the dialogue played is the start of a new dialogue block, reset the interjection limit
        /* if (car.currentPassenger.dialogue.Contains(dialogue)) {
            interjected = false;
        } */

        // Set the current dialogue
        currentDialogue = dialogue;

        //priorityDialogue.Remove(dialogue);

        // Clears any previous sentences before starting a new one
        sentences.Clear();

        /* if (!isInterjection && car.currentPassenger.dialogue.Contains(dialogue)) {
            car.currentPassenger.currentDialogueNum++;
            car.currentPassenger.dialogueLeftToFinish--;
        } */

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
        
        /* foreach (GameObject button in choiceButtonsList) {
            choiceButtonsList.Remove(button);
            //button.SetActive(false);
        } */

        // For every sentence in the current dialogue piece—
        foreach (string sentence in dialogue.sentences) {

            // Adds the sentence to the sentence queue list
            sentences.Enqueue(sentence);
        }

        // Displays the next sentence
        DisplayNextSentence();
    }

    // Displays the next sentence
    public void DisplayNextSentence() {

        /* if (dashRequestRunning) {
            return;
        } */
        
        // If there aren't any sentences to display (reached the end of this dialogue piece)
        if (sentences.Count <= 0) {

            // If there are choices attached to this dialogue piece—
            if (currentDialogue.choices.Length > 0) {

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

                // Set current dialogue piece to the pre-choice branch dialogue piece
                currentDialogue = preChoiceDialogue;

                // Clear the pre-choice branch dialogue piece variable
                preChoiceDialogue = null;

                // Waits, then plays the next dialogue piece
                StartCoroutine(WaitBetweenDialogue());
                return;
            }
            // If the passenger has said greeting, but not started main ride dialogue—
            else if (!car.currentPassenger.hasStartedRideDialogue) {

                // Set the passenger to have started the main ride dialogue
                car.currentPassenger.hasStartedRideDialogue = true;

                // Starts the main ride dialogue
                StartRideDialogue();
                return;
            }
            // Go to next regular dialogue piece if no choices
            else if (currentDialogue.nextDialogue) {

                // Waits, then plays the next dialogue piece
                StartCoroutine(WaitBetweenDialogue());
                return;
            }
            // Dropoff dialogue
            else if (currentDialogue.choices.Length == 0 && currentDialogue == car.currentPassenger.archetype.dropoffSalute) {

                // Starts the passenger's dropoff dialogue
                DropoffDialogue();
                return;
            }
            // End dialogue if no choices and nothing else to say
            else if (currentDialogue.choices.Length == 0) {

                // Ends the ride's dialogue
                EndDialogue();
                return;
            }
        }

        // Removes previously said sentence from sentences queue
        string sentence = sentences.Dequeue();

        // Stops any extra sentence coroutines that may be activated
        StopAllCoroutines();

        // Starts typing the queued sentence
        StartCoroutine(TypeSentence(sentence));
    }

    // Shows the choices at a choice branch
    public void ShowChoices() {

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

        // Plays the "show dialogue box" animation
        dialogueAnimator.SetBool("Play", false);

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
                    button.onClick.AddListener(() => AssignChoiceFollowUp(choice));
                }
            }
        }
    }

    // Function that tells each choice what to do when clicked
    public void AssignChoiceFollowUp(DialogueChoice choice) {

        // Switches from the "playing choices" state
        playingChoices = false;

        // If choice response exists, play it
        if (choice.nextDialogue != null) {
            StartDialogue(choice.nextDialogue);
        }
    }

    // Visually types the current sentence
    private IEnumerator TypeSentence(string sentence) {

        // PUT TRANSCRIPT LOG FUNCTIONALITY HERE

        Debug.Log(sentence);
        //RMM_dialogueText.text = "";

        // Initializes empty text to start typing
        dash_dialogueText.text = "";

        // For every character in the sentence
        foreach (char letter in sentence.ToCharArray()) {

            // Types the character and adds it to the current sentence display
            dash_dialogueText.text += letter;
            //RMM_dialogueText.text += letter;

            // Waits for the typing speed time
            yield return new WaitForSeconds(car.currentPassenger.textCPS);
        }

        // Waits for the passenger's hold-dialogue-on-screen time
        yield return new WaitForSeconds(car.currentPassenger.holdTime);

        // Displays next sentence if available
        DisplayNextSentence();
    }

    // Starts the first ride dialogue
    public void StartRideDialogue() {
        Debug.Log("Greeting finished, moving onto general ride dialogue.");

        // Plays the "show dialogue UI" animation
        dialogueAnimator.SetBool("Play", false);

        // Clears the current dialogue piece
        currentDialogue = null;

        // Gets the index number of the current passenger
        int index = CarController.PassengersDrivenIDs.IndexOf(car.currentPassenger.id);

        // Starts playing the appropriate ride number dialogue for the current passenger
        StartDialogue(car.currentPassenger.dialogue[CarController.PassengersDrivenRideNum[index] - 1]);
    }

    // Dropoff goodbye salute dialogue and dropoff of passenger
    public void DropoffDialogue() {
        Debug.Log("Dropped off passenger, and finished current dialogue piece!");

        // Hide dialogue box UI
        dialogueAnimator.SetBool("Play", false);

        // Clear current dialogue
        currentDialogue = null;
        
        // TODO: SET SPEECH BUBBLE TO FADE AWAY

        // Unparent passenger from car
        car.currentPassenger.transform.parent = null;

        // Set passenger position to the destination stop
        car.currentPassenger.transform.position = car.currentStop.transform.position;

        // Clear current taxi stop
        car.currentStop = null;

        // Clear current passenger
        car.currentPassenger = null;
    }

    // Ends dialogue and starts wait before next sentence group
    public void EndDialogue() {
        Debug.Log("Ended current ride dialogue!");

        // Hide dialogue box UI
        dialogueAnimator.SetBool("Play", false);

        // Clear current dialogue
        currentDialogue = null;
        
        // SET SPEECH BUBBLE TO FADE AWAY

        // Starts waiting
        //StartCoroutine(WaitBetweenDialogue());
    }

    // Waits in between dialogue blocks
    private IEnumerator WaitBetweenDialogue() {
        Debug.Log("Waiting for next dialogue piece!");

        // Plays the "hide dialogue UI box" animation
        dialogueAnimator.SetBool("Play", false);

        //currentDialogue = null;
        
        // SET SPEECH BUBBLE TO FADE AWAY

        Debug.Log("Waiting...");

        /* if (car.currentPassenger.currentRideNum == 0) {
            car.atTaxiStop = true;
        } */

        // Generates a random amount of time to wait from minimum and maximum possible wait times for the current passenger
        float waitTime = UnityEngine.Random.Range(car.currentPassenger.waitTimeMin, car.currentPassenger.waitTimeMax);
        
        // Waits for the generated amount of time
        yield return new WaitForSeconds(waitTime);

        // Starts the next piece of dialogue
        StartDialogue(currentDialogue.nextDialogue);

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
