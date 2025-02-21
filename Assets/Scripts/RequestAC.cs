using System.Collections;
using UnityEngine;

[System.Serializable]
public class RequestAC : DashboardRequestBase {

    public UIElementSlider acDial;

    public float dialWaitTime;
    public float dialSpeed;

    public override void CheckRequest() {

        // Check if the request has been fulfilled or not, and set it
        fulfilled = CarController.Temperature >= GameStateManager.car.currentPassenger.preferences.acMin && CarController.Temperature <= GameStateManager.car.currentPassenger.preferences.acMax;

        Activate(!fulfilled);


        /* return request.id switch
        {
            Request.AC => ,
            Request.HAZARDS => request.boolValue == car.currentPassenger.preferences.hazardPref,
            Request.WIPERS => request.boolValue == car.currentPassenger.preferences.wipersPref,
            Request.RADIO_SONG => CarController.CurrentRadioChannel == car.currentPassenger.preferences.radioSongId,
            Request.RADIO_POWER => request.boolValue == car.currentPassenger.preferences.radioPower,
            Request.CAR_SPEED => false,
            _ => false,
        }; */
    }

    public override void SetNewTarget() {
        changing = true;

        switch (CarController.Temperature) {

            case < 0.5f:
                floatValue = acDial.rotationMax;
                break;
            
            case > 0.5f:
                floatValue = acDial.rotationMin;
                break;

            case 0.5f:
                int rand = Random.Range(0, 2);

                if (rand == 1) {
                    floatValue = acDial.rotationMax;
                } else {
                    floatValue = acDial.rotationMin;
                }
                break;
        }

        StartCoroutine(MoveDial(floatValue));
    }

    private IEnumerator MoveDial(float value) {

        switch(value) {

            case < 90f:
                Debug.Log("bruh");

                // While the dial is not set to its targeted rotation, and the player isn't messing with it—
                while (acDial.dialObject.transform.localEulerAngles.y >= floatValue && !acDial.dragging) {

                    yield return new WaitForSeconds(dialWaitTime);

                    // Move rotation towards target rotation
                    float newRot = acDial.dialObject.transform.localEulerAngles.y - dialSpeed;

                    // Apply change in rotation
                    acDial.dialObject.transform.localEulerAngles = new Vector3(acDial.dialObject.transform.localEulerAngles.x, newRot, acDial.dialObject.transform.localEulerAngles.z);
                }
                break;

            case > 90f:

                // While the dial is not set to its targeted rotation, and the player isn't messing with it—
                while (acDial.dialObject.transform.localEulerAngles.y <= floatValue && !acDial.dragging) {

                    yield return new WaitForSeconds(dialWaitTime);

                    // Move rotation towards target rotation
                    float newRot = acDial.dialObject.transform.localEulerAngles.y + dialSpeed;

                    // Apply change in rotation
                    acDial.dialObject.transform.localEulerAngles = new Vector3(acDial.dialObject.transform.localEulerAngles.x, newRot, acDial.dialObject.transform.localEulerAngles.z);
                }
                break;

            case 90f:
                Debug.LogError("Target float value is exactly 90f!");
                break;
        }

        changing = false;
    }
}