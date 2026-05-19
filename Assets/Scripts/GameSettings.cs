using UnityEngine;

public static class GameSettings
{
    public const float DefaultMasterVolume = 1f;
    public const int DefaultFirstWaveOrcCount = 3;
    public const int DefaultWaveOrcIncrease = 1;
    public const float DefaultTimeBetweenWaves = 8f;
    public const float DefaultClearSurvivalTime = 120f;

    private const string MasterVolumeKey = "MasterVolume";
    private const string FirstWaveOrcCountKey = "FirstWaveOrcCount";
    private const string WaveOrcIncreaseKey = "WaveOrcIncrease";
    private const string TimeBetweenWavesKey = "TimeBetweenWaves";
    private const string ClearSurvivalTimeKey = "ClearSurvivalTime";

    public static float MasterVolume
    {
        get => PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume);
        set
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, Mathf.Clamp01(value));
            ApplyMasterVolume();
        }
    }

    public static int FirstWaveOrcCount
    {
        get => PlayerPrefs.GetInt(FirstWaveOrcCountKey, DefaultFirstWaveOrcCount);
        set => PlayerPrefs.SetInt(FirstWaveOrcCountKey, Mathf.Max(1, value));
    }

    public static int WaveOrcIncrease
    {
        get => PlayerPrefs.GetInt(WaveOrcIncreaseKey, DefaultWaveOrcIncrease);
        set => PlayerPrefs.SetInt(WaveOrcIncreaseKey, Mathf.Max(0, value));
    }

    public static float TimeBetweenWaves
    {
        get => PlayerPrefs.GetFloat(TimeBetweenWavesKey, DefaultTimeBetweenWaves);
        set => PlayerPrefs.SetFloat(TimeBetweenWavesKey, Mathf.Max(0.1f, value));
    }

    public static float ClearSurvivalTime
    {
        get => PlayerPrefs.GetFloat(ClearSurvivalTimeKey, DefaultClearSurvivalTime);
        set => PlayerPrefs.SetFloat(ClearSurvivalTimeKey, Mathf.Max(1f, value));
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        ApplyMasterVolume();
    }

    public static void ApplyMasterVolume()
    {
        AudioListener.volume = MasterVolume;
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static void ResetToDefaults()
    {
        MasterVolume = DefaultMasterVolume;
        FirstWaveOrcCount = DefaultFirstWaveOrcCount;
        WaveOrcIncrease = DefaultWaveOrcIncrease;
        TimeBetweenWaves = DefaultTimeBetweenWaves;
        ClearSurvivalTime = DefaultClearSurvivalTime;
        Save();
    }
}
