using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DialogueChoice")]
public class DialogueChoice : ScriptableObject
{

    [Tooltip("The text it says on the choice button.")]
    [TextArea(3, 10)]
    public string choiceText;

    [Tooltip("Reference to the next piece of dialogue from this choice.")]
    public DialoguePiece nextDialogue;
}