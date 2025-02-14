using System.Collections.Generic;
using UnityEngine;

public class ProceduralRoad : MonoBehaviour
{
    private CarPointer carPointer;

    public GameObject center;

    public MarkerInitial initialMarker;

    public List<GameObject> taxiStops;

    public List<RoadConnectionPoint> roadConnections = new();

    /* private void Start() {
        carPointer = GameObject.FindGameObjectWithTag("CarPointer").GetComponent<CarPointer>();

        // Adds all taxi stops in this road to the car's list of taxi stops
        foreach (var stop in taxiStops) {
            if (!carPointer.car.taxiStops.Contains(stop)) {
                carPointer.car.taxiStops.Add(stop);
            }
        }
    }

    private void OnDisable() {

        if (carPointer) {

            // Removes all taxi stops in this road from the car's list of taxi stops
            foreach (var stop in taxiStops) {
                if (carPointer.car.taxiStops.Contains(stop)) {
                    carPointer.car.taxiStops.Remove(stop);
                }
            }
        }
    } */
}