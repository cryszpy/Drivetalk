using UnityEngine;

public class TaxiStopDestination : MonoBehaviour
{
    public PassengerList passengerList;

    private CarController car;

    private void OnTriggerEnter(Collider collider) {

        if (collider.CompareTag("Car")) {

            Debug.Log("Arrived at taxi stop!");

            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {
                car = script;
            } else {
                Debug.LogWarning("Could not find CarController script on car!");
            }
            
            if (!car.currentPassenger) {
                GeneratePassenger();
            }
        }
    }

    private void GeneratePassenger() {

        float rand = Random.value;

        GetPassengerProbability(rand, passengerList.rarityProbabilities);
    }

    private void GetPassengerProbability(float value, float[] probTable) {
        
        if (value <= probTable[0]) {
            if (passengerList.tierOnePassengers.Count > 0) {

                passengerList.GetRandomPassenger(PassengerRarity.TIER_ONE);

            } else {
                Debug.LogWarning("Would've spawned a TIER ONE passenger but there are none!");
            }
        } else if (value <= probTable[1]) {
            if (passengerList.tierTwoPassengers.Count > 0) {

                passengerList.GetRandomPassenger(PassengerRarity.TIER_TWO);

            } else {
                Debug.LogWarning("Would've spawned a TIER TWO passenger but there are none!");
            }
        } else if (value <= probTable[2]) {
            if (passengerList.tierThreePassengers.Count > 0) {

                passengerList.GetRandomPassenger(PassengerRarity.TIER_THREE);

            } else {
                Debug.LogWarning("Would've spawned a TIER THREE passenger but there are none!");
            }
        } else {
            if (passengerList.tierFourPassengers.Count > 0) {

                passengerList.GetRandomPassenger(PassengerRarity.TIER_FOUR);

            } else {
                Debug.LogWarning("Would've spawned a TIER FOUR passenger but there are none!");
            }
        }

        // Make sure there are passengers to pick up
        if (passengerList.passengerQueue.Count > 0) {

            Passenger passenger = passengerList.passengerQueue[0];

            // Spawn passenger at stop
            Instantiate(passenger.gameObject, transform.position, Quaternion.identity);

            passengerList.passengerQueue.Remove(passenger);

            // MOVE THIS CODE TO DROP OFF PASSENGER FUNCTION
            /* if (passenger.dialogueLeftToFinish <= 0) {
                passenger.dialogueLeftToFinish = 0;
                passengerList.ExhaustPassenger(passenger, PassengerRarity.TIER_ONE);
                Debug.Log("Dialogue finished, passenger exhausted!");
            } */
        } else {
            Debug.LogWarning("No passenger in queue!");
        }
    }
}
