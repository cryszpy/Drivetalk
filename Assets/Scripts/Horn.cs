using UnityEngine;

public class Horn : UIElementSlider
{
    public AudioManager audioManager;
    // Function to be executed when the button is clicked
    public override void OnClick()
    {
        base.OnClick();
        audioManager.PlaySoundByName("Horn");
        Debug.Log("Horn pressed!");
    }
}