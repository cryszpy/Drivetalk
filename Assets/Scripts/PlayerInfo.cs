using UnityEngine;
using UnityEngine.AI;

public class PlayerInfo : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    [Tooltip("Reference to the currency UI element.")]
    [SerializeField] private CurrencyUI currencyUI;

    [Header("STATS")]

    [Tooltip("Player's currency statistic.")]
    private static float currency;
    public static float Currency { get => currency; set => currency = value; }

    // Used to update the player's currency statistic (NOT UI)
    public static void AddCurrency(int value) {
        Currency += value;
    }

}
