using UnityEngine;

[System.Serializable]
public struct DialogueLine {

    [TextArea(3, 10)]
    public string sentence;
    public PassengerExpression expression;

    [Header("PRE-DIALOGUE ACTIONS")]

    public PassengerExpression startingExpression;

    [Tooltip("The duration that this passenger will wait before saying **this** line. (If this is not a long pause, leave at 0)")]
    public float longPauseTime;

    [Tooltip("Audio file to play <b>before</b> this line is typed out.")]
    public AudioClip audioToPlay;

    [Tooltip("An object to spawn on the dashboard <b>before</b> this line is said.")]
    public GameObject dashboardObject;

    [Header("OTHER")]

    [Tooltip("Whether this dialogue piece contains the first mention of the passenger's name.")]
    public bool firstNameUsage;

    [Tooltip("Voice line to play.")]
    public AudioClip voiceLine;

    [Tooltip("Whether this line enables the mood meter.")]
    public bool requestsStart;

    [Tooltip("Whether this line disables the mood meter.")]
    public bool requestsEnd;

    [Tooltip("Whether this line signals an early dropoff for the current passenger.")]
    public bool earlyDropoff;

    [Tooltip("Upon reaching this dialogue line, the destination will spawn.")]
    public bool spawnDestination;
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/DialoguePiece")]
public class DialoguePiece : ScriptableObject
{

    [Tooltip("This dialogue piece's total sentences.")]
    public DialogueLine[] lines;

    [Tooltip("The next dialogue piece to play after this piece.")]
    public DialoguePiece nextDialogue;

    [Tooltip("What expression the passenger should have during this dialogue piece.")]
    public PassengerExpression fallbackExpression;

    [Tooltip("Any choices that must be made after this piece of dialogue ends.")]
    public DialogueChoice[] choices;
}