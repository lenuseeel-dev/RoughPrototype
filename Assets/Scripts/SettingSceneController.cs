using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingSceneController : MonoBehaviour
{
    [Header("Volume")]
    public Slider masterVolumeSlider;
    public TMP_Text masterVolumeValueText;

    [Header("Orc Count")]
    public TMP_InputField firstWaveOrcInput;
    public TMP_InputField waveOrcIncreaseInput;

    [Header("Scene")]
    public string introSceneName = "IntroScene";

    private bool isRefreshing;

    private void Start()
    {
        ValidateUiReferences();
        GameSettings.ApplyMasterVolume();
        RefreshUI();
    }

    public void OnMasterVolumeChanged(float value)
    {
        if (isRefreshing)
        {
            return;
        }

        GameSettings.MasterVolume = value;
        UpdateVolumeText(value);
    }

    public void OnFirstWaveOrcChanged(string value)
    {
        if (isRefreshing)
        {
            return;
        }

        GameSettings.FirstWaveOrcCount = ParseInt(value, GameSettings.DefaultFirstWaveOrcCount, 1);
        RefreshInputValues();
    }

    public void OnWaveOrcIncreaseChanged(string value)
    {
        if (isRefreshing)
        {
            return;
        }

        GameSettings.WaveOrcIncrease = ParseInt(value, GameSettings.DefaultWaveOrcIncrease, 0);
        RefreshInputValues();
    }

    public void SaveAndBack()
    {
        SaveCurrentValues();
        SceneManager.LoadScene(introSceneName);
    }

    public void ResetSettings()
    {
        GameSettings.ResetToDefaults();
        RefreshUI();
    }

    private void SaveCurrentValues()
    {
        if (masterVolumeSlider != null)
        {
            GameSettings.MasterVolume = masterVolumeSlider.value;
        }

        if (firstWaveOrcInput != null)
        {
            GameSettings.FirstWaveOrcCount = ParseInt(firstWaveOrcInput.text, GameSettings.DefaultFirstWaveOrcCount, 1);
        }

        if (waveOrcIncreaseInput != null)
        {
            GameSettings.WaveOrcIncrease = ParseInt(waveOrcIncreaseInput.text, GameSettings.DefaultWaveOrcIncrease, 0);
        }

        GameSettings.Save();
    }

    private void RefreshUI()
    {
        isRefreshing = true;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.value = GameSettings.MasterVolume;
        }

        RefreshInputValues();
        UpdateVolumeText(GameSettings.MasterVolume);

        isRefreshing = false;
    }

    private void RefreshInputValues()
    {
        if (firstWaveOrcInput != null)
        {
            firstWaveOrcInput.text = GameSettings.FirstWaveOrcCount.ToString();
        }

        if (waveOrcIncreaseInput != null)
        {
            waveOrcIncreaseInput.text = GameSettings.WaveOrcIncrease.ToString();
        }
    }

    private void UpdateVolumeText(float value)
    {
        if (masterVolumeValueText != null)
        {
            masterVolumeValueText.text = $"{Mathf.RoundToInt(value * 100f)}%";
        }
    }

    private int ParseInt(string value, int fallback, int minValue)
    {
        if (!int.TryParse(value, out int result))
        {
            result = fallback;
        }

        return Mathf.Max(minValue, result);
    }

    private void ValidateUiReferences()
    {
        if (masterVolumeSlider == null || masterVolumeValueText == null || firstWaveOrcInput == null || waveOrcIncreaseInput == null)
        {
            Debug.LogWarning("SettingSceneController: Inspector에 UI 참조를 모두 연결해야 합니다.\n- Master Volume Slider\n- Master Volume Value Text\n- First Wave Orc Input\n- Wave Orc Increase Input");
        }
    }
}

