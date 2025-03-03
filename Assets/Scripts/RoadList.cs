using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/RoadList")]
public class RoadList : ScriptableObject
{
    
    public List<GameObject> allRoadsList = new();

    public List<GPSDestination> destinations = new();
}

[System.Serializable]
public class GPSDestination {

    public GameObject tile;

    public string gpsText;
}