using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Tooltip("The main mixer to use for the audio in the game.")]
    public AudioMixer mainMixer;

    [Tooltip("Array of most sounds to be played asynchronously.")]
    public Sound[] sounds;

    public Sound vo;

    void Awake() {

        // Singleton pattern
        /* if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } */

        // Loops through each sound in the array
        foreach (Sound s in sounds) {

            // If the audio source location object is null, plays from AudioManager object
            if (s.location == null) {
                s.source = gameObject.AddComponent<AudioSource>();
            } else if (s.location.TryGetComponent<AudioSource>(out var foundSource)) {
                s.source = foundSource;
            } else {
                s.source = s.location.AddComponent<AudioSource>();
            }

            // Assigns sound stats (volume, pitch, 2D/3D spatial blend, etc)
            s.source.clip = s.clips[0];
            s.source.outputAudioMixerGroup = s.mixerGroup;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.panStereo = s.stereoPan;
            s.source.playOnAwake = s.playOnAwake;
            s.source.loop = s.loop;

            // If sound needs to be played when created, play it
            if (s.playOnAwake) {

                switch (s.type) {
                    case SoundType.SINGLE:
                        PlaySoundByName(s.name);
                        break;
                    case SoundType.RANDOM:
                        PlayRandomSoundByName(s.name);
                        break;
                }
            }
        }
    }

    // Plays a sound from a specific audio file
    public void PlaySoundByFile(AudioClip clip) {

        // Finds the file in question
        Sound s = Array.Find(sounds, sound => sound.clips.Contains(clip));
        
        // If file exists—
        if (s != null) {
            Debug.Log("Playing sound: " + s.name + "- " + clip.name);

            // Sets the audio file to be played
            s.source.clip = s.clips[Array.FindIndex(s.clips, file => file == clip)];

            // Plays sound
            s.source.Play();
        } else {
            Debug.LogWarning("Could not find sound that contains file: " + clip.name);
        }
    }

    public void PlayVoiceLine(AudioClip clip, GameObject passenger) {

        if (!clip || !passenger) {
            Debug.LogError("Voice line clip or passenger location is missing!");
            return;
        }

        // If the passenger already has an audio source, use that one
        if (passenger.TryGetComponent<AudioSource>(out var foundSource)) {
            vo.source = foundSource;
        } 
        // Otherwise, add one
        else {
            vo.source = passenger.AddComponent<AudioSource>();
        }

        // Assigns sound stats (volume, pitch, 2D/3D spatial blend, etc)
        vo.source.clip = clip;
        vo.name = clip.name;
        vo.location = passenger;
        vo.source.outputAudioMixerGroup = vo.mixerGroup;
        vo.source.volume = vo.volume;
        vo.source.pitch = vo.pitch;
        vo.source.spatialBlend = vo.spatialBlend;
        vo.source.panStereo = vo.stereoPan;
        vo.source.playOnAwake = vo.playOnAwake;
        vo.source.loop = vo.loop;

        Debug.Log("Playing sound: " + vo.name + "- " + clip.name);
        vo.source.Play();
    }

    // Plays a sound by name
    public void PlaySoundByName(string name) {

        // Finds the sound in question
        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        // If sound exists—
        if (s != null) {
            Debug.Log("Playing sound: " + s.name);

            // Sets the sound to be played
            s.source.clip = s.clips[0];

            // Plays sound
            s.source.Play();
        } else {
            Debug.LogWarning("Could not find sound: " + name);
        }
    }

    // Plays a random version of a sound by name
    public void PlayRandomSoundByName(string name) {

        // Finds the sound type in question
        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        // If sound exists—
        if (s != null) {
            Debug.Log("Playing random sound: " + s.name);

            // Gets a random variation of the sound
            s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];

            // Plays sound
            s.source.Play();
        } else {
            Debug.LogWarning("Could not find sound: " + name);
        }
    }

    // Sets master volume (used in settings sliders)
    public void SetMasterVolume(float level) {
        mainMixer.SetFloat("masterVolume", level);
    }

    // Sets voice volume (used in settings sliders)
    public void SetVoiceVolume(float level) {
        mainMixer.SetFloat("voiceVolume", level);
    }

    // Sets music volume (used in settings sliders)
    public void SetMusicVolume(float level) {
        mainMixer.SetFloat("musicVolume", level);
    }

    // Sets SFX volume (used in settings sliders)
    public void SetSFXVolume(float level) {
        mainMixer.SetFloat("soundFXVolume", level);
    }

    // Sets ambience SFX volume (used in settings sliders)
    public void SetAmbienceVolume(float level) {
        mainMixer.SetFloat("ambienceSoundsVolume", level);
    }

    // Sets car SFX volume (used in settings sliders)
    public void SetCarSFXVolume(float level) {
        mainMixer.SetFloat("carSoundsVolume", level);
    }
}