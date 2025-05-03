using System.Collections.Generic;
using UnityEngine;

public enum PassengerRarity {
    STORY, TIER_ONE, TIER_TWO, TIER_THREE, TIER_FOUR
}

[CreateAssetMenu(menuName = "ScriptableObjects/PassengerList")]
public class PassengerList : ScriptableObject
{
    
    public List<Passenger> storyPassengers = new();
    public List<Passenger> exhaustedStory = new();

    public List<Passenger> backupList = new();

    public void ExhaustPassenger(Passenger passenger, PassengerRarity rarity) {
        switch (rarity) {
            case PassengerRarity.STORY:
                exhaustedStory.Add(passenger);
                storyPassengers.Remove(passenger);
                break;
        }
    }

    public void ResetPassengerList(List<Passenger> swapFrom, List<Passenger> swapTo) {
        if (swapFrom.Count != 0) {
            foreach (Passenger passenger in swapFrom.ToArray()) {
                swapTo.Add(passenger);
                swapFrom.Remove(passenger);
            }
        }
    }

    public void ResetListInOrder(List<Passenger> exhaustedList, List<Passenger> list) {

        // Move all remaining list characters to exhausted list
        if (list.Count > 0) {
            foreach (Passenger passenger in list.ToArray()) {
                exhaustedList.Add(passenger);
                list.Remove(passenger);
            }
        }

        // Swap exhausted list and regular list
        ResetPassengerList(exhaustedList, list);
    }

    public void ResetAllPassengers() {
        ResetListInOrder(exhaustedStory, storyPassengers);
    }
}