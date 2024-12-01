using System;
using System.Collections.Generic;
using UnityEngine;

public enum DashboardControl {
    NONE, AC, RADIO_SONG, RADIO_VOLUME, HAZARDS, WIPERS, HORN, DEFOG, HEADLIGHTS, WINDOWS
}

public enum FloatCheckType {
    EQUAL, LESSER, GREATER
}

[System.Serializable]
public class DashboardRequest {

    public DashboardControl control;

    public float value = 5;
    public float min;
    public float max;

    [HideInInspector] public float requestTimer = 0;

    public FloatCheckType floatCheckType;

    public bool boolValue;

    public float floatValue;

    public bool hasResponded = false;

    public DialoguePiece completedResponse;
    public DialoguePiece preCompletedResponse;
}

[System.Serializable]
public struct DialogueLine {

    [TextArea(3, 10)]
    public string sentence;
    public PassengerExpression expression;
    public PassengerExpression startingExpression;
}


[CreateAssetMenu(menuName = "ScriptableObjects/DialoguePiece")]
public class DialoguePiece : ScriptableObject
{

    [Tooltip("This dialogue piece's total sentences.")]
    public DialogueLine[] lines;

    [Tooltip("The next dialogue piece to play after this piece.")]
    public DialoguePiece nextDialogue;

    [Tooltip("Boolean flag; Checks whether this piece of dialogue is a first-line greeting.")]
    public DashboardRequest request;

    [Tooltip("Boolean flag; Whether this dialogue piece contains the first mention of the passenger's name.")]
    public bool firstNameUsage = false;

    [Tooltip("What expression the passenger should have during this dialogue piece.")]
    public PassengerExpression fallbackExpression = PassengerExpression.DEFAULT;

    [Tooltip("Any choices that must be made after this piece of dialogue ends.")]
    public DialogueChoice[] choices;
}