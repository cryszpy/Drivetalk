using UnityEngine;

public class TaxiStopDestination : MonoBehaviour
{

    [Tooltip("Reference to the car.")]
    private CarController car;

    // Triggers on collision with an object
    private void OnTriggerEnter(Collider collider) {

        // If the collided object is the car—
        if (collider.CompareTag("CarFrame") && GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            // If the car's script is accessible—
            if (collider.transform.parent.TryGetComponent<CarController>(out var script)) {

                // Set the car reference to the accessed script
                car = script;

                if (car.carPointer.taxiStopsEnabled) {
                    car.atTaxiStop = true;
                    Debug.Log("Arrived at taxi stop!");
                }
                
            } else {
                Debug.LogError("Could not find CarController script on car!");
            }
            
            // If the car does not have a current passenger—
            if (!car.currentPassenger) {

                // Generate a passenger.
                GeneratePassenger();
            }
        }
    }

    // Generate a passenger
    private void GeneratePassenger() {

        /* float rand = Random.value;

        GetPassengerProbability(rand, passengerList.rarityProbabilities); */

        // Make sure there are passengers to pick up
        if (car.passengerList.storyPassengers.Count > 0) {

            // Get the next passenger in the queue
            Passenger passenger = car.passengerList.storyPassengers[0];

            // Spawn passenger at stop
            GameObject character = Instantiate(passenger.gameObject, transform.position, Quaternion.identity);

            // Exhaust / remove passenger from the queue
            car.passengerList.ExhaustPassenger(passenger, PassengerRarity.STORY);

            // Ensure dialogue is updated and external functions are bound
            passenger.storyContainer.SetStories();

            // Pick up passenger in the car
            car.PickUpPassenger(character);

            // TODO ADD IN PASSENGER AI WALK TO CAR

        } else {
            Debug.LogWarning("No story passengers left!");
            // PUT END OF DEMO SCREEN HERE
            GameStateManager.dialogueManager.demoOverScreen.SetActive(true);
        }
    }

    /* private void GetPassengerProbability(float value, float[] probTable) {
        
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
    } */
}
