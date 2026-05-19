using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    const string BestKillCountKey = "BestKillCount";

    public GameTimer gameTimer;
    public float lastPlayTime;
    public int bestKillCount;
    public bool isNewBestTime;

    public int killCount;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            bestKillCount = PlayerPrefs.GetInt(BestKillCountKey, 0);
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

        if (killCount > bestKillCount)
        {
            bestKillCount = killCount;
            PlayerPrefs.SetInt(BestKillCountKey, bestKillCount);
            PlayerPrefs.Save();
            isNewBestTime = true;
        }
    }

    public void GameOver()
    {
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
            lastPlayTime = gameTimer.GetTime();
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

    public int GetBestKillCount()
    {
        return bestKillCount;
    }
}
