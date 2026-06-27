using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public enum ScreenMode
    {
        MainManu,
        Game
    }
    
    [SerializeField] private ScreenMode mode;
    
    [Header("UI Sliders")]
    [SerializeField] private Slider generalSoundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider difficultySlider;

    [Header("Difficulty Labels")]
    [SerializeField] private TextMeshProUGUI easyLabel;
    [SerializeField] private TextMeshProUGUI mediumLabel;
    [SerializeField] private TextMeshProUGUI hardLabel;

    [Header("Buttons")]
    [SerializeField] private GameObject mainMenuButton;
    
    private const string GeneralSoundKey = "GeneralSoundVolume";
    private const string MusicKey = "MusicVolume";
    private const string DifficultyKey = "GameDifficulty";

    void OnEnable()
    {
        LoadSettings();
        
        generalSoundSlider.onValueChanged.AddListener(OnGeneralSoundChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
    }

    void OnDisable()
    {
        generalSoundSlider.onValueChanged.RemoveListener(OnGeneralSoundChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        difficultySlider.onValueChanged.RemoveListener(OnDifficultyChanged);
    }

    private void Start()
    {
        if (mode == ScreenMode.MainManu)
        {
            mainMenuButton.SetActive(false);
        }
        else
        {
            mainMenuButton.SetActive(true);
        }
    }

    private void LoadSettings()
    {
        float savedGeneral = PlayerPrefs.GetFloat(GeneralSoundKey, 80f);
        float savedMusic = PlayerPrefs.GetFloat(MusicKey, 60f);
        int savedDifficulty = PlayerPrefs.GetInt(DifficultyKey, 2); 

        generalSoundSlider.value = savedGeneral;
        musicSlider.value = savedMusic;
        difficultySlider.value = savedDifficulty;

        UpdateDifficultyText(savedDifficulty);
    }

    private void OnGeneralSoundChanged(float value)
    {
        PlayerPrefs.SetFloat(GeneralSoundKey, value);
    
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UpdateVolumeFromPrefs();
        }
    }

    private void OnMusicChanged(float value)
    {
        PlayerPrefs.SetFloat(MusicKey, value);
    
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UpdateVolumeFromPrefs();
        }
    }

    private void OnDifficultyChanged(float value)
    {
        int difficultyIndex = Mathf.RoundToInt(value);
        PlayerPrefs.SetInt(DifficultyKey, difficultyIndex);
        UpdateDifficultyText(difficultyIndex);
    }

    private void UpdateDifficultyText(int difficulty)
    {
        Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Color activeColor = Color.white; 

        easyLabel.color = inactiveColor;
        mediumLabel.color = inactiveColor;
        hardLabel.color = inactiveColor;

        switch (difficulty)
        {
            case 1:
                easyLabel.color = activeColor;
                break;
            case 2:
                mediumLabel.color = activeColor;
                break;
            case 3:
                hardLabel.color = activeColor;
                break;
        }
    }
    
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}