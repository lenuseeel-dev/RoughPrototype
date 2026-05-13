using UnityEngine;
using UnityEngine.UI;

public class FadeInUI : MonoBehaviour
{
    public float fadeSpeed = 1.5f;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        Color color = image.color;

        color.a -= fadeSpeed * Time.deltaTime;

        image.color = color;

        if (color.a <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}