using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

    [SerializeField] private CurrencyUI currencyUI;
    private static float currency;
    public static float Currency { get => currency; set => currency = value; }
    // Used to update the player's currency statistic (NOT UI)
    public static void AddCurrency(int value) {
        Currency += value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
