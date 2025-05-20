using UnityEngine;

public class HeldWord : MonoBehaviour
{

    [HideInInspector] public Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        Debug.Log("Awake: " + transform.position);
    }

    private void Start()
    {
        Debug.Log("Start " + transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        MovePosition();
    }

    public void MovePosition() {
        transform.position = GameStateManager.instance.ConvertToCanvasSpace(Input.mousePosition, canvas);
    }

    public Vector3 LerpConvertToCanvasSpace(Vector3 original, Canvas c) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(c.transform as RectTransform, original, c.worldCamera, out Vector2 pos);
        return Vector3.Lerp(transform.position, c.transform.TransformPoint(pos), Time.deltaTime * 30f);
    }
}
