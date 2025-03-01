using UnityEngine;

public enum GAMESTATE {
    MAINMENU, MENU, PLAYING, PAUSED, GAMEOVER
}

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        // Singleton pattern
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            car = GameObject.FindGameObjectWithTag("CarFrame").transform.parent.GetComponent<CarController>();
            dialogueManager = GetComponent<DialogueManager>();
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
            roadManager = GetComponent<RoadManager>();
            comfortManager = GetComponent<ComfortabilityManager>();
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update() {
        gamestateTracker = Gamestate;
    }
}