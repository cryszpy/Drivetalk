using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueUIElement : MonoBehaviour
{

    private CarController car;

    public Animator animator;

    public TMP_Text elementText;

    public bool finished = false;

    // Update is called once per frame
    void Update()
    {
        if (finished) {
            finished = false;

            StartCoroutine(NaturalDestroy());
        }
    }

    private IEnumerator NaturalDestroy() {

        if (car.currentPassenger) {
            yield return new WaitForSeconds(car.currentPassenger.holdTime);
        }

        Debug.Log("Checked!");

        while (GameStateManager.dialogueManager.playingChoices 
            || (GameStateManager.dialogueManager.currentDialogue != null 
            && GameStateManager.dialogueManager.currentDialogue.choices.Length > 0 
            && GameStateManager.dialogueManager.currentLine.sentence == GameStateManager.dialogueManager.currentDialogue.lines.Last().sentence 
            && GameStateManager.dialogueManager.currentDialogue.choices.Length > 0)) {
            yield return null;
        }

        animator.SetTrigger("Out"); // Triggers destruction of element at end of animation
    }

    public void DestroyElement() {

        if (GameStateManager.dialogueManager.activeDialogueBlocks.Contains(gameObject)) {
            GameStateManager.dialogueManager.activeDialogueBlocks.Dequeue();
        }
        
        Destroy(gameObject);
    }

    public virtual UnityEngine.Object Create(UnityEngine.Object original, Transform parent, CarController car) {
        GameObject dialogueBlock = Instantiate(original, parent) as GameObject;
        
        if (dialogueBlock.TryGetComponent<DialogueUIElement>(out var script)) {
            script.car = car;
            return dialogueBlock;
        } else {
            Debug.LogError("Could not find DialogueUIElement script or extension of such on this Object.");
            return null;
        }
    }
}