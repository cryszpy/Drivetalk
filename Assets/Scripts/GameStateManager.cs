using UnityEngine;
using UnityEngine.Tilemaps;

public enum GAMESTATE {
    MAINMENU, MENU, PLAYING, PAUSED, GAMEOVER
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    [Tooltip("The game's current gamestate.")]
    private static GAMESTATE gamestate;
    public static GAMESTATE Gamestate { get => gamestate; }

    // Sets the game's state
    public static void SetState(GAMESTATE newState) {

        // Set's the game's state
        gamestate = newState;

        // Freezes the game's time depending on which state the game is switched to
        Time.timeScale = gamestate switch
        {
            GAMESTATE.MAINMENU => 1,
            GAMESTATE.MENU => 1,
            GAMESTATE.GAMEOVER => 1,
            GAMESTATE.PAUSED => 0,
            GAMESTATE.PLAYING => 1,
            _ => (float)1,
        };
    }

    // Debug tracker for the game's current state
    public GAMESTATE gamestateTracker;

    public delegate void GlobalEvent();
    public static GlobalEvent EOnDestinationSet;
    public static GlobalEvent EOnPassengerDropoff;
    public static GlobalEvent EOnLeftWindow;
    public static GlobalEvent EOnRightWindow;
    public static GlobalEvent EOnRoadConnected;

    public static CarController car;

    public static DialogueManager dialogueManager;

    public static AudioManager audioManager;

    public static ComfortabilityManager comfortManager;

    public static RoadManager roadManager;

    public GameObject initialRoadPrefab;

    public GameObject initialCarPointerSpawn;

    public GameObject initialCarSpawn;

    public GameObject backgroundRoad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        // Singleton pattern
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            FindReferences();
            DontDestroyOnLoad(gameObject);
        }
    }

    public void FindReferences() {
        car = GameObject.FindGameObjectWithTag("CarFrame").transform.parent.GetComponent<CarController>();
        dialogueManager = GetComponent<DialogueManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        roadManager = GetComponent<RoadManager>();
        comfortManager = GetComponent<ComfortabilityManager>();
    }

    public Vector3 ConvertToCanvasSpace(Vector3 original, Canvas c) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(c.transform as RectTransform, original, c.worldCamera, out Vector2 pos);
        return c.transform.TransformPoint(pos);
    }

    public void ExitToMainMenu() {

        // Reset car stuff
        dialogueManager.gpsIndicator.SetActive(false);
        dialogueManager.activeDialogueBlocks.Clear();
        car.gpsScreen.gps.dragging = false;
        car.gpsScreen.gps.gameObject.layer = car.gpsScreen.gps.regularLayer;
        car.gpsScreen.actualText.maxVisibleCharacters = 0;
        car.gpsScreen.inputField.textComponent.text = "";
        car.gpsScreen.inputField.text = "";

        foreach (GameObject tile in car.gpsScreen.recentDestTiles) {
            Destroy(tile);
        }
        car.gpsScreen.recentDestTiles.Clear();

        // Toggle hazards status off
        CarController.HazardsActive = false;
        GameObject.FindGameObjectWithTag("Hazards").GetComponent<Hazards>().buttonAnimator.SetBool("Active", CarController.HazardsActive);

        // Reset dialogue systems
        dialogueManager.ResetDialogue();

        comfortManager.comfortabilityRunning = false;

        // Reset initial road
        GameObject initialRoad = Instantiate(initialRoadPrefab, Vector3.zero, Quaternion.identity);

        // Set new initial road
        car.carPointer.initialRoad = initialRoad.GetComponent<ProceduralRoad>();

        // Assumes initial road is a 4-way
        car.carPointer.furthestConnectionPoint = car.carPointer.initialRoad.roadConnections[0];

        // Reset positions of car and carPointer
        car.agent.enabled = false;
        car.carPointer.agent.enabled = false;
        car.carPointer.rb.Move(initialCarPointerSpawn.transform.position, Quaternion.identity);
        car.rb.Move(initialCarSpawn.transform.position, Quaternion.identity);

        // Clear all previous roads
        foreach (var road in car.carPointer.roadQueue) {
            Destroy(road.gameObject);
        }
        car.carPointer.roadQueue.Clear();
        car.carPointer.directionQueue.Clear();

        car.carPointer.defaultRotation = 0;
        car.carPointer.currentSteeringDirection = SteeringDirection.FORWARD;

        // Remove current passenger
        if (car.currentPassenger) {
            GameObject dead = car.currentPassenger.gameObject;
            car.currentPassenger = null;
            Destroy(dead);
        }

        // Reset passenger stats
        CarController.PassengersDrivenIDs.Clear();
        CarController.PassengersDrivenRideNum.Clear();
        CarController.TotalPassengersDriven = 0;
        car.currentRideNum = 0;

        // Reset car variables
        car.arrivedAtDest = false;
        car.atTaxiStop = false;
        car.carPointer.inIntersection = false;
        car.carPointer.atStopSign = false;
        car.carPointer.path.Clear();
        car.carPointer.gpsPath.Clear();
        car.carPointer.gpsPathRef.Clear();
        car.carPointer.pathfindingTarget = null;
        car.carPointer.destinationMarker = null;

        car.passengerList.ResetAllPassengers();
        comfortManager.ResetDashboardControls();

        car.carPointer.currentMarker = car.carPointer.FindClosestMarker();
    }

    private void Update() {
        gamestateTracker = Gamestate;
    }
}