using UnityEngine;

public class Horn : UIElementButton
{
    public AudioManager audioManager;
    // Function to be executed when the button is clicked
    public override void OnClick()
    {
        base.OnClick();
        audioManager.PlaySoundByName("Horn");
        Debug.Log("Horn pressed!");
    }

    // Function to be executed when the button is hovered
    public override void OnHover()
    {
        base.OnHover();
    }
}
