using UnityEngine;

public class Hazards : UIElementSlider
{

    [SerializeField] private Animator buttonAnimator;
    [SerializeField] private Animator lightsAnimator;

    // Function to be executed when slider is clicked
    public override void OnClick()
    {
        dragging = true;
        gameObject.layer = regularLayer;

        // Toggle radio power status
        buttonAnimator.SetBool("Active", !CarController.HazardsActive);
    }

    // Called from animator
    public void EndAnimation() {

        // Changes radio power status after animation is done
        CarController.HazardsActive = !CarController.HazardsActive;
        lightsAnimator.SetBool("Active", CarController.HazardsActive);
    }
}