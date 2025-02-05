using System;
using UnityEngine;

public class RadioForward : UIElementSlider
{
    [Tooltip("Reference to the car's radio.")]
    public Radio radio;

    public override void Update() {

        // If the game's state is not in menu or main menu—
        if (GameStateManager.Gamestate == GAMESTATE.PLAYING || GameStateManager.Gamestate == GAMESTATE.MAINMENU) {

            // If this slider is being hovered over—
            if (hovered) {
                OnHover();

                /* // Start minigame
                if (Input.GetMouseButtonDown(0) && dialogueManager.dashRequestRunning)
                {
                    // Execute click function
                    unityEvent.Invoke();
                } 
                // Allow player to fiddle
                else if (Input.GetMouseButton(0) && !dialogueManager.dashRequestRunning) {
                    StartDrag();
                }
                else {
                    dragging = false;
                } */

                if (Input.GetMouseButtonDown(0)) {
                    unityEvent.Invoke();
                }
            }

            // If player is fiddling with dial and no dash request is running
            /* if (dragging && !dialogueManager.dashRequestRunning) {
                Drag();
            } */
            if (dragging) {
                gameObject.layer = hoveredLayer;
                Drag();
            }

            // Updates mouse position every frame
            mousePreviousPos = Input.mousePosition;
        }
    }

    public override void OutlineRaycast() {

        if (GameStateManager.Gamestate != GAMESTATE.PAUSED) {

            // Raycast from the UI element to the mouse cursor
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // If raycast successfully hits mouse cursor (meaning cursor is currently hovered over UI element), and the UI element belongs to this script—
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
            {
                RaycastCheck(hit);
            } 
            // If cursor is not hovered over element, reset to default state
            else {
                DefaultState();
            }
        }
    }

    public override void RaycastCheck(RaycastHit hit)
    {
        // If the raycast has hit this button—
        if (hit.collider.gameObject == gameObject) {

            // If there isn't any other button being hovered—
            if (carPointer.hoveredButton == null) {

                // Set this button to be hovered
                carPointer.hoveredButton = gameObject;
            }

            // If this button is the only button being hovered—
            if (carPointer.hoveredButton == gameObject) {

                // Trigger OnHover effects
                OnHover();
            }
            
        } else {
            DefaultState();
        }
    }

    // Function to be executed when button is clicked
    public override void OnClick()
    {
        Debug.Log("cliked')");
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
