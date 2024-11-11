using System.Collections.Generic;
using UnityEngine;

public class TranscriptLog : MonoBehaviour
{
    [SerializeField] private GameObject textTemplate;

    private List<GameObject> logItems = new();

    public void LogText(string text, string name) {

        if (logItems.Count == 10) {
            GameObject tempItem = logItems[0];
            Destroy(tempItem);
            logItems.Remove(tempItem);
        }

        GameObject newText = Instantiate(textTemplate) as GameObject;
        newText.SetActive(true);

        newText.GetComponent<TranscriptLogItem>().SetText(text, name);
        newText.transform.SetParent(textTemplate.transform.parent, false);

        logItems.Add(newText);
    }
}
