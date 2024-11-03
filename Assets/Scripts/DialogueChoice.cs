using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DialogueChoice")]
public class DialogueChoice : ScriptableObject
{
    public bool dialogueSeen;

    [TextArea(3, 10)]
    public string choiceText;

    public DialoguePiece nextDialogue;
}