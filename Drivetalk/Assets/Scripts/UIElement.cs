using UnityEngine;
using UnityEngine.Events;

public enum UIElementType {
    BUTTON, SLIDER
}

public class UIElement : MonoBehaviour
{
    public UIElementType elementType;

    public UnityEvent unityEvent = new UnityEvent();

    public GameObject hoveredObject;

    public Camera mainCamera;

    public virtual void Start() {
        if (!mainCamera) {
            mainCamera = Camera.main;
            Debug.LogWarning("Main camera not assigned! Reassigned.");
        }
    }

    public virtual void Update() {

        if (GameStateManager.Gamestate != GAMESTATE.MENU) {

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            int layerMask = ~(1 << 7);

            if (Physics.Raycast(ray, out RaycastHit hit, 25f, layerMask) && hit.collider.gameObject == gameObject)
            {
                switch (elementType)
                {

                    case UIElementType.BUTTON:
                        OnHover();
                        if (Input.GetMouseButtonDown(0))
                        {
                            unityEvent.Invoke();
                        }
                        break;
                    case UIElementType.SLIDER:
                        OnHover();
                        if (Input.GetMouseButton(0))
                        {
                            unityEvent.Invoke();
                        }
                        break;
                }
            } else {
                DefaultState();
            }
        }
    }

    public virtual void OnClick() {
        if (hoveredObject.activeSelf) {
            hoveredObject.SetActive(false);
        }
    }

    public virtual void OnHover() {
        if (!hoveredObject.activeSelf) {
            hoveredObject.SetActive(true);
        }
    }

    public virtual void DefaultState() {
        if (hoveredObject.activeSelf) {
            hoveredObject.SetActive(false);
        }
    }

}
