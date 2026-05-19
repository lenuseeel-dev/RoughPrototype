using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingSceneController : MonoBehaviour
{
    [Header("Volume")]
    public Slider masterVolumeSlider;
    public TMP_Text masterVolumeValueText;

    [Header("Orc Count")]
    public TMP_InputField firstWaveOrcInput;
    public TMP_InputField waveOrcIncreaseInput;

    [Header("Wave")]
    public TMP_InputField timeBetweenWavesInput;

    [Header("Clear")]
    public TMP_InputField clearSurvivalTimeInput;

    [Header("Scene")]
    public string introSceneName = "IntroScene";

    private bool isRefreshing;

    private void Start()
    {
        BuildCanvasUiIfNeeded();
        ValidateUiReferences();
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

    public void OnTimeBetweenWavesChanged(string value)
    {
        if (isRefreshing)
        {
            return;
        }

        GameSettings.TimeBetweenWaves = ParseFloat(value, GameSettings.DefaultTimeBetweenWaves, 0.1f);
        RefreshInputValues();
    }

    public void OnClearSurvivalTimeChanged(string value)
    {
        if (isRefreshing)
        {
            return;
        }

        GameSettings.ClearSurvivalTime = ParseFloat(value, GameSettings.DefaultClearSurvivalTime, 1f);
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

        if (timeBetweenWavesInput != null)
        {
            GameSettings.TimeBetweenWaves = ParseFloat(timeBetweenWavesInput.text, GameSettings.DefaultTimeBetweenWaves, 0.1f);
        }

        if (clearSurvivalTimeInput != null)
        {
            GameSettings.ClearSurvivalTime = ParseFloat(clearSurvivalTimeInput.text, GameSettings.DefaultClearSurvivalTime, 1f);
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

        if (timeBetweenWavesInput != null)
        {
            timeBetweenWavesInput.text = GameSettings.TimeBetweenWaves.ToString("0.##");
        }

        if (clearSurvivalTimeInput != null)
        {
            clearSurvivalTimeInput.text = GameSettings.ClearSurvivalTime.ToString("0.##");
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

    private float ParseFloat(string value, float fallback, float minValue)
    {
        if (!float.TryParse(value, out float result))
        {
            result = fallback;
        }

        return Mathf.Max(minValue, result);
    }

    private void ValidateUiReferences()
    {
        if (masterVolumeSlider == null || masterVolumeValueText == null || firstWaveOrcInput == null || waveOrcIncreaseInput == null || timeBetweenWavesInput == null || clearSurvivalTimeInput == null)
        {
            Debug.LogWarning("SettingSceneController: Setting UI references are missing.");
        }
    }

    private void BuildCanvasUiIfNeeded()
    {
        if (masterVolumeSlider != null && masterVolumeValueText != null && firstWaveOrcInput != null && waveOrcIncreaseInput != null && timeBetweenWavesInput != null && clearSurvivalTimeInput != null)
        {
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("SettingCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        CreateBackground(canvas.transform);
        RectTransform panel = CreatePanel(canvas.transform);

        CreateText(panel, "SETTING", 42, new Vector2(0f, 230f), new Vector2(520f, 60f), TextAlignmentOptions.Center);
        CreateText(panel, "Master Volume", 24, new Vector2(-150f, 145f), new Vector2(250f, 40f), TextAlignmentOptions.Left);
        masterVolumeValueText = CreateText(panel, "100%", 22, new Vector2(190f, 145f), new Vector2(100f, 40f), TextAlignmentOptions.Right);
        masterVolumeSlider = CreateSlider(panel, new Vector2(60f, 103f), new Vector2(360f, 28f));

        CreateText(panel, "First Orcs", 24, new Vector2(-150f, 20f), new Vector2(250f, 42f), TextAlignmentOptions.Left);
        firstWaveOrcInput = CreateInput(panel, new Vector2(170f, 20f), new Vector2(130f, 46f), TMP_InputField.ContentType.IntegerNumber);

        CreateText(panel, "Orcs Per Wave", 24, new Vector2(-150f, -42f), new Vector2(250f, 42f), TextAlignmentOptions.Left);
        waveOrcIncreaseInput = CreateInput(panel, new Vector2(170f, -42f), new Vector2(130f, 46f), TMP_InputField.ContentType.IntegerNumber);

        CreateText(panel, "Wave Delay", 24, new Vector2(-150f, -104f), new Vector2(250f, 42f), TextAlignmentOptions.Left);
        timeBetweenWavesInput = CreateInput(panel, new Vector2(170f, -104f), new Vector2(130f, 46f), TMP_InputField.ContentType.DecimalNumber);

        CreateText(panel, "Clear Time (s)", 24, new Vector2(-150f, -166f), new Vector2(250f, 42f), TextAlignmentOptions.Left);
        clearSurvivalTimeInput = CreateInput(panel, new Vector2(170f, -166f), new Vector2(130f, 46f), TMP_InputField.ContentType.DecimalNumber);

        Button resetButton = CreateButton(panel, "Reset", new Vector2(-100f, -238f), new Vector2(160f, 52f));
        Button backButton = CreateButton(panel, "Back", new Vector2(100f, -238f), new Vector2(160f, 52f));

        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        firstWaveOrcInput.onEndEdit.AddListener(OnFirstWaveOrcChanged);
        waveOrcIncreaseInput.onEndEdit.AddListener(OnWaveOrcIncreaseChanged);
        timeBetweenWavesInput.onEndEdit.AddListener(OnTimeBetweenWavesChanged);
        clearSurvivalTimeInput.onEndEdit.AddListener(OnClearSurvivalTimeChanged);
        resetButton.onClick.AddListener(ResetSettings);
        backButton.onClick.AddListener(SaveAndBack);
    }

    private void CreateBackground(Transform parent)
    {
        if (parent.Find("SettingBackground") != null)
        {
            return;
        }

        GameObject backgroundObject = new GameObject("SettingBackground");
        backgroundObject.transform.SetParent(parent, false);

        Image image = backgroundObject.AddComponent<Image>();
        image.color = Color.white;
        image.preserveAspect = true;

        RectTransform rectTransform = backgroundObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.SetAsFirstSibling();
    }

    private RectTransform CreatePanel(Transform parent)
    {
        Transform existing = parent.Find("SettingPanel");
        if (existing != null)
        {
            DestroyObject(existing.gameObject);
        }

        GameObject panelObject = new GameObject("SettingPanel");
        panelObject.transform.SetParent(parent, false);

        Image image = panelObject.AddComponent<Image>();
        image.color = new Color(0.05f, 0.06f, 0.07f, 0.86f);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(620f, 570f);
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
        textComponent.raycastTarget = false;

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

        CreateSliderImage(sliderObject.transform, "Background", new Color(0.16f, 0.16f, 0.16f, 1f), size, true);
        RectTransform fillArea = CreateSliderContainer(sliderObject.transform, "Fill Area");
        RectTransform fill = CreateSliderImage(fillArea, "Fill", new Color(0.25f, 0.75f, 0.48f, 1f), size, true);
        RectTransform handleArea = CreateSliderContainer(sliderObject.transform, "Handle Slide Area");
        RectTransform handle = CreateSliderImage(handleArea, "Handle", Color.white, new Vector2(28f, 28f), false);

        slider.fillRect = fill;
        slider.handleRect = handle;
        slider.targetGraphic = handle.GetComponent<Image>();
        return slider;
    }

    private RectTransform CreateSliderContainer(Transform parent, string objectName)
    {
        GameObject containerObject = new GameObject(objectName);
        containerObject.transform.SetParent(parent, false);

        RectTransform rectTransform = containerObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        return rectTransform;
    }

    private RectTransform CreateSliderImage(Transform parent, string objectName, Color color, Vector2 size, bool stretch)
    {
        GameObject imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);

        Image image = imageObject.AddComponent<Image>();
        image.color = color;

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = stretch ? new Vector2(0f, 0.5f) : new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = stretch ? new Vector2(1f, 0.5f) : new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = stretch ? new Vector2(0f, size.y) : size;
        rectTransform.anchoredPosition = Vector2.zero;
        return rectTransform;
    }

    private TMP_InputField CreateInput(Transform parent, Vector2 position, Vector2 size, TMP_InputField.ContentType contentType)
    {
        GameObject inputObject = new GameObject("NumberInput");
        inputObject.transform.SetParent(parent, false);

        Image image = inputObject.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.94f);

        TMP_InputField input = inputObject.AddComponent<TMP_InputField>();
        input.contentType = contentType;
        input.targetGraphic = image;

        RectTransform rectTransform = inputObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        TMP_Text text = CreateText(inputObject.transform, "Text", 24, Vector2.zero, new Vector2(size.x - 16f, size.y - 8f), TextAlignmentOptions.Center);
        text.color = Color.black;
        input.textComponent = text;
        return input;
    }

    private Button CreateButton(Transform parent, string label, Vector2 position, Vector2 size)
    {
        GameObject buttonObject = new GameObject(label + "Button");
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.25f, 0.75f, 0.48f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        CreateText(buttonObject.transform, label, 24, Vector2.zero, size, TextAlignmentOptions.Center);
        return button;
    }

    private void DestroyObject(GameObject target)
    {
        if (Application.isPlaying)
        {
            Destroy(target);
            return;
        }

        DestroyImmediate(target);
    }
}
