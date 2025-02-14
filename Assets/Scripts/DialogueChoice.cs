using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DialogueChoice")]
public class DialogueChoice : DialoguePiece
{

    [Tooltip("The text it says on the choice button.")]
    [TextArea(3, 10)]
    public string choiceText;

    [Tooltip("How much this choice affects the passenger's mood meter.")]
    public float moodChange;
}