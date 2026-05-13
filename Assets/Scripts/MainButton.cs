using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class MainButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rt;
    private Vector2 originalSize;
    private bool useRectTransform;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            originalSize = rt.rect.size;
            useRectTransform = true;
        }
        else
        {
            Debug.LogWarning("MainButton 스크립트는 UI 버튼 오브젝트(또는 RectTransform이 있는 오브젝트)에 붙여야 합니다.");
            originalSize = transform.localScale;
            useRectTransform = false;
        }
    }

    public void GoToIntro()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useRectTransform)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize.x * 1.1f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalSize.y * 1.1f);
        }
        else
        {
            transform.localScale = originalSize * 1.1f;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (useRectTransform)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize.x);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalSize.y);
        }
        else
        {
            transform.localScale = originalSize;
        }
    }
}