using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneBgm : MonoBehaviour
{
    private const string TargetSceneName = "GameScene";
    private const string BgmResourcePath = "Sound/BGM";
    private const string BgmObjectName = "GameSceneBGM";

    private static GameSceneBgm instance;

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
            PlayBgm();
            return;
        }

        StopBgm();
    }

    private static void PlayBgm()
    {
        if (instance != null)
        {
            return;
        }

        AudioClip bgmClip = Resources.Load<AudioClip>(BgmResourcePath);
        if (bgmClip == null)
        {
            Debug.LogWarning($"GameSceneBgm: Resources/{BgmResourcePath} BGM을 찾을 수 없습니다.");
            return;
        }

        GameObject bgmObject = new GameObject(BgmObjectName);
        instance = bgmObject.AddComponent<GameSceneBgm>();

        AudioSource audioSource = bgmObject.AddComponent<AudioSource>();
        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.45f;
        audioSource.spatialBlend = 0f;
        audioSource.Play();
    }

    private static void StopBgm()
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
