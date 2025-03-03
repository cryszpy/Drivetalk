using UnityEngine;

public class RadioPower : UIElementSlider
{

    [SerializeField] private Animator animator;

    // Function to be executed when slider is clicked
    public override void OnClick()
    {
        base.OnClick();

        // Toggle radio power status
        animator.SetBool("Power", !CarController.RadioPower);
    }

    // Called from animator
    public void EndAnimation() {

        // Changes radio power status after animation is done
        CarController.RadioPower = !CarController.RadioPower;
        carPointer.car.radio.audioSource.volume = CarController.RadioPower ? 0.5f : 0f;
    }
}