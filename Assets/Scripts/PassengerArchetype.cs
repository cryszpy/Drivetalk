using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PassengerArchetype")]
public class PassengerArchetype : ScriptableObject
{
    public DialoguePiece pickupGreeting;
    public DialoguePiece dropoffSalute;
    public DialoguePiece acResponse;
    public DialoguePiece songResponse;
    public DialoguePiece songVolumeResponse;
}