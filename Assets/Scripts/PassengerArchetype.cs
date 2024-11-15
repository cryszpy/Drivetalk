using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PassengerArchetype")]
public class PassengerArchetype : ScriptableObject
{
    [Tooltip("Pickup greeting dialogue piece for this archetype.")]
    public DialoguePiece pickupGreeting;

    [Tooltip("Dropoff salute dialogue piece for this archetype.")]
    public DialoguePiece dropoffSalute;
}