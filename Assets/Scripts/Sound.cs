using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public SoundType type;

    public string name;

    public GameObject location;

    public AudioClip[] clips;

    public AudioMixerGroup mixerGroup;

    [Range(0f, 1f)]
    public float volume;

    [Range(-3f, 3f)]
    public float pitch = 1;

    [Range(-1f, 1f)]
    public float stereoPan = 0;

    [Tooltip("0 = 2D, 1 = 3D")]
    [Range(0f, 1f)]
    public float spatialBlend = 0;

    public bool playOnAwake;
    public bool loop;

    [HideInInspector] public AudioSource source;
}

public enum SoundType {
    SINGLE, RANDOM
}