using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [Tooltip("Reference to the main camera.")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("Reference to the main point of focus on the dashboard.")]
    [SerializeField] private Transform cameraOrigin;

    [SerializeField] private GameObject lookLimit;

    [Tooltip("A float that sets how loose the camera's cursor-follow is. Higher values = looser")]
    [Range(2, 100)] public float cameraTargetDivider;

    [Tooltip("Sets a virtual position for the cursor's Z position.")]
    [SerializeField] private float distanceZ;

    // Update is called once per frame
    private void Update()
    {
        // If the game isn't in a menu or paused—
        if (GameStateManager.Gamestate != GAMESTATE.MENU && GameStateManager.Gamestate != GAMESTATE.PAUSED) {

            // Gets the mouse position
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y / 2, distanceZ));

            //Debug.Log(mainCamera.ScreenToWorldPoint(Input.mousePosition) + " : " + mousePosition);

            // Calculates a target position in between the dashboard focus point, and the cursor's active position
            Vector3 cameraTargetPosition = (mousePosition + (cameraTargetDivider - 1) * cameraOrigin.position) / cameraTargetDivider;

            float clamp = Mathf.Clamp(cameraTargetPosition.y, 1.72f, 2.2f);

            Vector3 clampedPosition = new(cameraTargetPosition.x, clamp, cameraTargetPosition.z);

            //Debug.Log((clampedPosition.z - lookLimit.transform.position.z));

            transform.position = clampedPosition;

            // Aims the camera at the target position
            /* if ((clampedPosition.z - lookLimit.transform.position.z) > 0) {
                transform.position = clampedPosition;
            } */
        }
    }
}
