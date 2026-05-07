using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    const string BestTimeKey = "BestTime";

    public GameTimer gameTimer;
    public float lastPlayTime;
    public float bestTime;
    public bool isNewBestTime;

    public int killCount;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            bestTime = PlayerPrefs.GetFloat(BestTimeKey, 0f);
            isNewBestTime = false;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // The current GameTimer is assigned by GameTimer itself when the game scene loads.
    }

    public void AddKill()
    {
        killCount++;
    }

    public void GameOver()
    {
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
            lastPlayTime = gameTimer.GetTime();

            if (lastPlayTime > bestTime)
            {
                bestTime = lastPlayTime;
                PlayerPrefs.SetFloat(BestTimeKey, bestTime);
                PlayerPrefs.Save();
                isNewBestTime = true;
            }
            else
            {
                isNewBestTime = false;
            }
        }
    }

    public void ResetGame()
    {
        killCount = 0;
        lastPlayTime = 0f;
        gameTimer = null;
        isNewBestTime = false;
    }

    public float GetPlayTime()
    {
        if (gameTimer != null)
            return gameTimer.GetTime();

        return lastPlayTime;
    }

    public float GetBestTime()
    {
        return bestTime;
    }
}