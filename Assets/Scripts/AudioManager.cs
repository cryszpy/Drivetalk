using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mainMixer;

    public Sound[] sounds;

    void Awake() {

        // Singleton pattern
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds) {

            // If the audio source location object is null, plays from AudioManager object
            if (s.location == null) {
                s.source = gameObject.AddComponent<AudioSource>();
            } else {
                s.source = s.location.AddComponent<AudioSource>();
            }

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

    public void PlaySoundByFile(AudioClip clip) {

        Sound s = Array.Find(sounds, sound => sound.clips.Contains(clip));
        
        if (s != null) {
            Debug.Log("Playing sound: " + s.name + "- " + clip.name);

            s.source.clip = s.clips[Array.FindIndex(s.clips, file => file == clip)];

            s.source.Play();
        } else {
            Debug.LogWarning("Could not find sound that contains file: " + clip.name);
        }
    }

    public void PlaySoundByName(string name) {

        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        if (s != null) {
            Debug.Log("Playing sound: " + s.name);

            s.source.clip = s.clips[0];

            s.source.Play();
        } else {
            Debug.LogWarning("Could not find sound: " + name);
        }
    }

    public void PlayRandomSoundByName(string name) {

        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        if (s != null) {
            Debug.Log("Playing random sound: " + s.name);

            s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];

            s.source.Play();
        } else {
            Debug.LogWarning("Could not find sound: " + name);
        }
    }

    public void SetMasterVolume(float level) {
        mainMixer.SetFloat("masterVolume", level);
    }

    public void SetMusicVolume(float level) {
        mainMixer.SetFloat("musicVolume", level);
    }

    public void SetSFXVolume(float level) {
        mainMixer.SetFloat("soundFXVolume", level);
    }

    public void SetAmbienceVolume(float level) {
        mainMixer.SetFloat("ambienceSoundsVolume", level);
    }

    public void SetCarSFXVolume(float level) {
        mainMixer.SetFloat("carSoundsVolume", level);
    }
}
