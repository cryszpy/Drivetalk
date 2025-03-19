using UnityEngine;

public class Hazards : UIElementSlider
{

    public Animator buttonAnimator;
    [SerializeField] private Animator lightsAnimator;

    // Function to be executed when slider is clicked
    public override void OnClick()
    {
        base.OnClick();

        // Toggle hazards status
        buttonAnimator.SetBool("Active", !CarController.HazardsActive);
    }

    // Called from animator
    public void EndAnimation() {

        // Changes hazards status after animation is done
        CarController.HazardsActive = !CarController.HazardsActive;
        lightsAnimator.SetBool("Active", CarController.HazardsActive);
    }
}