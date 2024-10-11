using UnityEngine;
using UnityEngine.AI;

public class PlayerInfo : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    [SerializeField] private CurrencyUI currencyUI;

    [Header("STATS")]

    private static float currency;
    public static float Currency { get => currency; set => currency = value; }
    // Used to update the player's currency statistic (NOT UI)
    public static void AddCurrency(int value) {
        Currency += value;
    }

    private static float temperature;
    public static float Temperature { get => temperature; set => temperature = value;}

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (GameStateManager.Gamestate != GAMESTATE.PAUSED) {
                GameStateManager.SetState(GAMESTATE.PAUSED);
            } else {
                GameStateManager.SetState(GAMESTATE.PLAYING);
            }
        }
    }

}
