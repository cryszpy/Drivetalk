using UnityEngine;

public enum GAMESTATE {
    MENU, PLAYING, PAUSED, GAMEOVER
}

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;

    private static GAMESTATE gamestate;
    public static GAMESTATE Gamestate { get => gamestate; }
    public static void SetState(GAMESTATE newState) {
        gamestate = newState;
        Time.timeScale = gamestate switch
        {
            GAMESTATE.MENU => 1,
            GAMESTATE.GAMEOVER => 1,
            GAMESTATE.PAUSED => 0,
            GAMESTATE.PLAYING => 1,
            _ => (float)1,
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Singleton pattern
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
