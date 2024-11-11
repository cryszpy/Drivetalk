using UnityEngine;

public class Horn : UIElementButton
{
    // Function to be executed when the button is clicked
    public override void OnClick()
    {
        base.OnClick();
        Debug.Log("Horn pressed!");
    }

    // Function to be executed when the button is hovered
    public override void OnHover()
    {
        base.OnHover();
    }
}
