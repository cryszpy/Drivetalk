using UnityEngine;

[System.Serializable]
public class RequestHazards : DashboardRequestBase {

    public Animator buttonAnimator;

    public override void CheckRequest() {

        // Check if the request has been fulfilled or not, and set it
        fulfilled = CarController.HazardsActive == GameStateManager.car.currentPassenger.preferences.hazardPref;

        Activate(!fulfilled);
    }

    public override void SetNewTarget() {
        changing = true;

        buttonAnimator.SetBool("Active", !CarController.HazardsActive);

        changing = false;
    }
}