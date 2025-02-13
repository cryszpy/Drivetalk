using UnityEngine;

[System.Serializable]
public struct DialogueLine {

    [TextArea(3, 10)]
    public string sentence;
    public PassengerExpression expression;
    public PassengerExpression startingExpression;

    public bool requestsStart;
    public bool requestsEnd;
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/DialoguePiece")]
public class DialoguePiece : ScriptableObject
{

    [Tooltip("This dialogue piece's total sentences.")]
    public DialogueLine[] lines;

    [Tooltip("The next dialogue piece to play after this piece.")]
    public DialoguePiece nextDialogue;

    [Tooltip("Boolean flag; Whether this dialogue piece contains the first mention of the passenger's name.")]
    public bool firstNameUsage = false;

    [Tooltip("What expression the passenger should have during this dialogue piece.")]
    public PassengerExpression fallbackExpression;

    [Tooltip("Any choices that must be made after this piece of dialogue ends.")]
    public DialogueChoice[] choices;
}