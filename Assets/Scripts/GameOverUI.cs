using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killText;

    void Start()
    {
        if (timeText == null)
        {
            timeText = GameObject.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        }

        if (killText == null)
        {
            killText = GameObject.Find("KillText")?.GetComponent<TextMeshProUGUI>();
        }

        if (GameManager.instance == null) return;

        float time = GameManager.instance.GetPlayTime();
        int bestKillCount = GameManager.instance.GetBestKillCount();
        int kills = GameManager.instance.killCount;

        if (timeText != null)
        {
            timeText.text = "Time Survived: " + FormatTime(time) + "\n" +
                            "Best Kill Count: " + bestKillCount;

            if (GameManager.instance.isNewBestTime)
                timeText.text += "  NEW RECORD!";
        }

        if (killText != null)
            killText.text = "Enemies Killed: " + kills;
    }

    string FormatTime(float time)
    {
        int totalSeconds = Mathf.FloorToInt(time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }
}