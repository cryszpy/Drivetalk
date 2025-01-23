using System.Linq;
using UnityEngine;

public class PassengerExpressionTrigger : MonoBehaviour
{

    // Called when the passenger's pre-expression animation is finished
    private void Trigger() {
        
        if (GameStateManager.dialogueManager.lines.Count > 0 && !GameStateManager.dialogueManager.typingSentence) {

            // Reset starting expression status
            GameStateManager.dialogueManager.startingExpressionDone = true;

            // Switch back to regular expression
            GameStateManager.dialogueManager.SwitchExpression(GameStateManager.dialogueManager.lines.First().expression);

            // Resume talking if this is triggered as the pre-expression only
            if (GameStateManager.dialogueManager.currentLine.expression == GameStateManager.dialogueManager.car.currentPassenger.expressions[0])
                GameStateManager.dialogueManager.DisplayNextSentence();

            //StartCoroutine(GameStateManager.dialogueManager.TypeSentence(GameStateManager.dialogueManager.currentLine));
        } else if (GameStateManager.dialogueManager.currentDialogue && GameStateManager.dialogueManager.preChoiceDialogue) {

            // If the currently active request has been precompleted, play respective dialogue
            if (GameStateManager.dialogueManager.currentDialogue == GameStateManager.dialogueManager.preChoiceDialogue.request.preCompletedResponse) {
                GameStateManager.dialogueManager.StartDialogue(GameStateManager.dialogueManager.currentDialogue);
            }

        }
        /* if (!dialogueManager.currentDialogue.nextDialogue && dialogueManager.preChoiceDialogue) {
            dialogueManager.SwitchExpression(dialogueManager.currentLine.expression);
            dialogueManager.StartDialogue(dialogueManager.currentDialogue);
        } else {
            dialogueManager.SwitchExpression(dialogueManager.currentDialogue.nextDialogue.lines[0].expression);
            dialogueManager.StartDialogue(dialogueManager.currentDialogue.nextDialogue);
        } */
    }
}