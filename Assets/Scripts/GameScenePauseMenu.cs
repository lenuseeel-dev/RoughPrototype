using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScenePauseMenu : MonoBehaviour
{
    private const string GameSceneName = "GameScene";
    private const string IntroSceneName = "IntroScene";
    private const string PauseMenuObjectName = "GameScenePauseMenu";

    private GameObject overlay;
    private float timeScaleBeforePause = 1f;
    private bool isPaused;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsurePauseMenuForScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        EnsurePauseMenuForScene(scene);
    }

    private static void EnsurePauseMenuForScene(Scene scene)
    {
        if (scene.name != GameSceneName)
            return;

        if (GameObject.Find(PauseMenuObjectName) != null)
            return;

        EnsureEventSystem();

        Canvas canvas = FindCanvas();
        if (canvas == null)
        {
            canvas = CreateCanvas();
        }

        GameObject menuObject = new GameObject(PauseMenuObjectName, typeof(RectTransform));
        menuObject.transform.SetParent(canvas.transform, false);

        RectTransform menuRect = menuObject.GetComponent<RectTransform>();
        menuRect.anchorMin = Vector2.zero;
        menuRect.anchorMax = Vector2.one;
        menuRect.offsetMin = Vector2.zero;
        menuRect.offsetMax = Vector2.zero;

        GameScenePauseMenu pauseMenu = menuObject.AddComponent<GameScenePauseMenu>();
        pauseMenu.BuildOverlay();
    }

    private static Canvas FindCanvas()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject != null && canvasObject.TryGetComponent(out Canvas canvas))
            return canvas;

        return FindFirstObjectByType<Canvas>();
    }

    private static Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Canvas");

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private void BuildOverlay()
    {
        overlay = new GameObject("PauseOverlay", typeof(RectTransform));
        overlay.transform.SetParent(transform, false);

        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image haze = overlay.AddComponent<Image>();
        haze.color = new Color(0.88f, 0.92f, 0.96f, 0.68f);
        haze.raycastTarget = true;

        GameObject panel = new GameObject("ButtonPanel", typeof(RectTransform));
        panel.transform.SetParent(overlay.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(360f, 260f);

#if UNITY_WEBGL && !UNITY_EDITOR
        CreateButton(panel.transform, "RestartButton", "RESTART", new Vector2(0f, 45f), RestartGame);
        CreateButton(panel.transform, "MainMenuButton", "MAIN MENU", new Vector2(0f, -45f), GoToMainMenu);
#else
        CreateButton(panel.transform, "RestartButton", "RESTART", new Vector2(0f, 90f), RestartGame);
        CreateButton(panel.transform, "MainMenuButton", "MAIN MENU", new Vector2(0f, 0f), GoToMainMenu);
        CreateButton(panel.transform, "QuitButton", "QUIT", new Vector2(0f, -90f), QuitGame);
#endif

        overlay.SetActive(false);
    }

    private static void CreateButton(Transform parent, string objectName, string label, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform));
        buttonObject.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(300f, 64f);

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.07f, 0.08f, 0.1f, 0.9f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(onClick);

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.07f, 0.08f, 0.1f, 0.9f);
        colors.highlightedColor = new Color(0.16f, 0.18f, 0.22f, 0.95f);
        colors.pressedColor = new Color(0.03f, 0.04f, 0.05f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        GameObject textObject = new GameObject("Text", typeof(RectTransform));
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 30f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.NoWrap;
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        if (isPaused)
            return;

        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
        isPaused = true;
        overlay.SetActive(true);
    }

    private void ResumeGame()
    {
        if (!isPaused)
            return;

        Time.timeScale = timeScaleBeforePause <= 0f ? 1f : timeScaleBeforePause;
        isPaused = false;
        overlay.SetActive(false);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGame();
        }

        SceneManager.LoadScene(GameSceneName);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGame();
        }

        SceneManager.LoadScene(IntroSceneName);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
        }
    }
}
