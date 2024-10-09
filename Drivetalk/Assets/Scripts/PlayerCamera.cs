using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] private float speedY;
    [SerializeField] private float speedX;

    [SerializeField] private float camLimitYMin;
    [SerializeField] private float camLimitYMax;
    [SerializeField] private float camLimitXMin;
    [SerializeField] private float camLimitXMax;

    private float rotationX;
    private float rotationY;

    private void Start() {
        if (!mainCamera) {
            mainCamera = Camera.main;
            Debug.LogWarning("Main camera not set! Reassigned.");
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        rotationY += speedY * Input.GetAxis("Mouse X");
        rotationX -= speedX * Input.GetAxis("Mouse Y");

        // Restricts vertical camera movement
        rotationX = Mathf.Clamp(rotationX, camLimitYMin, camLimitYMax);

        // Restricts horizontal camera movement
        rotationY = Mathf.Clamp(rotationY, camLimitXMin, camLimitXMax);

        transform.eulerAngles = new Vector3(rotationX, rotationY, 0);
    }
}
