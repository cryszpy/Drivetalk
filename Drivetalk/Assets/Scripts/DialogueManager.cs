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

    public DialogueMode mode;
    
    public CarController car;

    public TMP_Text dash_dialogueText;
    public GameObject dash_dialogueBox;
    public TMP_Text RMM_dialogueText;
    public GameObject RMM_dialogueBox;

    [SerializeField] private Animator choiceNotifAnimator;

    public Queue<string> sentences;

    public DialoguePiece currentDialogue;

    [SerializeField] private List<GameObject> choiceButtonsList;

    public GameObject choiceNotif;

    [SerializeField] private RearviewMirror rearviewMirror;
    
    [Header("STATS")]
    [SerializeField] private float textCPS;

    [SerializeField] private float choicesNotifSolid;
    [SerializeField] private float choicesNotifFlashing;

    public bool playingChoices = false;

    public bool timerPaused = false;
    public float choiceNotifTimer = 0;
    private bool interjected = false;

    private void Start() {
        sentences = new Queue<string>();
    }

    private void Update() {

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
    }

    public void EnterRMM() {
        mode = DialogueMode.RMM;
        timerPaused = true;
        rearviewMirror.backButton.SetActive(true);
        choiceNotif.SetActive(false);
    }

    public void ExitRMM() {
        mode = DialogueMode.DASH;
        timerPaused = false;
        rearviewMirror.backButton.SetActive(false);

        if (playingChoices) {
            choiceNotif.SetActive(true);
            car.choicesBar.SetActive(false);
        }
    }

    public void StartDialogue(DialoguePiece dialogue, bool isInterjection) {

        // If the dialogue played is the start of a new dialogue block, reset the interjection limit
        if (car.currentPassenger.dialogue.Contains(dialogue)) {
            interjected = false;
        }

        currentDialogue = dialogue;

        //priorityDialogue.Remove(dialogue);

        sentences.Clear();

        if (!isInterjection && car.currentPassenger.dialogue.Contains(dialogue)) {
            car.currentPassenger.currentDialogueNum++;
            car.currentPassenger.dialogueLeftToFinish--;
        }

        // Remove existing buttons
        foreach (GameObject button in choiceButtonsList) {
            button.SetActive(false);
        }

        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {

        // Display choices
        if (sentences.Count == 0 && currentDialogue.choices.Length > 0) {

            playingChoices = true;

            if (mode == DialogueMode.RMM) {
                ShowChoices();
            }
            
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
        Debug.Log(sentence);
        RMM_dialogueText.text = "";
        dash_dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray()) {
            dash_dialogueText.text += letter;
            RMM_dialogueText.text += letter;

            yield return new WaitForSeconds(textCPS);
        }

        yield return new WaitForSeconds(car.currentPassenger.holdTime);

        DisplayNextSentence();
    }

    public void EndDialogue() {
        Debug.Log("Ended current dialogue piece!");

        currentDialogue = null;
        
        // SET SPEECH BUBBLE TO FADE AWAY

        if (car.currentPassenger.dialogueLeftToFinish > 0) {
            StartCoroutine(WaitBetweenDialogue());
        }
    }

    private IEnumerator WaitBetweenDialogue() {
        Debug.Log("Waiting...");

        float waitTime = UnityEngine.Random.Range(car.currentPassenger.waitTimeMin, car.currentPassenger.waitTimeMax);
        
        yield return new WaitForSeconds(waitTime);

        // INTERJECTIONS
        if (!interjected) {

            float rand = UnityEngine.Random.value;

            if (rand <= car.currentPassenger.interjectionChance && car.currentPassenger.dialogueLeftToFinish > 0) {
                Debug.Log("Interjection success!");
                Interject();
            } else {
                Debug.Log("Interjection failed!");
                StartDialogue(car.currentPassenger.dialogue[car.currentPassenger.currentDialogueNum], false);
            }
        } else {
            Debug.Log("Already interjected once this break!");
            StartDialogue(car.currentPassenger.dialogue[car.currentPassenger.currentDialogueNum], false);
        }
    }

    private void Interject() {
        interjected = true;

        List<DialoguePiece> smallTalkList = new();
        List<DialoguePiece> dashAdjustlist = new();
        
        AddInterjections(smallTalkList, dashAdjustlist);

        // If all interjection dialogue has already been played, reset and choose again
        if (smallTalkList.Count <= 0) {
            ResetInterjectionType(InterjectionType.SMALL_TALK);
            AddInterjections(smallTalkList, dashAdjustlist);
        }
        
        if (dashAdjustlist.Count <= 0) {
            ResetInterjectionType(InterjectionType.DASH_ADJUST);
            AddInterjections(smallTalkList, dashAdjustlist);
        }

        float rand2 = UnityEngine.Random.value;

        if (rand2 <= car.currentPassenger.interjectionPreferenceThreshold) {
            int rand3 = UnityEngine.Random.Range(0, smallTalkList.Count - 1);

            DialoguePiece chosen = smallTalkList[rand3];
            chosen.seen = true;

            StartDialogue(chosen, true);
        } else {
            int rand3 = UnityEngine.Random.Range(0, dashAdjustlist.Count - 1);

            DialoguePiece chosen = dashAdjustlist[rand3];
            chosen.seen = true;

            StartDialogue(chosen, true);
        }
    }

    private void AddInterjections(List<DialoguePiece> smallTalkList, List<DialoguePiece> dashAdjustList) {

        foreach (DialoguePiece interjection in car.currentPassenger.interjections) {

            if (!interjection.seen) {

                switch (interjection.interjectionType) {
                    case InterjectionType.SMALL_TALK:
                        smallTalkList.Add(interjection);
                        break;
                    case InterjectionType.DASH_ADJUST:
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
    }
}
