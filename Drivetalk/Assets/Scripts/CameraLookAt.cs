using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cameraOrigin;
    [Range(2, 100)] public float cameraTargetDivider;

    [SerializeField] private float distanceZ;

    // Update is called once per frame
    private void Update()
    {
        if (GameStateManager.Gamestate != GAMESTATE.MENU) {

            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ));
            Vector3 cameraTargetPosition = (mousePosition + (cameraTargetDivider - 1) * cameraOrigin.position) / cameraTargetDivider;

            transform.position = cameraTargetPosition;
        }
    }
}
