using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIElement : MonoBehaviour
{

    private CarController car;

    public Animator animator;

    public TMP_Text elementText;

    public Button continueButton;

    public GameObject clickIndicator;

    public bool finished = false;

    // Update is called once per frame
    void Update()
    {
        if (finished)
        {
            finished = false;

            if (GameStateManager.dialogueManager.autoDialogue) StartCoroutine(NaturalDestroy());
        }

        // Make sure the continue button is disabled if auto-dialogue is on
        if (continueButton) continueButton.interactable = !GameStateManager.dialogueManager.autoDialogue;
    }

    private IEnumerator NaturalDestroy() {

        if (car.currentPassenger) {
            yield return new WaitForSeconds(car.currentPassenger.holdTime);
        }

        // Make sure choice prompt dialogue stays while choices are active
        while (GameStateManager.dialogueManager.playingChoices) {
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

    public virtual (GameObject, GameObject) Create(GameObject original, Transform parent, CarController car) {
        GameObject dialogueBlock = Instantiate(original, parent);
        
        if (dialogueBlock.TryGetComponent<DialogueUIElement>(out var script)) {
            script.car = car;
            script.continueButton.onClick.AddListener(() => GameStateManager.dialogueManager.ContinueButton());
            return (dialogueBlock, script.clickIndicator);
        } else {
            Debug.LogError("Could not find DialogueUIElement script or extension of such on this Object.");
            return (null, null);
        }
    }
}