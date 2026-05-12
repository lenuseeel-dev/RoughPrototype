using UnityEngine;
using TMPro;

public class GameClearUI : MonoBehaviour
{
    public TextMeshProUGUI killText;
    public TextMeshProUGUI bestKillText;

    void Start()
    {
        if (GameManager.instance == null) return;

        if (killText == null)
        {
            killText = GameObject.Find("KillText")?.GetComponent<TextMeshProUGUI>();
        }

        if (bestKillText == null)
        {
            bestKillText = GameObject.Find("BestKillText")?.GetComponent<TextMeshProUGUI>();
        }

        int kills = GameManager.instance.killCount;
        int bestKillCount = GameManager.instance.GetBestKillCount();

        if (killText != null)
        {
            killText.text = "Enemies Killed: " + kills;

            if (GameManager.instance.isNewBestTime)
                killText.text += "  NEW RECORD!";
        }

        if (bestKillText != null)
        {
            bestKillText.text = "Best Kill Count: " + bestKillCount;
        }
    }
}
