using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DialoguePiece")]
public class DialoguePiece : ScriptableObject
{
    public bool dialogueSeen;

    [TextArea(3, 10)]
    public string[] sentences;

    public DialoguePiece nextDialogue;

    public DialogueChoice[] choices;
}