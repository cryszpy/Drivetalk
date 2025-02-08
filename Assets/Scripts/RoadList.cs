using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/RoadList")]
public class RoadList : ScriptableObject
{
    
    public List<GameObject> allRoadsList = new();
}
