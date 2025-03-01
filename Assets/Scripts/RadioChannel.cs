using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ChannelRange {

    public int channelNum;

    public float rangeMin;
    public float rangeMax;
}

public class RadioChannel : UIElementSlider
{

    [Header("RADIO SCRIPT REFERENCES")]

    public Radio radio;

    public List<ChannelRange> channels = new();

    [Header("RADIO STATS")]

    public float turnSpeed;
    public float turnWaitTime;

    private bool turning = false;

    public override void Start() {
        base.Start();
        CarController.CurrentRadioChannel = 0;
    }

    public override void Update()
    {
        base.Update();

        if (GameStateManager.Gamestate == GAMESTATE.PLAYING) {

            // Update the car's temperature statistic to use in passenger happiness calculations
            float oldRange = rotationMax - rotationMin;
            float newRange = 1 - 0;
            float val = ((dialObject.transform.localEulerAngles.z - rotationMin) * newRange / oldRange) + 0;

            int channelCounter = 0;

            for (int i = 0; i < channels.Count; i++) {

                ChannelRange channel = channels[i];

                // Dial is turned to correct range
                if (i == channels.Count - 1 && val >= channel.rangeMin && val <= channel.rangeMax) {
                    channelCounter++;

                    // If channel is not on newest selected channel—
                    if (CarController.CurrentRadioChannel != channel.channelNum) {
                        
                        // Change the channel
                        ChangeChannel(channel.channelNum);
                    }
                }
                else if (val >= channel.rangeMin && val < channel.rangeMax) {
                    channelCounter++;

                    // If channel is not on newest selected channel—
                    if (CarController.CurrentRadioChannel != channel.channelNum) {
                        
                        // Change the channel
                        ChangeChannel(channel.channelNum);
                    }
                }
            }
            
            // If no channels are selected—
            if (channelCounter <= 0 && CarController.CurrentRadioChannel != -1) {

                // Play radio static
                if (radio.radioStatic) {
                    ChangeChannel(-1);
                }
            }
        }
    }

    // Changes the channel of the radio (called ONLY from THIS UPDATE FUNCTION)
    public void ChangeChannel(int index) {

        // Set static if no channel is provided
        if (index == -1) {
            radio.SetRadioStatic();
            return;
        }

        // Set channel number
        CarController.CurrentRadioChannel = index;

        // Set current song and song color based on current channel
        radio.currentSong = radio.songs[index];
        radio.currentColor = radio.songColors[index];

        // Sets audio file to be played
        radio.audioSource.clip = radio.currentSong;

        // Sets radio waveform
        radio.lineRenderer.colorGradient = radio.currentColor;

        // Plays the target song
        radio.isSongPlaying = true;
        radio.audioSource.Play();

        Debug.Log("Currently playing: " + radio.currentSong.name);
    }

    public override void Drag() {

        // Check whether the player has stopped dragging slider
        if (Input.GetMouseButtonUp(0)) {
            if (carPointer.hoveredButton == gameObject) {
                carPointer.hoveredButton = null;
                hovered = false;
                dragging = false;
            }
        }

        if (dialObject) {
            
            // Calculates the difference between the current mouse's position and mouse's previous position
            mousePosDelta = Input.mousePosition - mousePreviousPos;

            // Get the mouse's difference in position applied to the slider's desired rotation axis
            change = Vector3.Dot(mousePosDelta, new Vector3(1, 0, 0));

            float currentRot = dialObject.transform.localEulerAngles.z;
            float newRot = currentRot + change;

            // Limit slider rotation to be between a certain minimum and maximum degree angle
            newRot = Mathf.Clamp(newRot, rotationMin, rotationMax);

            // Apply change in rotation based on mouse cursor movement
            dialObject.transform.localEulerAngles = new Vector3(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, newRot);
        }
    }

    public virtual IEnumerator TurnDial(int index, bool skip = default) {
        turning = true;

        float oldRange = rotationMax - rotationMin;
        float newRange = 1 - 0;

        // Calculate target rotation in degrees (from 30-150)
        float targetRot = (channels[index].rangeMax + channels[index].rangeMin) / 2 * oldRange / newRange + rotationMin;

        // Get the current rotation in degrees
        float currentRot = dialObject.transform.localEulerAngles.z;

        // While the current rotation does not match the target rotation—
        while (!(currentRot <= targetRot + 0.5f && currentRot >= targetRot - 0.5f)) {

            // If the radio has looped through all channels—
            if (skip) {

                // Skip animation and just set the dial rotation
                dialObject.transform.localEulerAngles = new Vector3(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, targetRot);
                break;
            }

            // Get the current rotation every frame
            currentRot = dialObject.transform.localEulerAngles.z;

            float newRot = currentRot;

            // Change rotation in the right direction based on target
            if (targetRot < currentRot) {
                newRot -= turnSpeed;
            } else {
                newRot += turnSpeed;
            }

            // Limit slider rotation to be between a certain minimum and maximum degree angle
            newRot = Mathf.Clamp(newRot, rotationMin, rotationMax);

            // Apply change in rotation based on mouse cursor movement
            dialObject.transform.localEulerAngles = new Vector3(dialObject.transform.localEulerAngles.x, dialObject.transform.localEulerAngles.y, newRot);

            yield return new WaitForSeconds(turnWaitTime);
        }

        turning = false;
    }
}