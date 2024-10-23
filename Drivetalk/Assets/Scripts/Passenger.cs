using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PassengerRequirement {
    public PassengerRequirementType reqType;

    public Passenger passengerReq;

    public float statToCheck;
}

public class Passenger : MonoBehaviour
{

    public PassengerRarity rarity;

    public int id;

    public string passengerName;

    public List<PassengerRequirement> requirements;
    public bool requirementMet;

    public float choiceNotifSolidTime;
    public float choiceNotifFlashTime;

    public float waitTimeMin;
    public float waitTimeMax;
    public float holdTime;

    public float interjectionChance;
    public float interjectionPreferenceThreshold;
    public List<DialoguePiece> interjections;

    public int currentDialogueNum;
    public int dialogueLeftToFinish;
    public List<DialoguePiece> dialogue;

    private void Start() {
        dialogueLeftToFinish = dialogue.Count;
    }
}

public enum PassengerRequirementType {
    NONE,
    HAS_DRIVEN_PASSENGER,
    LAST_PASSENGER_DRIVEN,
    SONG_IS_PLAYING,
    TOTAL_PASSENGERS_DRIVEN,
    DAY_NUM,
    RATING_SCORE_EQUALS,
    RATING_SCORE_GREATER_THAN,
    RATING_SCORE_LESS_THAN,
}