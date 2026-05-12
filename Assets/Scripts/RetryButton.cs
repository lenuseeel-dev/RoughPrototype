using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    public void RestartGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGame();
        }

        SceneManager.LoadScene("IntroScene");
    }
}