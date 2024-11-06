using UnityEngine;

public class TaxiStopDestination : MonoBehaviour
{
    public PassengerList passengerList;

    private CarController car;

    private void OnTriggerEnter(Collider collider) {

        if (collider.CompareTag("CarFrame")) {

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
            GameObject character = Instantiate(passenger.gameObject, transform.position, Quaternion.identity);

            passengerList.passengerQueue.Remove(passenger);

            car.PickUpPassenger(character);

            // TODO ADD IN PASSENGER AI WALK TO CAR

        } else {
            Debug.LogWarning("No passenger in queue!");
        }
    }
}
