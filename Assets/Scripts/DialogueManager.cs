using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogueMode {
    DASH, RMM
}

public class DialogueManager : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    /* public DialogueMode mode; */
    
    public CarController car;

    [SerializeField] private Animator dialogueAnimator;
    public TMP_Text dash_dialogueText;
    public GameObject dash_dialogueBox;
    /* public TMP_Text RMM_dialogueText;
    public GameObject RMM_dialogueBox; */

    //[SerializeField] private Animator choiceNotifAnimator;

    public Queue<string> sentences;

    public DialoguePiece currentDialogue;

    [SerializeField] private List<GameObject> choiceButtonsList;

    /* public GameObject choiceNotif;

    [SerializeField] private RearviewMirror rearviewMirror; */
    
    [Header("STATS")]

    /* [SerializeField] private float choicesNotifSolid;
    [SerializeField] private float choicesNotifFlashing; */

    public bool playingChoices = false;

    /* public bool timerPaused = false;
    public float choiceNotifTimer = 0;

    private bool interjected = false;
    private float dashRequestTimer = 0;
    public bool dashRequestRunning = false;
    public float dashTicker;
    public DashRequestRequirement currentDashReq; */

    private void Start() {
        sentences = new Queue<string>();
        FindReferences();
    }

    private void Update() {

        // Enables and disables all RMM dialogue UI stuff
        /* if (mode == DialogueMode.RMM && currentDialogue && !playingChoices) {
            rearviewMirror.backButton.SetActive(true);
            RMM_dialogueBox.SetActive(true);
            dash_dialogueBox.SetActive(false);
        } else if (mode == DialogueMode.DASH && currentDialogue && !playingChoices) {
            RMM_dialogueBox.SetActive(false);
            dash_dialogueBox.SetActive(true);
        } else {
            RMM_dialogueBox.SetActive(false);
            dash_dialogueBox.SetActive(false);
        } */

        // Choice notification timer
        /* if (!timerPaused && playingChoices && mode != DialogueMode.RMM) {
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
        } */

        // Dash request timer
        /* if (dashRequestRunning) {
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
        } */
    }

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

    // Entering rearview mirror mode
    /* public void EnterRMM() {

        // Sets the mode
        mode = DialogueMode.RMM;

        // Pauses choice notification timer (so it doesn't count down)
        timerPaused = true;

        // Enables back button
        rearviewMirror.backButton.SetActive(true);

        // Enables choice notification
        choiceNotif.SetActive(false);
    }

    // Exiting rearview mirror mode
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
    }
 */
    
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

        if (dialogueAnimator == null) {
            dialogueAnimator = dash_dialogueBox.GetComponent<Animator>();
            Debug.LogWarning("Dialogue box Animator component was null! Reassigned.");
        }

    }
    
    public void StartDialogue(DialoguePiece dialogue, bool isInterjection) {

        FindReferences();

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

        if (!isInterjection && car.currentPassenger.dialogue.Contains(dialogue)) {
            car.currentPassenger.currentDialogueNum++;
            car.currentPassenger.dialogueLeftToFinish--;
        }

        // Remove existing buttons
        if (choiceButtonsList.Count > 0) {
            foreach (GameObject button in choiceButtonsList) {
                Destroy(button);
            }
            choiceButtonsList.Clear();
        }
        
        /* foreach (GameObject button in choiceButtonsList) {
            choiceButtonsList.Remove(button);
            //button.SetActive(false);
        } */

        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {

        /* if (dashRequestRunning) {
            return;
        } */
        // Display choices
        if (sentences.Count == 0 && currentDialogue.choices.Length > 0) {

            playingChoices = true;

            /* if (mode == DialogueMode.RMM) {
                ShowChoices();
            } */
            ShowChoices();
            
            return;
        }
        // Dropoff dialogue
        else if (sentences.Count == 0 && currentDialogue.choices.Length == 0 && currentDialogue == car.currentPassenger.archetype.dropoffSalute) {
            DropoffDialogue();
            return;
        }
        // End dialogue if no choices and nothing else to say
        else if (sentences.Count == 0 && currentDialogue.choices.Length == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        StopAllCoroutines();

        StartCoroutine(TypeSentence(sentence));
    }

    public void ShowChoices() {

        // Remove existing buttons
        if (choiceButtonsList.Count > 0) {
            foreach (GameObject button in choiceButtonsList) {
                Destroy(button);
            }
            choiceButtonsList.Clear();
        }

        dialogueAnimator.SetBool("Play", false);

        car.choicesBar.SetActive(true);

        // If there are choices to be displayed—
        if (currentDialogue) {

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

    public void AssignChoiceFollowUp(DialogueChoice choice) {
        playingChoices = false;

        // If choice response exists, play it
        if (choice.nextDialogue != null) {
            StartDialogue(choice.nextDialogue, false);
        }
    }

    private IEnumerator TypeSentence(string sentence) {

        // PUT TRANSCRIPT LOG FUNCTIONALITY HERE

        Debug.Log(sentence);
        //RMM_dialogueText.text = "";
        dash_dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray()) {
            dash_dialogueText.text += letter;
            //RMM_dialogueText.text += letter;

            yield return new WaitForSeconds(car.currentPassenger.textCPS);
        }

        yield return new WaitForSeconds(car.currentPassenger.holdTime);

        DisplayNextSentence();
    }

    // Dropoff goodbye salute dialogue and dropoff of passenger
    public void DropoffDialogue() {
        Debug.Log("Dropped off passenger, and finished current dialogue piece!");

        dialogueAnimator.SetBool("Play", false);

        currentDialogue = null;
        
        // SET SPEECH BUBBLE TO FADE AWAY

        // Unparent passenger from car
        car.currentPassenger.transform.parent = null;

        // Set passenger position to the destination stop
        car.currentPassenger.transform.position = car.currentStop.transform.position;

        car.currentStop = null;

        car.currentPassenger = null;
    }

    // Ends dialogue and starts wait before next sentence group
    public void EndDialogue() {
        Debug.Log("Ended current dialogue piece!");

        dialogueAnimator.SetBool("Play", false);

        currentDialogue = null;
        
        // SET SPEECH BUBBLE TO FADE AWAY

        // Starts waiting
        if (car.currentPassenger.dialogueLeftToFinish > 0) {
            StartCoroutine(WaitBetweenDialogue());
        }
    }

    private IEnumerator WaitBetweenDialogue() {
        Debug.Log("Waiting...");

        if (car.currentPassenger.currentDialogueNum == 0) {
            car.atTaxiStop = true;
        }

        float waitTime = UnityEngine.Random.Range(car.currentPassenger.waitTimeMin, car.currentPassenger.waitTimeMax);
        
        yield return new WaitForSeconds(waitTime);

        StartDialogue(car.currentPassenger.dialogue[car.currentPassenger.currentDialogueNum], false);

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
