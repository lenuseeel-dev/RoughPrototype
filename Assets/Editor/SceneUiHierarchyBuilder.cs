using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class SceneUiHierarchyBuilder
{
    private const string IntroScenePath = "Assets/Scenes/IntroScene.unity";
    private const string SettingScenePath = "Assets/Scenes/SettingScene.unity";
    private const string SettingButtonPath = "Assets/Sprites/SettingButton.png";
    private const string SettingBackgroundPath = "Assets/Sprites/SettingBackground.png";

    static SceneUiHierarchyBuilder()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorApplication.delayCall += BuildActiveSceneUiIfNeeded;
    }

    [MenuItem("Tools/UI/Build Intro and Setting UI")]
    public static void BuildIntroAndSettingUi()
    {
        string currentScenePath = SceneManager.GetActiveScene().path;

        BuildScene(IntroScenePath, true);
        BuildScene(SettingScenePath, true);

        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/UI/Build Current Scene UI")]
    public static void BuildCurrentSceneUi()
    {
        BuildSceneUi(SceneManager.GetActiveScene(), true);
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        BuildSceneUi(scene, false);
    }

    private static void BuildActiveSceneUiIfNeeded()
    {
        BuildSceneUi(SceneManager.GetActiveScene(), false);
    }

    private static void BuildScene(string scenePath, bool force)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        BuildSceneUi(scene, force);
    }

    private static void BuildSceneUi(Scene scene, bool force)
    {
        if (!scene.IsValid())
        {
            return;
        }

        if (scene.path == IntroScenePath)
        {
            BuildIntroScene(force);
            return;
        }

        if (scene.path == SettingScenePath)
        {
            BuildSettingScene(force);
        }
    }

    private static void BuildIntroScene(bool force)
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            canvas = CreateCanvas("Canvas");
        }

        GameObject settingButton = GameObject.Find("SettingButton");
        if (settingButton != null && !force)
        {
            ConfigureIntroSettingButton(settingButton);
            return;
        }

        if (settingButton == null)
        {
            settingButton = new GameObject("SettingButton", typeof(RectTransform));
            settingButton.transform.SetParent(canvas.transform, false);
        }

        Image image = GetOrAdd<Image>(settingButton);
        image.sprite = LoadSprite(SettingButtonPath);
        image.color = Color.white;
        image.preserveAspect = true;

        Button button = GetOrAdd<Button>(settingButton);
        button.targetGraphic = image;

        RectTransform rectTransform = settingButton.transform as RectTransform;
        if (rectTransform == null)
        {
            Debug.LogWarning("SettingButton must be under a UI Canvas to use RectTransform.");
            return;
        }
        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(210f, 72f);
        rectTransform.anchoredPosition = new Vector2(-141f, 72f);

        ConfigureIntroSettingButton(settingButton);
        EnsureEventSystem();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private static void ConfigureIntroSettingButton(GameObject settingButton)
    {
        IntroSettingButton introSettingButton = GetOrAdd<IntroSettingButton>(settingButton);
        Button button = GetOrAdd<Button>(settingButton);

        if (button.targetGraphic == null)
        {
            button.targetGraphic = settingButton.GetComponent<Graphic>();
        }

        if (button.onClick.GetPersistentEventCount() == 0)
        {
            UnityEventTools.AddPersistentListener(button.onClick, introSettingButton.GoToSetting);
            EditorUtility.SetDirty(button);
        }
    }

    private static void BuildSettingScene(bool force)
    {
        GameObject existingCanvas = GameObject.Find("SettingCanvas");
        bool alreadyHasTimeBetweenWaves = GameObject.Find("TimeBetweenWavesInput") != null;
        bool alreadyHasClearSurvivalTime = GameObject.Find("ClearSurvivalTimeInput") != null;

        if (existingCanvas != null && !force && alreadyHasTimeBetweenWaves && alreadyHasClearSurvivalTime)
        {
            return;
        }

        if (existingCanvas != null)
        {
            Object.DestroyImmediate(existingCanvas);
        }

        GameObject controllerObject = GameObject.Find("SettingSceneController") ?? GameObject.Find("SettingController");
        if (controllerObject == null)
        {
            controllerObject = new GameObject("SettingSceneController");
        }
        else
        {
            controllerObject.name = "SettingSceneController";
        }

        SettingSceneController controller = GetOrAdd<SettingSceneController>(controllerObject);

        Canvas canvas = CreateCanvas("SettingCanvas");
        CreateBackground(canvas.transform);
        RectTransform panel = CreatePanel(canvas.transform);

        CreateText(panel, "SETTING", 42, new Vector2(0f, 230f), new Vector2(520f, 60f), TextAlignmentOptions.Center);
        CreateText(panel, "Master Volume", 24, new Vector2(-150f, 145f), new Vector2(250f, 40f), TextAlignmentOptions.Left);
        TMP_Text volumeText = CreateText(panel, "100%", 22, new Vector2(190f, 145f), new Vector2(100f, 40f), TextAlignmentOptions.Right);
        Slider volumeSlider = CreateSlider(panel, new Vector2(60f, 103f), new Vector2(360f, 28f));

        CreateText(panel, "First Orcs", 24, new Vector2(-150f, 20f), new Vector2(250f, 42f), TextAlignmentOptions.Left);
        TMP_InputField firstOrcInput = CreateInput(panel, "FirstWaveOrcInput", new Vector2(170f, 20f), new Vector2(130f, 46f), TMP_InputField.ContentType.IntegerNumber);

        CreateText(panel, "Orcs Per Wave", 24, new Vector2(-150f, -42f), new Vector2(250f, 42f), TextAlignmentOptions.Left);
        TMP_InputField waveOrcInput = CreateInput(panel, "WaveOrcIncreaseInput", new Vector2(170f, -42f), new Vector2(130f, 46f), TMP_InputField.ContentType.IntegerNumber);

        CreateText(panel, "Wave Delay", 24, new Vector2(-150f, -104f), new Vector2(250f, 42f), TextAlignmentOptions.Left);
        TMP_InputField timeBetweenWavesInput = CreateInput(panel, "TimeBetweenWavesInput", new Vector2(170f, -104f), new Vector2(130f, 46f), TMP_InputField.ContentType.DecimalNumber);

        CreateText(panel, "Clear Time (s)", 24, new Vector2(-150f, -166f), new Vector2(250f, 42f), TextAlignmentOptions.Left);
        TMP_InputField clearSurvivalTimeInput = CreateInput(panel, "ClearSurvivalTimeInput", new Vector2(170f, -166f), new Vector2(130f, 46f), TMP_InputField.ContentType.DecimalNumber);

        Button resetButton = CreateButton(panel, "Reset", new Vector2(-100f, -238f), new Vector2(160f, 52f));
        Button backButton = CreateButton(panel, "Back", new Vector2(100f, -238f), new Vector2(160f, 52f));

        controller.masterVolumeSlider = volumeSlider;
        controller.masterVolumeValueText = volumeText;
        controller.firstWaveOrcInput = firstOrcInput;
        controller.waveOrcIncreaseInput = waveOrcInput;
        controller.timeBetweenWavesInput = timeBetweenWavesInput;
        controller.clearSurvivalTimeInput = clearSurvivalTimeInput;

        UnityEventTools.AddPersistentListener(volumeSlider.onValueChanged, controller.OnMasterVolumeChanged);
        UnityEventTools.AddPersistentListener(firstOrcInput.onEndEdit, controller.OnFirstWaveOrcChanged);
        UnityEventTools.AddPersistentListener(waveOrcInput.onEndEdit, controller.OnWaveOrcIncreaseChanged);
        UnityEventTools.AddPersistentListener(timeBetweenWavesInput.onEndEdit, controller.OnTimeBetweenWavesChanged);
        UnityEventTools.AddPersistentListener(clearSurvivalTimeInput.onEndEdit, controller.OnClearSurvivalTimeChanged);
        UnityEventTools.AddPersistentListener(resetButton.onClick, controller.ResetSettings);
        UnityEventTools.AddPersistentListener(backButton.onClick, controller.SaveAndBack);

        EnsureEventSystem();

        EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private static Canvas CreateCanvas(string objectName)
    {
        GameObject canvasObject = new GameObject(objectName, typeof(RectTransform));
        canvasObject.layer = LayerMask.NameToLayer("UI");

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static void CreateBackground(Transform parent)
    {
        GameObject backgroundObject = CreateUiObject("SettingBackground", parent);
        Image image = backgroundObject.AddComponent<Image>();
        image.sprite = LoadSprite(SettingBackgroundPath);
        image.color = Color.white;
        image.preserveAspect = true;

        RectTransform rectTransform = backgroundObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private static RectTransform CreatePanel(Transform parent)
    {
        GameObject panelObject = CreateUiObject("SettingPanel", parent);
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

    private static TMP_Text CreateText(Transform parent, string text, int size, Vector2 position, Vector2 rectSize, TextAlignmentOptions alignment)
    {
        GameObject textObject = CreateUiObject(text, parent);
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

    private static Slider CreateSlider(Transform parent, Vector2 position, Vector2 size)
    {
        GameObject sliderObject = CreateUiObject("MasterVolumeSlider", parent);
        RectTransform rectTransform = sliderObject.GetComponent<RectTransform>();
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

    private static RectTransform CreateSliderContainer(Transform parent, string objectName)
    {
        GameObject containerObject = CreateUiObject(objectName, parent);
        RectTransform rectTransform = containerObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        return rectTransform;
    }

    private static RectTransform CreateSliderImage(Transform parent, string objectName, Color color, Vector2 size, bool stretch)
    {
        GameObject imageObject = CreateUiObject(objectName, parent);
        Image image = imageObject.AddComponent<Image>();
        image.color = color;

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = stretch ? new Vector2(0f, 0.5f) : new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = stretch ? new Vector2(1f, 0.5f) : new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = stretch ? new Vector2(0f, size.y) : size;
        rectTransform.anchoredPosition = Vector2.zero;
        return rectTransform;
    }

    private static TMP_InputField CreateInput(Transform parent, string objectName, Vector2 position, Vector2 size, TMP_InputField.ContentType contentType)
    {
        GameObject inputObject = CreateUiObject(objectName, parent);
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

    private static Button CreateButton(Transform parent, string label, Vector2 position, Vector2 size)
    {
        GameObject buttonObject = CreateUiObject(label + "Button", parent);
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

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject gameObject = new GameObject(objectName, typeof(RectTransform));
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
    }

    private static T GetOrAdd<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }
}
