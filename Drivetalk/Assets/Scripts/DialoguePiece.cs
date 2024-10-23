using System.Collections.Generic;
using UnityEngine;

public enum InterjectionType {
    NONE, SMALL_TALK, DASH_ADJUST
}

[CreateAssetMenu(menuName = "ScriptableObjects/DialoguePiece")]
public class DialoguePiece : ScriptableObject
{
    public InterjectionType interjectionType;
    
    public bool seen;

    [TextArea(3, 10)]
    public string[] sentences;

    public DialoguePiece nextDialogue;

    public DialogueChoice[] choices;
}