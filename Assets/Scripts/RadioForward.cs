using System;
using UnityEngine;

public class RadioForward : UIElementButton
{
    public Radio radio;

    public override void Update() {
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

    public override void OnClick()
    {
        base.OnClick();

        SkipRadioSong();

        Debug.Log("Radio song skipped!");
    }

    public override void OnHover()
    {
        base.OnHover();
    }

    public void SkipRadioSong() {

        // Reset song progress tracker
        radio.isSongPlaying = false;
        radio.songTimer = 0;

        radio.audioSource.Stop();

        radio.IncrementSongNumber(1);
        
        radio.SetRadioSong(radio.currentSongIndex);
    }
}
