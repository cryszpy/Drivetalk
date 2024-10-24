using System.Collections.Generic;
using UnityEngine;

public enum InterjectionType {
    NONE, SMALL_TALK, DASH_REQUEST
}

[CreateAssetMenu(menuName = "ScriptableObjects/DialoguePiece")]
public class DialoguePiece : ScriptableObject
{
    public InterjectionType interjectionType;
    public DashRequestRequirement dashRequirement;
    
    public bool seen;

    [TextArea(3, 10)]
    public string[] sentences;

    public DialoguePiece nextDialogue;

    public DialogueChoice[] choices;
}

[System.Serializable]
public struct DashRequestRequirement {
    public DashRequestType reqType;

    public ACSetting acSetting;

    public float statToCheck;
}

public enum DashRequestType {
    NONE,
    AC_SETTING,
    HORN, 
    RADIO_VOLUME,
    RADIO_SONG,
    CIGARETTE
}