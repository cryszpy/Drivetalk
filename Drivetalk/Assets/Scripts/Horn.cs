using UnityEngine;

public class Horn : UIElement
{
    public override void OnClick()
    {
        base.OnClick();
        Debug.Log("Horn pressed!");
    }

    public override void OnHover()
    {
        base.OnHover();
    }
}
