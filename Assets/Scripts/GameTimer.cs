using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    private const string ClearSoundResourcePath = "Sound/ClearSound";

    public TextMeshProUGUI timerText;

    [Header("Audio")]
    public AudioClip clearSound;
    [Range(0f, 1f)]
    public float clearSoundVolume = 1f;

    float currentTime;
    bool isRunning;

    void Awake()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameTimer = this;
        }
    }

    void Start()
    {
        if (clearSound == null)
        {
            clearSound = Resources.Load<AudioClip>(ClearSoundResourcePath);
        }

        if (timerText == null)
        {
            timerText = GameObject.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        }

        if (timerText == null)
        {
            Debug.LogWarning("GameTimer: TimeText 객체가 없거나 연결되지 않았습니다. GameScene에 TimeText를 추가하고 timerText에 할당하세요.");
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.gameTimer = this;
            StartTimer();
        }
    }

    public void StartTimer()
    {
        currentTime = 0f;
        isRunning = true;

        if (timerText != null)
            timerText.text = FormatTime(currentTime);
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = 0f;

        if (timerText != null)
            timerText.text = FormatTime(currentTime);
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;

            if (timerText != null)
                timerText.text = FormatTime(currentTime);

            if (currentTime >= 120f)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.GameOver();
                }
                StopTimer();
                PlayClearSound();
                SceneManager.LoadScene("GameClearScene");
            }
        }
    }

    public float GetTime()
    {
        return currentTime;
    }

    string FormatTime(float time)
    {
        int totalSeconds = Mathf.FloorToInt(time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    void PlayClearSound()
    {
        if (clearSound == null)
        {
            Debug.LogWarning($"GameTimer: Resources/{ClearSoundResourcePath} clear sound was not found.");
            return;
        }

        GameObject soundObject = new GameObject("ClearSound");
        DontDestroyOnLoad(soundObject);

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clearSound;
        audioSource.volume = clearSoundVolume;
        audioSource.spatialBlend = 0f;
        audioSource.Play();

        Destroy(soundObject, clearSound.length);
    }
}
