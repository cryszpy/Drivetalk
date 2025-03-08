using System.Collections.Generic;
using UnityEngine;

public enum PassengerRarity {
    STORY, TIER_ONE, TIER_TWO, TIER_THREE, TIER_FOUR
}

[CreateAssetMenu(menuName = "ScriptableObjects/PassengerList")]
public class PassengerList : ScriptableObject
{
    public float[] rarityProbabilities;
    
    public List<Passenger> passengerQueue = new();
    public List<Passenger> storyPassengers = new();
    public List<Passenger> exhaustedStory = new();

    public List<Passenger> tierOnePassengers = new();
    public List<Passenger> exhaustedTierOne = new();

    public List<Passenger> tierTwoPassengers = new();
    public List<Passenger> exhaustedTierTwo = new();

    public List<Passenger> tierThreePassengers = new();
    public List<Passenger> exhaustedTierThree = new();

    public List<Passenger> tierFourPassengers = new();
    public List<Passenger> exhaustedTierFour = new();

    public void GetRandomPassenger(PassengerRarity rarity) {

        int rand;

        Passenger passenger;

        switch (rarity) {
            case PassengerRarity.TIER_ONE:
                rand = UnityEngine.Random.Range(0, tierOnePassengers.Count - 1);
                passenger = tierOnePassengers[rand];
                break;
            case PassengerRarity.TIER_TWO:
                rand = UnityEngine.Random.Range(0, tierTwoPassengers.Count - 1);
                passenger = tierTwoPassengers[rand];
                break;
            case PassengerRarity.TIER_THREE:
                rand = UnityEngine.Random.Range(0, tierThreePassengers.Count - 1);
                passenger = tierThreePassengers[rand];
                break;
            case PassengerRarity.TIER_FOUR:
                rand = UnityEngine.Random.Range(0, tierFourPassengers.Count - 1);
                passenger = tierFourPassengers[rand];
                break;
            default:
                rand = UnityEngine.Random.Range(0, tierOnePassengers.Count - 1);
                passenger = tierOnePassengers[rand];
                break;
        }

        // If passenger has requirements, check them
        if (passenger.requirements.Count > 0) {
            for (int i = 0; i < passenger.requirements.Count; i++) {
                if (CheckRequirements(passenger.requirements[i])) {
                    passengerQueue.Add(passenger);
                } else {
                    GetRandomPassenger(rarity);
                }
            }
        } else {
            passengerQueue.Add(passenger);
        }
    }

    public bool CheckRequirements(PassengerRequirement requirement) {
        return requirement.reqType switch
        {
            PassengerRequirementType.HAS_DRIVEN_PASSENGER => CarController.PassengersDrivenIDs.Contains(requirement.passengerReq.id),
            PassengerRequirementType.LAST_PASSENGER_DRIVEN => CarController.LastPassengerID == requirement.passengerReq.id,
            PassengerRequirementType.SONG_IS_PLAYING => CarController.CurrentRadioChannel == requirement.statToCheck,
            PassengerRequirementType.TOTAL_PASSENGERS_DRIVEN => CarController.TotalPassengersDriven == requirement.statToCheck,
            PassengerRequirementType.DAY_NUM => false,
            PassengerRequirementType.RATING_SCORE_EQUALS => CarController.Rating == requirement.statToCheck,
            PassengerRequirementType.RATING_SCORE_GREATER_THAN => CarController.Rating > requirement.statToCheck,
            PassengerRequirementType.RATING_SCORE_LESS_THAN => CarController.Rating < requirement.statToCheck,
            _ => false,
        };
    }

    public void ExhaustPassenger(Passenger passenger, PassengerRarity rarity) {
        switch (rarity) {
            case PassengerRarity.STORY:
                exhaustedStory.Add(passenger);
                storyPassengers.Remove(passenger);
                break;
            case PassengerRarity.TIER_ONE:
                exhaustedTierOne.Add(passenger);
                tierOnePassengers.Remove(passenger);
                break;
            case PassengerRarity.TIER_TWO:
                exhaustedTierTwo.Add(passenger);
                tierTwoPassengers.Remove(passenger);
                break;
            case PassengerRarity.TIER_THREE:
                exhaustedTierThree.Add(passenger);
                tierThreePassengers.Remove(passenger);
                break;
            case PassengerRarity.TIER_FOUR:
                exhaustedTierFour.Add(passenger);
                tierFourPassengers.Remove(passenger);
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
        ResetPassengerList(exhaustedTierOne, tierOnePassengers);
        ResetPassengerList(exhaustedTierTwo, tierTwoPassengers);
        ResetPassengerList(exhaustedTierThree, tierThreePassengers);
        ResetPassengerList(exhaustedTierFour, tierFourPassengers);
    }
}