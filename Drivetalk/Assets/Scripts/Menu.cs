using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu != null) {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu() {

        // UNPAUSE
        if (GameStateManager.Gamestate == GAMESTATE.PAUSED) {
            GameStateManager.SetState(GAMESTATE.PLAYING);
            pauseMenu.SetActive(false);
        } 
        // PAUSE
        else if (GameStateManager.Gamestate == GAMESTATE.PLAYING){
            GameStateManager.SetState(GAMESTATE.PAUSED);
            pauseMenu.SetActive(true);
        }
    }

    public void LoadMainMenu() {
        // load level 0
    }

    public void PlayButton() {

    }

    public void QuitButton() {
        Application.Quit();
        Debug.Log("QUIT!");
    }
}
