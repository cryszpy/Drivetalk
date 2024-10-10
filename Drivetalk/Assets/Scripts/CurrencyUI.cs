using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;

    // Used to update the currency UI
    public void SetCurrency (float currency) {

        if (currencyText.TryGetComponent<TextMeshProUGUI>(out var letters)) {
            letters.text = currency.ToString();
        }

    }
}
