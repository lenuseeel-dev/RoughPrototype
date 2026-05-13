using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneSound : MonoBehaviour
{
    private const string TargetSceneName = "IntroScene";
    private const string SoundResourcePath = "Sound/IntroSound";
    private const string SoundObjectName = "IntroSceneSound";

    private static IntroSceneSound instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        HandleScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleScene(scene);
    }

    private static void HandleScene(Scene scene)
    {
        if (scene.name == TargetSceneName)
        {
            PlaySound();
            return;
        }

        StopSound();
    }

    private static void PlaySound()
    {
        if (instance != null)
        {
            return;
        }

        AudioClip soundClip = Resources.Load<AudioClip>(SoundResourcePath);
        if (soundClip == null)
        {
            Debug.LogWarning($"IntroSceneSound: Resources/{SoundResourcePath} intro sound was not found.");
            return;
        }

        GameObject soundObject = new GameObject(SoundObjectName);
        instance = soundObject.AddComponent<IntroSceneSound>();

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.45f;
        audioSource.spatialBlend = 0f;
        audioSource.Play();
    }

    private static void StopSound()
    {
        if (instance == null)
        {
            return;
        }

        Destroy(instance.gameObject);
        instance = null;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
