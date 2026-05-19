using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneKillScoreHUD : MonoBehaviour
{
    private const string GameSceneName = "GameScene";
    private const string HudObjectName = "KillScoreText";

    private TextMeshProUGUI scoreText;
    private int lastKillCount = -1;
    private int lastBestKillCount = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureHudForScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureHudForScene(scene);
    }

    private static void EnsureHudForScene(Scene scene)
    {
        if (scene.name != GameSceneName)
            return;

        if (GameObject.Find(HudObjectName) != null)
            return;

        Canvas canvas = FindCanvas();
        if (canvas == null)
        {
            canvas = CreateCanvas();
        }

        GameObject hudObject = new GameObject(HudObjectName, typeof(RectTransform));
        hudObject.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = hudObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(20f, -20f);
        rectTransform.sizeDelta = new Vector2(360f, 80f);

        TextMeshProUGUI text = hudObject.AddComponent<TextMeshProUGUI>();
        text.raycastTarget = false;
        text.fontSize = 26f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.color = Color.white;
        text.textWrappingMode = TextWrappingModes.NoWrap;

        Shadow shadow = hudObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.75f);
        shadow.effectDistance = new Vector2(2f, -2f);

        GameSceneKillScoreHUD hud = hudObject.AddComponent<GameSceneKillScoreHUD>();
        hud.scoreText = text;
        hud.UpdateScoreText(force: true);
    }

    private static Canvas FindCanvas()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject != null && canvasObject.TryGetComponent(out Canvas canvas))
            return canvas;

        return Object.FindFirstObjectByType<Canvas>();
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

    private void Awake()
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        UpdateScoreText(force: false);
    }

    private void UpdateScoreText(bool force)
    {
        if (scoreText == null)
            return;

        int killCount = 0;
        int bestKillCount = 0;

        if (GameManager.instance != null)
        {
            killCount = GameManager.instance.killCount;
            bestKillCount = GameManager.instance.GetBestKillCount();
        }

        if (!force && killCount == lastKillCount && bestKillCount == lastBestKillCount)
            return;

        lastKillCount = killCount;
        lastBestKillCount = bestKillCount;
        scoreText.text = $"Orcs Killed: {killCount}\nBest Score: {bestKillCount}";
    }
}
