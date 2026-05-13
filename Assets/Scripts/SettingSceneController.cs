using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingSceneController : MonoBehaviour
{
    private const string SettingSceneName = "SettingScene";
    private const string SettingBackgroundAssetPath = "Assets/Sprites/SettingBackground.png";

    [Header("Volume")]
    public Slider masterVolumeSlider;
    public TMP_Text masterVolumeValueText;

    [Header("Orc Count")]
    public TMP_InputField firstWaveOrcInput;
    public TMP_InputField waveOrcIncreaseInput;

    [Header("Scene")]
    public string introSceneName = "IntroScene";

    private bool isRefreshing;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        CreateControllerIfNeeded(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateControllerIfNeeded(scene);
    }

    private static void CreateControllerIfNeeded(Scene scene)
    {
        if (scene.name != SettingSceneName || FindObjectOfType<SettingSceneController>() != null)
        {
            return;
        }

        new GameObject("SettingSceneController").AddComponent<SettingSceneController>();
    }

    private void Start()
    {
        BuildDefaultUIIfNeeded();
        GameSettings.ApplyMasterVolume();
        RefreshUI();
    }

    public void OnMasterVolumeChanged(float value)
    {
        if (isRefreshing)
        {
            return;
        }

        GameSettings.MasterVolume = value;
        UpdateVolumeText(value);
    }

    public void OnFirstWaveOrcChanged(string value)
    {
        if (isRefreshing)
        {
            return;
        }

        GameSettings.FirstWaveOrcCount = ParseInt(value, GameSettings.DefaultFirstWaveOrcCount, 1);
        RefreshInputValues();
    }

    public void OnWaveOrcIncreaseChanged(string value)
    {
        if (isRefreshing)
        {
            return;
        }

        GameSettings.WaveOrcIncrease = ParseInt(value, GameSettings.DefaultWaveOrcIncrease, 0);
        RefreshInputValues();
    }

    public void SaveAndBack()
    {
        SaveCurrentValues();
        SceneManager.LoadScene(introSceneName);
    }

    public void ResetSettings()
    {
        GameSettings.ResetToDefaults();
        RefreshUI();
    }

    private void SaveCurrentValues()
    {
        if (masterVolumeSlider != null)
        {
            GameSettings.MasterVolume = masterVolumeSlider.value;
        }

        if (firstWaveOrcInput != null)
        {
            GameSettings.FirstWaveOrcCount = ParseInt(firstWaveOrcInput.text, GameSettings.DefaultFirstWaveOrcCount, 1);
        }

        if (waveOrcIncreaseInput != null)
        {
            GameSettings.WaveOrcIncrease = ParseInt(waveOrcIncreaseInput.text, GameSettings.DefaultWaveOrcIncrease, 0);
        }

        GameSettings.Save();
    }

    private void RefreshUI()
    {
        isRefreshing = true;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.value = GameSettings.MasterVolume;
        }

        RefreshInputValues();
        UpdateVolumeText(GameSettings.MasterVolume);

        isRefreshing = false;
    }

    private void RefreshInputValues()
    {
        if (firstWaveOrcInput != null)
        {
            firstWaveOrcInput.text = GameSettings.FirstWaveOrcCount.ToString();
        }

        if (waveOrcIncreaseInput != null)
        {
            waveOrcIncreaseInput.text = GameSettings.WaveOrcIncrease.ToString();
        }
    }

    private void UpdateVolumeText(float value)
    {
        if (masterVolumeValueText != null)
        {
            masterVolumeValueText.text = $"{Mathf.RoundToInt(value * 100f)}%";
        }
    }

    private int ParseInt(string value, int fallback, int minValue)
    {
        if (!int.TryParse(value, out int result))
        {
            result = fallback;
        }

        return Mathf.Max(minValue, result);
    }

    private void BuildDefaultUIIfNeeded()
    {
        if (masterVolumeSlider != null || firstWaveOrcInput != null || waveOrcIncreaseInput != null)
        {
            return;
        }

        EnsureEventSystem();

        Canvas canvas = CreateCanvas();
        CreateBackground(canvas.transform);
        RectTransform root = CreatePanel(canvas.transform, "SettingPanel", new Vector2(520f, 430f));

        CreateText(root, "SETTING", 34, new Vector2(0f, 160f), new Vector2(460f, 54f), TextAlignmentOptions.Center);
        CreateText(root, "Master Volume", 22, new Vector2(-120f, 82f), new Vector2(210f, 36f), TextAlignmentOptions.Left);
        masterVolumeValueText = CreateText(root, "100%", 20, new Vector2(178f, 82f), new Vector2(90f, 36f), TextAlignmentOptions.Right);
        masterVolumeSlider = CreateSlider(root, new Vector2(70f, 42f), new Vector2(330f, 24f));

        CreateText(root, "First Orcs", 22, new Vector2(-130f, -24f), new Vector2(230f, 38f), TextAlignmentOptions.Left);
        firstWaveOrcInput = CreateInput(root, new Vector2(155f, -24f), new Vector2(110f, 42f));

        CreateText(root, "Orcs Per Wave", 22, new Vector2(-130f, -84f), new Vector2(230f, 38f), TextAlignmentOptions.Left);
        waveOrcIncreaseInput = CreateInput(root, new Vector2(155f, -84f), new Vector2(110f, 42f));

        Button resetButton = CreateButton(root, "Reset", new Vector2(-92f, -158f), new Vector2(150f, 48f));
        Button backButton = CreateButton(root, "Back", new Vector2(92f, -158f), new Vector2(150f, 48f));

        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        firstWaveOrcInput.onEndEdit.AddListener(OnFirstWaveOrcChanged);
        waveOrcIncreaseInput.onEndEdit.AddListener(OnWaveOrcIncreaseChanged);
        resetButton.onClick.AddListener(ResetSettings);
        backButton.onClick.AddListener(SaveAndBack);
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("SettingCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private void CreateBackground(Transform parent)
    {
#if UNITY_EDITOR
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SettingBackgroundAssetPath);
        if (backgroundSprite == null)
        {
            return;
        }

        GameObject backgroundObject = new GameObject("SettingBackground");
        backgroundObject.transform.SetParent(parent, false);

        Image image = backgroundObject.AddComponent<Image>();
        image.sprite = backgroundSprite;
        image.preserveAspect = true;
        image.color = Color.white;

        RectTransform rectTransform = backgroundObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.SetAsFirstSibling();
#endif
    }

    private RectTransform CreatePanel(Transform parent, string objectName, Vector2 size)
    {
        GameObject panelObject = new GameObject(objectName);
        panelObject.transform.SetParent(parent, false);

        Image image = panelObject.AddComponent<Image>();
        image.color = new Color(0.07f, 0.08f, 0.09f, 0.88f);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = Vector2.zero;
        return rectTransform;
    }

    private TMP_Text CreateText(Transform parent, string text, int size, Vector2 position, Vector2 rectSize, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(text);
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = size;
        textComponent.color = Color.white;
        textComponent.alignment = alignment;

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = rectSize;
        rectTransform.anchoredPosition = position;
        return textComponent;
    }

    private Slider CreateSlider(Transform parent, Vector2 position, Vector2 size)
    {
        GameObject sliderObject = new GameObject("MasterVolumeSlider");
        sliderObject.transform.SetParent(parent, false);

        RectTransform rectTransform = sliderObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;

        RectTransform fillArea = CreateSliderPart(sliderObject.transform, "Fill Area", new Color(1f, 1f, 1f, 0f), size);
        RectTransform fill = CreateSliderPart(fillArea, "Fill", new Color(0.2f, 0.72f, 0.48f, 1f), size);
        RectTransform handleArea = CreateSliderPart(sliderObject.transform, "Handle Slide Area", new Color(1f, 1f, 1f, 0f), size);
        RectTransform handle = CreateSliderPart(handleArea, "Handle", Color.white, new Vector2(28f, 28f));

        slider.fillRect = fill;
        slider.handleRect = handle;
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.direction = Slider.Direction.LeftToRight;

        return slider;
    }

    private RectTransform CreateSliderPart(Transform parent, string objectName, Color color, Vector2 size)
    {
        GameObject partObject = new GameObject(objectName);
        partObject.transform.SetParent(parent, false);

        Image image = partObject.AddComponent<Image>();
        image.color = color;

        RectTransform rectTransform = partObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0.5f);
        rectTransform.anchorMax = new Vector2(1f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = Vector2.zero;
        return rectTransform;
    }

    private TMP_InputField CreateInput(Transform parent, Vector2 position, Vector2 size)
    {
        GameObject inputObject = new GameObject("NumberInput");
        inputObject.transform.SetParent(parent, false);

        Image image = inputObject.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.92f);

        TMP_InputField input = inputObject.AddComponent<TMP_InputField>();
        input.contentType = TMP_InputField.ContentType.IntegerNumber;

        RectTransform rectTransform = inputObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        TMP_Text text = CreateText(inputObject.transform, "Text", 22, Vector2.zero, new Vector2(size.x - 18f, size.y - 10f), TextAlignmentOptions.Center);
        text.color = Color.black;
        input.textComponent = text;

        return input;
    }

    private Button CreateButton(Transform parent, string label, Vector2 position, Vector2 size)
    {
        GameObject buttonObject = new GameObject(label + "Button");
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.2f, 0.72f, 0.48f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        CreateText(buttonObject.transform, label, 22, Vector2.zero, size, TextAlignmentOptions.Center);
        return button;
    }
}
