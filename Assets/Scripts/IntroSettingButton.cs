using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class IntroSettingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const string IntroSceneName = "IntroScene";
    private const string SettingSceneName = "SettingScene";
    private const string SettingButtonName = "SettingButton";
    private const string SettingButtonAssetPath = "Assets/Sprites/SettingButton.png";

    private RectTransform rectTransform;
    private Vector2 originalSizeDelta;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            MovePivotToCenterKeepingPosition();
            originalSizeDelta = rectTransform.sizeDelta;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        CreateButtonIfNeeded(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateButtonIfNeeded(scene);
    }

    private static void CreateButtonIfNeeded(Scene scene)
    {
        if (scene.name != IntroSceneName)
        {
            return;
        }

        GameObject existingButton = GameObject.Find(SettingButtonName);
        if (existingButton != null)
        {
            Button existingUiButton = existingButton.GetComponent<Button>();
            IntroSettingButton existingIntroButton = existingButton.GetComponent<IntroSettingButton>();

            if (existingIntroButton == null)
            {
                existingIntroButton = existingButton.AddComponent<IntroSettingButton>();
            }

            if (existingUiButton != null)
            {
                existingUiButton.onClick.RemoveListener(existingIntroButton.GoToSetting);
                existingUiButton.onClick.AddListener(existingIntroButton.GoToSetting);
            }

            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        GameObject buttonObject = new GameObject(SettingButtonName);
        buttonObject.transform.SetParent(canvas.transform, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = Color.white;

#if UNITY_EDITOR
        Sprite settingSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SettingButtonAssetPath);
        if (settingSprite != null)
        {
            image.sprite = settingSprite;
            image.preserveAspect = true;
        }
#endif

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(180f, 90f);
        rectTransform.anchoredPosition = new Vector2(-130f, 81f);

        IntroSettingButton introSettingButton = buttonObject.AddComponent<IntroSettingButton>();
        button.onClick.AddListener(introSettingButton.GoToSetting);
    }

    public void GoToSetting()
    {
        SceneManager.LoadScene(SettingSceneName);
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
        {
            return;
        }

        if (originalSizeDelta == Vector2.zero)
        {
            originalSizeDelta = rectTransform.sizeDelta;
        }

        rectTransform.sizeDelta = originalSizeDelta * scale;
    }

    private void MovePivotToCenterKeepingPosition()
    {
        Vector2 targetPivot = new Vector2(0.5f, 0.5f);
        if (rectTransform.pivot == targetPivot)
        {
            return;
        }

        Vector2 pivotDelta = targetPivot - rectTransform.pivot;
        rectTransform.anchoredPosition += new Vector2(
            pivotDelta.x * rectTransform.sizeDelta.x,
            pivotDelta.y * rectTransform.sizeDelta.y);
        rectTransform.pivot = targetPivot;
    }
}
