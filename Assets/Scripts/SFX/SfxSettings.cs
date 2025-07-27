using UnityEngine;
using UnityEngine.UI;

public class SfxSettings : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private bool startWithSoundOn = true;
    [SerializeField] private bool startWithMusicOn = true;

    [Header("Sound Buttons")]
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Image soundOnImage;
    [SerializeField] private Button soundOffButton;
    [SerializeField] private Image soundOffImage;

    [Header("Music Settings")]
    [SerializeField] private AudioSource musicManager;
    [SerializeField] private float musicVolume = 0.009f;

    [Header("Music Buttons")]
    [SerializeField] private Button musicOnButton;
    [SerializeField] private Image musicOnImage;
    [SerializeField] private Button musicOffButton;
    [SerializeField] private Image musicOffImage;

    private bool isSoundOn;
    private bool isMusicOn;

    private void Start()
    {
        isSoundOn = startWithSoundOn;
        isMusicOn = startWithMusicOn;

        UpdateSoundState();
        UpdateMusicState();
        UpdateButtonsState();
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        UpdateSoundState();
        UpdateButtonsState();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        UpdateMusicState();
        UpdateButtonsState();
    }

    private void UpdateSoundState()
    {
        if (SoundManager.Instance != null)
        {

            foreach (var sound in SoundManager.Instance.GetSounds())
            {
                sound.source.volume = isSoundOn ? sound.volume : 0f;
            }
        }
    }

    private void UpdateMusicState()
    {
        if (musicManager != null)
        {
            musicManager.volume = isMusicOn ? musicVolume : 0f;
        }
    }

    private void UpdateButtonsState()
    {

        soundOnButton.gameObject.SetActive(isSoundOn);
        soundOnImage.enabled = isSoundOn;
        soundOffButton.gameObject.SetActive(!isSoundOn);
        soundOffImage.enabled = !isSoundOn;


        musicOnButton.gameObject.SetActive(isMusicOn);
        musicOnImage.enabled = isMusicOn;
        musicOffButton.gameObject.SetActive(!isMusicOn);
        musicOffImage.enabled = !isMusicOn;
    }
}