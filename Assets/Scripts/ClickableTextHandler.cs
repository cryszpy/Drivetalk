using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class ClickableTextHandler : MonoBehaviour, IPointerDownHandler
{

    private TMP_Text textBox;

    private Canvas canvas;

    private Camera mainCam;

    public delegate void ClickOnLinkEvent(string keyword);
    public static event ClickOnLinkEvent EOnLinkClick;

    public static event ClickOnLinkEvent EOnLinkRelease;

    private void Awake()
    {
        textBox = GetComponent<TMP_Text>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
            mainCam = null;
        } else {
            mainCam = canvas.worldCamera;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 mousePos = new(eventData.position.x, eventData.position.y, 0);

        int linkTaggedText = TMP_TextUtilities.FindIntersectingLink(textBox, mousePos, mainCam);

        if (linkTaggedText != -1) {
            TMP_LinkInfo linkInfo = textBox.textInfo.linkInfo[linkTaggedText];

            EOnLinkClick?.Invoke(linkInfo.GetLinkText());
        }
    }
}
