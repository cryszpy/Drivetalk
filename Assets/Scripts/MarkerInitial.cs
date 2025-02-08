using System.Collections.Generic;
using UnityEngine;

public class MarkerInitial : Marker
{
    public void ConnectByRaycast() {
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, 25f)) {

            if (hit.collider.CompareTag("Marker")) {
                ConnectToAdjacentMarkers(new List<Marker> { hit.collider.GetComponent<Marker>()});

                GameStateManager.EOnRoadConnected?.Invoke();
            }
        }
    }
}