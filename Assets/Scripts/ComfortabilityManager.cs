using System.Collections.Generic;
using UnityEngine;

public class ComfortabilityManager : MonoBehaviour
{

    [Header("STATS")] // --------------------------------------------------------------------------------

    public List<DashboardRequestBase> dashRequests = new();

    public List<DashboardRequestBase> activeRequests = new();
    
    [Tooltip("The current passenger's comfortability level.")]
    public float currentComfortability;

    [Tooltip("Whether the passive comfortability system is currently running or not.")]
    public bool comfortabilityRunning = false;

    public bool happy;

    public float initialCooldown;
    private float initialTimer = 0;

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

        if (GameStateManager.car.currentPassenger && comfortabilityRunning) {

            // If no requests are active, start the gradual ramp
            if (activeRequests.Count != dashRequests.Count) {
                initialTimer += Time.deltaTime;

                if (initialTimer > initialCooldown) {
                    initialTimer = 0;

                    DashboardRequestBase selected = dashRequests[Random.Range(0, dashRequests.Count)];
                    selected.active = true;
                    activeRequests.Add(selected);
                }
            }

            // Affect the passenger's comfortability
            if (happy) {
                currentComfortability += GameStateManager.car.currentPassenger.meterSpeed * Time.deltaTime;
            } else {
                currentComfortability -= GameStateManager.car.currentPassenger.meterSpeed * Time.deltaTime;
            }
        }
    }
}