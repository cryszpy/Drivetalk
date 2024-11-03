using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct PassengerRequirement {
    public PassengerRequirementType reqType;

    public Passenger passengerReq;

    public float statToCheck;
}

public class Passenger : MonoBehaviour
{

    [Tooltip("The rarity bracket this passenger belongs to.")]
    public PassengerRarity rarity;

    [Tooltip("This passenger's ID number.")]
    public int id;

    [Tooltip("This passenger's name.")]
    public string passengerName;

    [Tooltip("This passenger's generic response archetype.")]
    public PassengerArchetype archetype;

    [Tooltip("Any requirements for this passenger to have a chance of showing up.")]
    public List<PassengerRequirement> requirements;
    [Tooltip("Whether this passenger's spawn requirements have been met.")]
    public bool requirementMet;

    [Tooltip("The rate at which this passenger types out dialogue text. (lower values are faster)")]
    public float textCPS;

    [Tooltip("The amount of time the choice notification UI element stays SOLID for this passenger.")]
    public float choiceNotifSolidTime;
    [Tooltip("The amount of time the chioce notification UI element stays FLASHING for this passenger.")]
    public float choiceNotifFlashTime;

    [Tooltip("The minimum possible time that this passenger waits between saying dialogue groups.")]
    public float waitTimeMin;
    [Tooltip("The maximum possible time that this passenger waits between saying dialogue groups.")]
    public float waitTimeMax;
    [Tooltip("The amount of time this passenger will have each sentence held on-screen for.")]
    public float holdTime;

    [Tooltip("The chance that this passenger interjects in between dialogue groups.")]
    public float interjectionChance;
    [Tooltip("Left of this value is the chance for SMALL TALK — Right of this value is the chance for DASH REQUESTS.")]
    public float interjectionPreferenceThreshold;
    [Tooltip("The time limit that the player has to respond to a DASH REQUEST, before losing affinity.")]
    public float dashRequestTime;
    [Tooltip("The rate at which the player loses 1 affinity for every x second of not responding to a DASH REQUEST. (1 affinity/x seconds)")]
    public float dashRequestTickRate;
    [Tooltip("List of all possible interjections for this passenger. (SMALL TALK & DASH REQUESTS)")]
    public List<DialoguePiece> interjections;

    [Tooltip("The current dialogue's number for this passenger.")]
    public int currentDialogueNum;
    [Tooltip("How many original dialogue pieces left to finish this passenger's story. (NOT including interjections, choices, or dialogue stemming from choices.)")]
    public int dialogueLeftToFinish;
    [Tooltip("List of all dialogue beats for this passenger, in order.")]
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