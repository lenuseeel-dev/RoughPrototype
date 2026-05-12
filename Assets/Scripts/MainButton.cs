using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButton : MonoBehaviour
{
    public void GoToIntro()
    {
        SceneManager.LoadScene("IntroScene");
    }
}