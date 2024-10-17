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

    private Queue<string> sentences;

    public DialoguePiece currentDialogue;

    [SerializeField] private List<GameObject> choiceButtonsList;

    public GameObject choiceNotif;

    [SerializeField] private RearviewMirror rearviewMirror;
    
    [Header("STATS")]

    public List<DialoguePiece> priorityDialogue;

    [SerializeField] private float textCPS;

    [SerializeField] private float choicesNotifSolid;
    [SerializeField] private float choicesNotifFlashing;

    public bool playingChoices = false;

    private void Start() {
        sentences = new Queue<string>();
    }

    private void Update() {
        if (mode == DialogueMode.RMM && currentDialogue && !playingChoices && currentDialogue) {
            rearviewMirror.backButton.SetActive(true);
            RMM_dialogueBox.SetActive(true);
            dash_dialogueBox.SetActive(false);
        } else if (mode == DialogueMode.DASH && currentDialogue && !playingChoices && currentDialogue) {
            RMM_dialogueBox.SetActive(false);
            dash_dialogueBox.SetActive(true);
        } else {
            RMM_dialogueBox.SetActive(false);
            dash_dialogueBox.SetActive(false);
        }
    }

    public void EnterRMM() {
        mode = DialogueMode.RMM;
        choiceNotif.SetActive(false);
    }

    public void ExitRMM() {
        mode = DialogueMode.DASH;
    }

    public void StartDialogue(DialoguePiece dialogue) {

        currentDialogue = dialogue;

        priorityDialogue.Remove(dialogue);

        sentences.Clear();

        car.currentPassenger.currentDialogueNum++;
        car.currentPassenger.dialogueLeftToFinish--;

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

        // End dialogue if no choices and nothing else to say
        if (sentences.Count == 0 && currentDialogue.choices.Length == 0) {
            EndDialogue();
            return;
        } 
        // Display choices
        else if (sentences.Count == 0 && currentDialogue.choices.Length > 0) {

            playingChoices = true;

            if (mode != DialogueMode.RMM) {
                StartCoroutine(ChoicesNotification());
            } else {
                ShowChoices();
            }
            
            return;
        }

        string sentence = sentences.Dequeue();

        StopAllCoroutines();

        StartCoroutine(TypeSentence(sentence));
    }

    public IEnumerator ChoicesNotification() {

        if (playingChoices) {

            // Enable and show the interaction notification
            choiceNotif.SetActive(true);
            choiceNotifAnimator.SetBool("Flash", false);

            yield return new WaitForSeconds(choicesNotifSolid);

            // Start flashing the interaction notification
            choiceNotifAnimator.SetBool("Flash", true);

            yield return new WaitForSeconds(choicesNotifFlashing);

            // Disable and hide the interaction notification
            choiceNotif.SetActive(false);
            choiceNotifAnimator.SetBool("Flash", false);

            // Automatically go with the last choice option (should be "...") if the player doesn't click to interact
            if (mode != DialogueMode.RMM) {
                StartDialogue(currentDialogue.choices[^1].nextDialogue);
            }
        } else {
            choiceNotif.SetActive(false);
            choiceNotifAnimator.SetBool("Flash", false);
        }
        
    }

    public void ShowChoices() {

        // Remove existing buttons
        if (choiceButtonsList.Count > 0) {
            foreach (GameObject button in choiceButtonsList) {
                Destroy(button);
            }
            choiceButtonsList.Clear();
        }

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
            StartDialogue(choice.nextDialogue);
        }
    }

    private IEnumerator TypeSentence(string sentence) {
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

        StartDialogue(car.currentPassenger.dialogue[car.currentPassenger.currentDialogueNum]);
    }
}
