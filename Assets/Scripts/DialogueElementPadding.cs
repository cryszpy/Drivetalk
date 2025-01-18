using UnityEngine;
using UnityEngine.UI;

public class DialogueElementPadding : MonoBehaviour
{

    [SerializeField] private VerticalLayoutGroup layout;

    public int newPadding = 0;

    // Update is called once per frame
    void Update()
    {
        RectOffset tempPadding = new(layout.padding.left, layout.padding.right, layout.padding.top, layout.padding.bottom)
        {
            top = -newPadding
        };

        layout.padding = tempPadding;
    }
}
