using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GPSScreen : MonoBehaviour
{

    [Tooltip("Reference to the main map script.")]
    [SerializeField] private GPS gps;

    public RoadList roadList;

    public GameObject pivot;

    public TMP_InputField inputField;

    public TMP_Text inputText;

    public TMP_Text placeholderText;

    public TMP_Text actualText;

    public GPSDestination currentDestination;

    public List<GameObject> recentDestTiles = new();

    [Tooltip("Reference to the car's script.")]
    private CarController car;

    [SerializeField] private bool backspace = false;

    private bool entered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (!car) {
            car = GameObject.FindGameObjectWithTag("Car").GetComponent<CarController>();
            Debug.Log("Car component unassigned! Reassigned.");
        }
    }

    private void Update()
    {
        
        if ((Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Backspace)) && !backspace) {
            backspace = true;

            if (actualText.maxVisibleCharacters > 0) {
                actualText.maxVisibleCharacters = 0;
                inputField.textComponent.text = "";
                inputField.text = "";
            }
        }
        else {
            backspace = false;
        }

        if (Input.GetKeyDown(KeyCode.Return) && !entered) {
            entered = true;
            EnterDestination();
        }
    }

    // Called when picking a passenger up
    public void SetDestination(GPSDestination destination) {

        // Reset enter button
        entered = false;

        if (!destination.tile || destination.gpsText.Length <= 0) {
            Debug.LogError("Destination is invalid!");
            return;
        }

        currentDestination = destination;

        placeholderText.text = currentDestination.gpsText;
        inputField.characterLimit = currentDestination.gpsText.Length;

        actualText.maxVisibleCharacters = 0;
        actualText.text = currentDestination.gpsText;
    }

    // Called from Unity Event when player types anything
    public void UpdateInputText() {

        if (actualText.maxVisibleCharacters < actualText.text.Length && !backspace 
            && !Input.GetKey(KeyCode.Delete) && !Input.GetKey(KeyCode.Backspace)) {
            actualText.maxVisibleCharacters++;
        }
    }

    // Sets the map upon clicking the enter
    public void EnterDestination() {

        if (actualText.maxVisibleCharacters >= currentDestination.gpsText.Length) {

            gps.dragging = false;
            gps.gameObject.layer = gps.regularLayer;

            GameStateManager.dialogueManager.waitForRouting = false; // Set this when correct destination is picked

            // Sets car's boolean flag to no longer be at a taxi stop
            car.atTaxiStop = false;

            // If the car still has rides left in the day—
            if (car.currentRideNum <= car.totalRideNum) {

                // Sets the passenger destination object
                car.carPointer.currentDestinationTile = currentDestination.tile;

                // Start waiting until the passenger talks
                GameStateManager.EOnDestinationSet?.Invoke();
            }
        } else {
            Debug.LogWarning("Wrong address bubbo!");
            entered = false;
        }
    }

    // Sets the map upon clicking the enter
    public void EnterDestination(GPSDestination destination) {

        if (destination.tile != null) {

            gps.dragging = false;
            gps.gameObject.layer = gps.regularLayer;

            GameStateManager.dialogueManager.waitForRouting = false; // Set this when correct destination is picked

            // Sets car's boolean flag to no longer be at a taxi stop
            car.atTaxiStop = false;

            // If the car still has rides left in the day—
            if (car.currentRideNum <= car.totalRideNum) {

                // Sets the passenger destination object
                car.carPointer.currentDestinationTile = destination.tile;

                // Start waiting until the passenger talks
                GameStateManager.EOnDestinationSet?.Invoke();
            }
        } else {
            Debug.LogWarning("Wrong address bubbo!");
            entered = false;
        }
    }
}