using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ScriptableVariable<T> : ScriptableObject
{
    [SerializeField] private T _value;
    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            OnValueChanging?.Invoke(_value);
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

    /// <summary>
    /// Called Value has changed.
    /// </summary>
    public Action<T> OnValueChanged;
    /// <summary>
    /// Called before Value has changed.
    /// </summary>
    public Action<T> OnValueChanging;

    [Space]
    [Header("Settings")]
    [SerializeField] private bool _debugging = false;
    [SerializeField] private bool _save;

    public void Serialize()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText($"{Application.persistentDataPath}/{this.name}.json", json);
        Debug.Log("Writing data to: " + $"{Application.persistentDataPath}/{this.name}.json");
    }

    public void Deserialize()
    {
        string json = File.ReadAllText($"{Application.persistentDataPath}/{this.name}.json");
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
