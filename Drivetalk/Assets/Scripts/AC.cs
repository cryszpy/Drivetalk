using UnityEngine;

public class AC : UIElementSlider
{
    public override void Drag()
    {
        base.Drag();

        // Update the player's temperature stat
        float oldRange = rotationMax - rotationMin;
        float newRange = 1 - 0;
        PlayerInfo.Temperature = (((transform.localEulerAngles.y - rotationMin) * newRange) / oldRange) + 0;
    }
}
