using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]
    
    public CarController car;

    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialogueBox;

    [SerializeField] private Animator animator;

    private Queue<string> sentences;

    [SerializeField] private DialoguePiece currentDialogue;
    
    [Header("STATS")]

    public List<DialoguePiece> priorityDialogue;

    [SerializeField] private float textCPS;

    private bool playingDialogue = false;

    private void Start() {
        sentences = new Queue<string>();
    }

    public void StartDialogue(DialoguePiece dialogue) {
        playingDialogue = true;

        currentDialogue = dialogue;

        priorityDialogue.Remove(dialogue);

        sentences.Clear();

        car.currentPassenger.currentDialogueNum++;
        car.currentPassenger.dialogueLeftToFinish--;

        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }

        dialogueBox.SetActive(true);

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (sentences.Count == 0 && currentDialogue.choices.Length == 0) {
            EndDialogue();
            return;
        } else if (sentences.Count == 0 && currentDialogue.choices.Length != 0) {
            Debug.Log("DISPLAY CHOICES");
            return;
        }

        string sentence = sentences.Dequeue();

        StopAllCoroutines();

        StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence) {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;

            yield return new WaitForSeconds(textCPS);
        }

        yield return new WaitForSeconds(car.currentPassenger.holdTime);

        DisplayNextSentence();
    }

    public void EndDialogue() {
        // SET SPEECH BUBBLE TO FADE AWAY

        playingDialogue = false;
        dialogueBox.SetActive(false);

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
