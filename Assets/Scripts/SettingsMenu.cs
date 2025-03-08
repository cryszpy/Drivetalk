using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")] // ------------------------------------------------------------------------------------------------------------
    
    public List<TextSpeedPreset> textSpeeds = new();

    public int currentSpeedIndex;

    public TMP_Text textSpeedLabel;

    //[Header("STATS")] // ------------------------------------------------------------------------------------------------------------

    public void Awake()
    {
        // Set the default to the normal speed
        currentSpeedIndex = textSpeeds.IndexOf(textSpeeds.Find(x => x.value == 1));
        CarController.TextSpeedMult = textSpeeds[currentSpeedIndex].value;
        textSpeedLabel.text = textSpeeds[currentSpeedIndex].label;
    }

    // Switches text speed presets to the right
    public void TextSpeedFwd() {

        if (currentSpeedIndex + 1 > textSpeeds.Count - 1) {
            currentSpeedIndex = 0;
        } else {
            currentSpeedIndex += 1;
        }
        CarController.TextSpeedMult = textSpeeds[currentSpeedIndex].value;
        textSpeedLabel.text = textSpeeds[currentSpeedIndex].label;
    }

    // Switches text speed presets to the left
    public void TextSpeedBack() {

        if (currentSpeedIndex - 1 < 0) {
            currentSpeedIndex = textSpeeds.Count - 1;
        } else {
            currentSpeedIndex -= 1;
        }
        CarController.TextSpeedMult = textSpeeds[currentSpeedIndex].value;
        textSpeedLabel.text = textSpeeds[currentSpeedIndex].label;
    }

    // Sets master volume (used in settings sliders)
    public void SetMasterVolume(float level) {
        GameStateManager.audioManager.mainMixer.SetFloat("masterVolume", level);
    }

    // Sets voice volume (used in settings sliders)
    public void SetVoiceVolume(float level) {
        GameStateManager.audioManager.mainMixer.SetFloat("voiceVolume", level);
    }

    // Sets music volume (used in settings sliders)
    public void SetMusicVolume(float level) {
        GameStateManager.audioManager.mainMixer.SetFloat("musicVolume", level);
    }

    // Sets SFX volume (used in settings sliders)
    public void SetSFXVolume(float level) {
        GameStateManager.audioManager.mainMixer.SetFloat("soundFXVolume", level);
    }

    // Sets ambience SFX volume (used in settings sliders)
    public void SetAmbienceVolume(float level) {
        GameStateManager.audioManager.mainMixer.SetFloat("ambienceSoundsVolume", level);
    }

    // Sets car SFX volume (used in settings sliders)
    public void SetCarSFXVolume(float level) {
        GameStateManager.audioManager.mainMixer.SetFloat("carSoundsVolume", level);
    }
}