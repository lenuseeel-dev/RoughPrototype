using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    const float DefaultDuration = 0.6f;

    static SceneFader instance;

    CanvasGroup canvasGroup;
    Coroutine fadeCoroutine;

    public static void LoadScene(string sceneName, float duration = DefaultDuration)
    {
        GetOrCreateInstance().StartFadeLoad(sceneName, duration);
    }

    static SceneFader GetOrCreateInstance()
    {
        if (instance != null)
            return instance;

        GameObject faderObject = new GameObject("SceneFader");
        DontDestroyOnLoad(faderObject);

        return faderObject.AddComponent<SceneFader>();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasGroup == null)
            CreateFadeCanvas();
    }

    void CreateFadeCanvas()
    {
        GameObject canvasObject = new GameObject("FadeCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(canvasObject.transform, false);

        Image fadeImage = imageObject.AddComponent<Image>();
        fadeImage.color = Color.black;

        RectTransform rectTransform = fadeImage.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    void StartFadeLoad(string sceneName, float duration)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeLoadRoutine(sceneName, Mathf.Max(0.01f, duration)));
    }

    IEnumerator FadeLoadRoutine(string sceneName, float duration)
    {
        canvasGroup.blocksRaycasts = true;

        yield return Fade(1f, duration);
        SceneManager.LoadScene(sceneName);
        yield return null;
        yield return Fade(0f, duration);

        canvasGroup.blocksRaycasts = false;
        fadeCoroutine = null;
    }

    IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
