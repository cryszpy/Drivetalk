using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/TextSpeedPreset")]
public class TextSpeedPreset : ScriptableObject {

    public float value;
    public string label;
}