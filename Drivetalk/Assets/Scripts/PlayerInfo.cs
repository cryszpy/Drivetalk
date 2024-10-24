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

}
