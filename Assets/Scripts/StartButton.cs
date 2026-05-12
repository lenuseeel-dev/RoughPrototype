using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void GameStart()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGame();
        }

        SceneManager.LoadScene("GameScene");
    }
}