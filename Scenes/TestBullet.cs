using UnityEngine;
using UnityEngine.UI;

public class TestBullet : MonoBehaviour
{
    public RectTransform whiteImage;
    public RectTransform redImage;
    public RectTransform yellowImage;
    public Button redButton;
    public Outline outline;
    public Button exitButton;
    public bool outlineEnabled;

    private void Start()
    {
        outline.enabled = false;
        redButton.onClick.AddListener(() =>
        {
            outline.enabled = !outline.enabled;
            Debug.Log(outline.enabled);
            Debug.Log("red button click");
        });

        exitButton.onClick.AddListener(() => { Debug.Log("exit button click"); });
    }
}