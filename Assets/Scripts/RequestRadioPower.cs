using System.Collections;
using UnityEngine;

[System.Serializable]
public class RequestRadioPower : DashboardRequestBase {

    public Animator animator;

    public override void CheckRequest() {

        // Check if the request has been fulfilled or not, and set it
        fulfilled = CarController.RadioPower == GameStateManager.car.currentPassenger.preferences.radioPower;

        Activate(!fulfilled);
    }

    public override void SetNewTarget() {
        changing = true;

        animator.SetBool("Power", !CarController.RadioPower);

        changing = false;
    }
}