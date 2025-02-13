using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/DashboardPreference")]
public class DashboardPreference : ScriptableObject
{
    public float acMin;
    public float acMax;

    public int radioSongId;

    public bool hazardPref;

    public bool wipersPref;

    public bool radioPower;
}