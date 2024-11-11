using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DialoguePiece")]
public class DialoguePiece : ScriptableObject
{

    [Tooltip("This dialogue piece's total sentences.")]
    [TextArea(3, 10)]
    public string[] sentences;

    [Tooltip("The next dialogue piece to play after this piece.")]
    public DialoguePiece nextDialogue;

    [Tooltip("Boolean flag; Whether this dialogue piece contains the first mention of the passenger's name.")]
    public bool firstNameUsage = false;

    [Tooltip("Any choices that must be made after this piece of dialogue ends.")]
    public DialogueChoice[] choices;
}