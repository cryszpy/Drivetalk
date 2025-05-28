using System;
using System.Collections.Generic;
using UnityEngine;

[Tooltip("Struct definition for a requirement needed for a passenger to be able to show up.")]
[Serializable]
public struct PassengerRequirement {
    public PassengerRequirementType reqType;

    public Passenger passengerReq;

    public float statToCheck;
}

public class Passenger : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")] // -------------------------------------------------------------------------------------------------

    [Tooltip("The image to be displayed on the mood meter for this passenger's rides.")]
    public Sprite headshot;

    [Tooltip("This passenger's Animator component.")]
    public Animator animator;

    [Tooltip("The story container reference for this passenger's dialogue.")]
    public StoryContainer storyContainer;

    [Tooltip("This passenger's possible expressions.")]
    public List<PassengerExpression> expressions = new();

    [Tooltip("Any requirements for this passenger to have a chance of showing up.")]
    public List<PassengerRequirement> requirements;

    [Tooltip("List of all destinations this passenger will request, in order of ride number.")]
    public List<GameObject> requestedDestinationTiles;

    [Tooltip("This passenger's list of gifted objects, using the index as the ID.")]
    public List<GameObject> gifts = new();

    [Header("STATS")] // -------------------------------------------------------------------------------------------------

    [Tooltip("The rarity bracket this passenger belongs to.")]
    public PassengerRarity rarity;

    [Tooltip("This passenger's ID number.")]
    public int id;

    [Tooltip("This passenger's name.")]
    public string passengerName;

    public string currentName;

    [Tooltip("What color this passenger's name is in the name box.")]
    public Color nameColor = Color.white;

    [Tooltip("Whether this passenger's spawn requirements have been met.")]
    public bool requirementMet;

    [Tooltip("The rate at which this passenger types out dialogue text. (lower values are faster)")]
    public float textCPS;

    [Tooltip("The amount of time this passenger will have each sentence held on-screen for.")]
    public float holdTime;
    [Tooltip("The minimum possible time that this passenger waits to switch from default to side expression views.")]
    public float minSwitchTime;
    [Tooltip("The maximum possible time that this passenger waits to switch from default to side expression views.")]
    public float maxSwitchTime;

    [Tooltip("The rate at which this passenger's comfortability meter drains, per second.")]
    public float meterSpeed;

    [Tooltip("The amount of time this passenger waits to begin talking after selecting a destination.")]
    public float waitAfterRouteTime;

    [Tooltip("A list of all voice lines for this character.")]
    public List<AudioClip> voicelines = new();

    [Header("DASHBOARD PREFERENCES")] // -------------------------------------------------------------------------------------------------

    public DashboardPreference preferences;
}

[Tooltip("Enum describing passenger spawn requirement types.")]
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