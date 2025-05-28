using TMPro;
using UnityEngine;

public class TranscriptElement : MonoBehaviour
{
    [Tooltip("The dialogue text of this item.")]
    [SerializeField] private TMP_Text itemText;

    [Tooltip("The dialogue name text of this item.")]
    [SerializeField] private TMP_Text nameText;

    public void SetText(string text, string name) {
        itemText.text = name + ": " + text;
        //nameText.text = name;
    }
}