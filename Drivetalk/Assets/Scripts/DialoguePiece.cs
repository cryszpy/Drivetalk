using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DialoguePiece")]
public class DialoguePiece : ScriptableObject
{
    public bool dialogueSeen;

    public DialoguePiece[] choices;

    [TextArea(3, 10)]
    public string[] sentences;
}
