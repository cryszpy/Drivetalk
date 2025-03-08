using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComfortabilityManager : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    [Tooltip("Reference to the visible mood meter slider.")]
    public Slider moodMeter;

    [Header("STATS")] // --------------------------------------------------------------------------------

    public List<DashboardRequestBase> dashRequests = new();
    public List<DashboardRequestBase> availableRequests;

    public List<DashboardRequestBase> activeRequests = new();
    
    [Tooltip("The current passenger's comfortability level.")]
    public float currentComfortability = 100;

    [Tooltip("Whether the passive comfortability system is currently running or not.")]
    public bool comfortabilityRunning = false;

    public bool happy;

    public float initialCooldown;
    private float initialTimer = 0;

    private void Start()
    {
        availableRequests = new(dashRequests);
    }

    private void Update() {
        
        // If there are a nonzero amount of dashboard requests—
        if (dashRequests.Count > 0) {

            int unfulfilledRequests = 0;

            // For every existing dashboard request—
            foreach (var request in dashRequests) {

                // If the request is unfulfilled, increment the amount of unfulfilled requests
                if (!request.fulfilled && request.active) unfulfilledRequests++;
            }

            // Set the passenger's mood meter accordingly
            happy = unfulfilledRequests <= 0;
        }

        if (GameStateManager.car.currentPassenger && comfortabilityRunning && !GameStateManager.dialogueManager.playingChoices) {

            // If no requests are active, start the gradual ramp
            if (activeRequests.Count != dashRequests.Count) {
                initialTimer += Time.deltaTime;

                if (initialTimer > initialCooldown) {
                    initialTimer = 0;

                    DashboardRequestBase selected = availableRequests[Random.Range(0, availableRequests.Count)];
                    selected.active = true;
                    availableRequests.Remove(selected);
                    activeRequests.Add(selected);
                }
            }

            UpdateMood();
        }
    }

    private void UpdateMood() {

        // Affect the passenger's comfortability
        if (happy) {

            float add = GameStateManager.car.currentPassenger.meterSpeed * Time.deltaTime;

            if ((currentComfortability + add) > moodMeter.maxValue) {
                currentComfortability = moodMeter.maxValue;
            }  else {
                currentComfortability += add;
            }
            
        } else {

            float subtract = GameStateManager.car.currentPassenger.meterSpeed * Time.deltaTime;

            if ((currentComfortability - subtract) < moodMeter.minValue) {
                currentComfortability = moodMeter.minValue;
            }  else {
                currentComfortability -= subtract;
            }
        }

        moodMeter.value = currentComfortability;
    }

    public void ResetDashboardControls() {

        comfortabilityRunning = false;

        activeRequests.Clear();
        availableRequests.Clear();

        currentComfortability = 100;

        initialTimer = 0;

        availableRequests = new(dashRequests);
    }
}