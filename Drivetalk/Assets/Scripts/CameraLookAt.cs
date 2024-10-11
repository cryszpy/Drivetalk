using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] private CarController car;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerCamera playerCam;
    [SerializeField] private Transform cameraOrigin;
    [Range(2, 100)] [SerializeField] private float cameraTargetDivider;

    [SerializeField] private float distanceZ;

    // Update is called once per frame
    private void Update()
    {
        if (GameStateManager.Gamestate != GAMESTATE.MENU) {

            var mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ));
            var cameraTargetPosition = (mousePosition + (cameraTargetDivider - 1) * cameraOrigin.position) / cameraTargetDivider;

            transform.position = new Vector3(cameraTargetPosition.x, cameraTargetPosition.y, transform.position.z);
        }
    }
}
