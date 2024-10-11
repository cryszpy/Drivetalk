using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] public float speedY;
    [SerializeField] public float speedX;

    [SerializeField] private float camLimitYMin;
    [SerializeField] private float camLimitYMax;
    [SerializeField] private float camLimitXMin;
    [SerializeField] private float camLimitXMax;

    public float rotationX;
    public float rotationY;

    private void Start() {
        if (!mainCamera) {
            mainCamera = Camera.main;
            Debug.LogWarning("Main camera not set! Reassigned.");
        }
        //Cursor.lockState = CursorLockMode.Locked;
        GameStateManager.SetState(GAMESTATE.PLAYING);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.Gamestate != GAMESTATE.MENU) {
            
        }
    }
}
