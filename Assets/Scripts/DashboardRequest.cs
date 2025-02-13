
using UnityEngine;

public enum Request {
    AC, HAZARDS, WIPERS, RADIO_SONG, RADIO_POWER, CAR_SPEED
}

[System.Serializable]
public class DashboardRequestBase : MonoBehaviour {

    [Header("STATS")] // --------------------------------------------------------------------------------

    public GameObject indicator;

    public Request id;

    public float currentChance;
    public float baseChance;
    public float baseIncrease;

    [SerializeField] protected float floatValue;

    [SerializeField] protected bool boolValue;

    public bool fulfilled = false;

    public bool active = false;

    protected bool changing = false;

    [Tooltip("The rate in seconds at which the ricketiness of this control changes.")]
    public float cooldown;
    [SerializeField] protected float cooldownTimer = 0;

    public virtual void Start() {
        currentChance = baseChance;
    }

    public virtual void Update()
    {
        // Activate new dashboard controls if the car has a passenger
        if (GameStateManager.comfortManager.comfortabilityRunning && GameStateManager.car.currentPassenger != null && active) {

            // Checks the dashboard request for completion if it is active
            CheckRequest();

            // Cooldown timer once the request is fulfilled
            if (fulfilled && !changing) {
                cooldownTimer += Time.deltaTime;

                // Activate new target according to cooldown
                if (cooldownTimer > cooldown) {
                    cooldownTimer = 0;

                   // Create a new random target for the selected request
                    RequestChange();
                }

            } else {
                cooldownTimer = 0;
            }
            
        } else {
            cooldownTimer = 0;

            // Reset indicator
            if (indicator.activeInHierarchy) {
                indicator.SetActive(false);
            }
        }
    }

    public virtual void CheckRequest() {
        Activate(!fulfilled);
    }

    // Called when this request's cooldown to change targets is up
    public virtual void RequestChange() {

        float rand = Random.value;

        // Successful roll
        if (rand <= currentChance) {
            currentChance = baseChance;

            // Sets a new target based on the request
            SetNewTarget();
        } 
        // Unsuccessful roll
        else {
            currentChance += baseIncrease;
        }

        return;
    }

    public virtual void SetNewTarget() {
        changing = true;
        // Put stuff here
        changing = false;
        return;
    }

    public virtual void Activate(bool value) {

        // If control is not active, don't enable visual even if control isn't fulfilled
        if (active) {
            indicator.SetActive(value);
        } else {
            indicator.SetActive(false);
        }
    }
}