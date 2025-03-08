using UnityEngine;

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
    public static GlobalEvent EOnRideFinish;
    public static GlobalEvent EOnDialogueGroupFinish;
    public static GlobalEvent EOnDestinationSet;
    public static GlobalEvent EOnPassengerPickup;
    public static GlobalEvent EOnPassengerDropoff;
    public static GlobalEvent EOnLeftWindow;
    public static GlobalEvent EOnRightWindow;
    public static GlobalEvent EOnBlockRadiusDetection;
    public static GlobalEvent EOnRoadConnected;

    public static CarController car;

    public static DialogueManager dialogueManager;

    public static AudioManager audioManager;

    public static ComfortabilityManager comfortManager;

    public static RoadManager roadManager;

    public GameObject initialRoadPrefab;

    public GameObject initialCarPointerSpawn;

    public GameObject initialCarSpawn;

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

    public void ExitToMainMenu() {

        // Reset initial road
        GameObject initialRoad = Instantiate(initialRoadPrefab, Vector3.zero, Quaternion.identity);

        // Set new initial road
        car.carPointer.initialRoad = initialRoad.GetComponent<ProceduralRoad>();

        // Reset positions of car and carPointer
        car.agent.enabled = false;
        car.carPointer.agent.enabled = false;
        car.carPointer.rb.Move(initialCarPointerSpawn.transform.position, Quaternion.identity);
        car.rb.Move(initialCarSpawn.transform.position, Quaternion.identity);

        // Clear all previous roads
        foreach (var road in car.carPointer.roadQueue) {
            Destroy(road);
        }
        car.carPointer.roadQueue.Clear();
        car.carPointer.directionQueue.Clear();

        // Add new initial road to road list
        car.carPointer.roadQueue.Enqueue(car.carPointer.initialRoad);

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
        car.carPointer.path.Clear();
        car.carPointer.gpsPath.Clear();
        car.carPointer.gpsPathRef.Clear();
        car.carPointer.pathfindingTarget = null;

        car.passengerList.ResetAllPassengers();
        comfortManager.ResetDashboardControls();

        car.carPointer.currentMarker = car.carPointer.FindClosestMarker();
    }

    private void Update() {
        gamestateTracker = Gamestate;
    }
}