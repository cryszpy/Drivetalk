using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUIElement : MonoBehaviour
{

    [SerializeField] private Animator animator;

    public TMP_Text elementText;

    public bool finished = false;

    // Update is called once per frame
    void Update()
    {
        if (finished) {
            finished = false;

            StartCoroutine(SlideOut());
        }
    }

    private IEnumerator SlideOut() {
        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }
}
