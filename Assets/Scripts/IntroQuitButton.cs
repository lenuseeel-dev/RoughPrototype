using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class IntroQuitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector2 originalSizeDelta;

    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        gameObject.SetActive(false);
        return;
#endif

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalSizeDelta = rectTransform.sizeDelta;
        }

        Button button = GetComponent<Button>();
        button.onClick.RemoveListener(QuitGame);
        button.onClick.AddListener(QuitGame);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetButtonSize(1.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetButtonSize(1f);
    }

    private void SetButtonSize(float scale)
    {
        if (rectTransform == null)
            return;

        if (originalSizeDelta == Vector2.zero)
        {
            originalSizeDelta = rectTransform.sizeDelta;
        }

        rectTransform.sizeDelta = originalSizeDelta * scale;
    }
}
