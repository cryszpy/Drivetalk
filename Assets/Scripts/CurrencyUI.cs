using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [Tooltip("The text element for the player's current amount of money.")]
    [SerializeField] private TMP_Text currencyText;

    // Used to update the currency UI
    public void SetCurrency (float currency) {

        // If the currency text component exists—
        if (currencyText.TryGetComponent<TextMeshProUGUI>(out var letters)) {

            // Set the currency text to the currency amount
            letters.text = currency.ToString();
        }
    }
}
