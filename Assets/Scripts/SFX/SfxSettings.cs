using UnityEngine;
using UnityEngine.UI;

public class SfxSettings : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private bool startWithSoundOn = true;
    [SerializeField] private bool startWithMusicOn = true;

    [Header("Volume Control")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Sound Buttons")]
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Image soundOnImage;
    [SerializeField] private Button soundOffButton;
    [SerializeField] private Image soundOffImage;

    [Header("Music Buttons")]
    [SerializeField] private Button musicOnButton;
    [SerializeField] private Image musicOnImage;
    [SerializeField] private Button musicOffButton;
    [SerializeField] private Image musicOffImage;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicManager;
    [SerializeField] private float maxMusicVolume = 0.5f;

    private bool isSoundOn;
    private bool isMusicOn;
    private float currentMasterVolume = 1f;
    private float currentMusicVolume = 1f;
    private float currentSfxVolume = 1f;

    private void Start()
    {

        LoadSettings();


        isSoundOn = startWithSoundOn;
        isMusicOn = startWithMusicOn;


        InitializeSliders();

        UpdateAllAudioSettings();
        UpdateButtonsState();
    }

    private void InitializeSliders()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = currentMasterVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = currentMusicVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = currentSfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }

    private void LoadSettings()
    {
        currentMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        currentSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
        isSoundOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", currentMasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", currentMusicVolume);
        PlayerPrefs.SetFloat("SfxVolume", currentSfxVolume);
        PlayerPrefs.SetInt("SoundEnabled", isSoundOn ? 1 : 0);
        PlayerPrefs.SetInt("MusicEnabled", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volume)
    {
        currentMasterVolume = Mathf.Clamp01(volume);
        UpdateAllAudioSettings();
        SaveSettings();
    }

    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = Mathf.Clamp01(volume);
        UpdateAllAudioSettings();
        SaveSettings();
    }

    public void SetSfxVolume(float volume)
    {
        currentSfxVolume = Mathf.Clamp01(volume);
        UpdateAllAudioSettings();
        SaveSettings();
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        UpdateAllAudioSettings();
        UpdateButtonsState();
        SaveSettings();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        UpdateAllAudioSettings();
        UpdateButtonsState();
        SaveSettings();
    }

    private void UpdateAllAudioSettings()
    {
 
        if (musicManager != null)
        {
            musicManager.volume = isMusicOn ? currentMusicVolume * currentMasterVolume * maxMusicVolume : 0f;
        }


        if (SoundManager.Instance != null)
        {
            foreach (var sound in SoundManager.Instance.GetSounds())
            {
                sound.source.volume = isSoundOn ? sound.volume * currentSfxVolume * currentMasterVolume : 0f;
            }
        }
    }

    private void UpdateButtonsState()
    {

        if (soundOnButton != null) soundOnButton.gameObject.SetActive(isSoundOn);
        if (soundOnImage != null) soundOnImage.enabled = isSoundOn;
        if (soundOffButton != null) soundOffButton.gameObject.SetActive(!isSoundOn);
        if (soundOffImage != null) soundOffImage.enabled = !isSoundOn;


        if (musicOnButton != null) musicOnButton.gameObject.SetActive(isMusicOn);
        if (musicOnImage != null) musicOnImage.enabled = isMusicOn;
        if (musicOffButton != null) musicOffButton.gameObject.SetActive(!isMusicOn);
        if (musicOffImage != null) musicOffImage.enabled = !isMusicOn;
    }
}