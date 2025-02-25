using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioWaveform : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")] // -----------------------------------------------------------------------------------

    [SerializeField] private Radio radio;

    [SerializeField] protected LineRenderer lineRenderer;

    [SerializeField] private GameObject startPosition;

    [SerializeField] private GameObject endPosition;

    [Header("WAVEFORM STATS")] // -----------------------------------------------------------------------------------

    [Range(2, 25)]
    [Tooltip("The amount of segments in the line.")]
    public int maxSegmentCount = 2;
    protected int segmentCount;

    [Range(0, 1.5f)]
    [Tooltip("The scale of randomness applied to every segment excluding start and end.")]
    public float variationScale = 0.5f;

    [Tooltip("Minimum possible length of line segments.")]
    public float minSegmentLength = 0.5f;

    [Tooltip("How fast segments move to their new positions if randomness is enabled.")]
    public float segmentMoveSpeed = 5f;

    [Range(0, 30)]
    [Tooltip("How often the segment randomization occurs. (s)")]
    public float randomizeTime;
    protected float randomizeTimer = 0;
    protected bool canRandomize = true;

    protected List<Vector3> positions;

    public void Start()
    {
        segmentCount = maxSegmentCount;
        lineRenderer.colorGradient = radio.currentColor;
    }

    public virtual void Update() {

        UpdateVisual(CarController.RadioPower);

        // Line segment randomization timer
        if (!canRandomize) {
            randomizeTimer += Time.deltaTime;

            if (randomizeTimer > randomizeTime) {
                canRandomize = true;
                randomizeTimer = 0;
            }
        }

        if (lineRenderer.enabled == true && positions.Count > 0) {
            MoveSegments();
        }
    }

    public virtual void UpdateVisual(bool value) {

        switch (value) {
            case true:

                // Sets start value
                Vector3 start = startPosition.transform.localPosition;
                Vector3 end = endPosition.transform.localPosition;

                // Sets starting position
                lineRenderer.SetPosition(0, start);

                // Max out segment counts at the start
                segmentCount = maxSegmentCount;

                // Get the appropriate equal length for every segment
                Vector3 maxPosition = Vector3.Lerp(start, end, 1f / segmentCount);

                // If the segment lengths' are too small—
                while ((maxPosition - start).magnitude < minSegmentLength && segmentCount > 2) {

                    // Reduce the number of segments
                    if (segmentCount - 1 > 2) {
                        segmentCount--;
                    } else {
                        segmentCount = 2;
                    }

                    // Recalculate segment lengths
                    maxPosition = Vector3.Lerp(start, end, 1f / segmentCount);
                }

                // Initializes list if empty
                if (positions == null || positions.Count <= 0 || positions.Count != segmentCount) {
                    positions = new (segmentCount);

                    for (int i = 0; i < segmentCount; i++) {
                        positions.Add(Vector3.zero);
                    }
                }

                // Sets the number of line segments in the laser
                lineRenderer.positionCount = segmentCount;

                // Sets positions in between start and end
                for (int i = 1; i < segmentCount; i++) {

                    Vector3 distance = end - start;

                    Vector3 position = Vector3.Lerp(start, end, (float)i / segmentCount);

                    // Sets the initial position when laser is first fired
                    if (positions[i] == Vector3.zero) {
                        positions[i] = position;
                        lineRenderer.SetPosition(i, position);
                    }

                    // Randomize at a certain time interval
                    if (canRandomize) {

                        Vector3 perpendicular = Vector3.Cross(distance, new(0, 0, 1)).normalized * (Random.Range(-1f, 1f) * variationScale);

                        positions[i] = position + perpendicular;
                    }
                }

                // Reset randomization
                if (canRandomize) {
                    canRandomize = false;
                }

                // Sets the end position
                lineRenderer.SetPosition(segmentCount - 1, end);

                if (lineRenderer.enabled == false) {
                    lineRenderer.enabled = true;
                }

                break;
            case false:

                if (lineRenderer.enabled == true) {
                    lineRenderer.enabled = false;
                }

                positions?.Clear();
                break;
        }
    }

    public virtual void MoveSegments() {

        // For every segment in the line renderer—
        for (int i = 1; i < lineRenderer.positionCount - 1; i++) {

            // Get current position
            Vector3 currentPos = lineRenderer.GetPosition(i);

            lineRenderer.SetPosition(i, Vector3.MoveTowards(currentPos, positions[i], segmentMoveSpeed * Time.deltaTime));
        }
    }
}