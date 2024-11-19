using UnityEngine;

public class PassengerExpressionTrigger : MonoBehaviour
{
    private DialogueManager dialogueManager;

    private void Start() {
        dialogueManager = GameStateManager.dialogueManager;
    }

    // Called when the passenger's pre-expression animation is finished
    private void Trigger() {
        dialogueManager.SwitchExpression(dialogueManager.currentDialogue.nextDialogue.expression);
        dialogueManager.StartDialogue(dialogueManager.currentDialogue.nextDialogue);
    }
}
