using UnityEngine;

public class PassengerExpressionTrigger : MonoBehaviour
{
    // Called when the passenger's pre-expression animation is finished
    private void Trigger() {

        // Reset starting expression status
        GameStateManager.dialogueManager.startingExpressionDone = true;
    }
}