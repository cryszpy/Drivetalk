using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;

    private Vector3[] points;

    private void Awake() {
        lr = GetComponent<LineRenderer>();

        lr.enabled = false;
    }

    // Sets up array of points
    public void SetUpLine(Vector3[] points) {
        lr.positionCount = points.Length;

        this.points = points;
    }

    private void Update() {

        // If there are points to track on the GPS—
        if (points != null && points.Length > 0) {

            // Enable the renderer
            lr.enabled = true;

            // For every point—
            for (int i = 0; i < points.Length; i++) {

                // Set the position 50f higher than the roads
                lr.SetPosition(i, new(points[i].x, points[i].y + 10f, points[i].z));
            }
        } 
        // Else, disable the renderer
        else {
            lr.enabled = false;
        }
    }
}
