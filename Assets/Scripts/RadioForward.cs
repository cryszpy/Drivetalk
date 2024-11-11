using System;
using UnityEngine;

public class RadioForward : UIElementButton
{
    [Tooltip("Reference to the car's radio.")]
    public Radio radio;

    public override void Update() {

        // If button is hovered over—
        if (hovered) {
            // If this UI element is clicked—
            if (Input.GetMouseButtonDown(0))
            {
                // Execute click function
                unityEvent.Invoke();
            }
        }
    }

    public override void FixedUpdate() {

        // Raycast from the UI element to the mouse cursor
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // If raycast successfully hits mouse cursor (meaning cursor is currently hovered over UI element), and the UI element belongs to this script—
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
        {
            if (hit.collider.transform == transform) {
                // Execute hover function
                OnHover();
            }
            
        } 
        // If cursor is not hovered over element, reset to default state
        else {
            DefaultState();
        }
    }

    // Function to be executed when button is clicked
    public override void OnClick()
    {
        base.OnClick();

        // Skips to the next radio song
        SkipRadioSong();

        Debug.Log("Radio song skipped!");
    }

    // Skips to the next radio song
    public void SkipRadioSong() {

        // Reset song progress tracker
        radio.isSongPlaying = false;
        radio.songTimer = 0;

        // Stops current song
        radio.audioSource.Stop();

        // Increments song number
        radio.IncrementSongNumber(1);
        
        // Sets current radio song to incremented index number
        radio.SetRadioSong(radio.currentSongIndex);
    }
}
