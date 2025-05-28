using TMPro;
using UnityEngine;

public class GPSRecentDestination : MonoBehaviour
{
    [Tooltip("The destination tile linked to this button.")]
    public GameObject destinationObject;

    public TMP_Text locationText;

    private GPSScreen gpsScreen;

    private GPSDestination thisDestination;

    public void SetStats(GPSDestination destination) {

        thisDestination = destination;

        // Sets text and linked destination
        locationText.text = destination.gpsText;
        destinationObject = destination.tile;
    }

    // Called from Unity UI Event
    public void OnClick() {
        gpsScreen.EnterDestination(thisDestination);
    }

    public static UnityEngine.GameObject Create(UnityEngine.Object original, Transform parent, GPSScreen screen) {
        GameObject dest = Instantiate(original, parent) as GameObject;
        
        if (dest.TryGetComponent<GPSRecentDestination>(out var script)) {
            script.gpsScreen = screen;
            script.SetStats(screen.currentDestination);
            return dest;
        } else {
            Debug.LogError("Could not find GPSRecentDestination script or extension of such on this GameObject.");
            return null;
        }
    }
}