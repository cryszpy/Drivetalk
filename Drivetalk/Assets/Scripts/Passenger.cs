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

    public List<DialoguePiece> choices;

    public int dialogueLeftToFinish;

    public List<DialoguePiece> dialogue;
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