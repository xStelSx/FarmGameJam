using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SfxSettings : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioListener audioListener; 
    [SerializeField] private bool startWithSoundOn = true;

    [Header("Button References")]
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Image soundOnImage;
    [SerializeField] private Button soundOffButton;
    [SerializeField] private Image soundOffImage;

    private bool isSoundOn;

    private void Start()
    {

        isSoundOn = startWithSoundOn;
        UpdateSoundState();
        UpdateButtonsState();
    }

    public void ToggleSound()
    {

        isSoundOn = !isSoundOn;
        UpdateSoundState();
        UpdateButtonsState();
    }

    private void UpdateSoundState()
    {

        AudioListener.volume = isSoundOn ? 1f : 0f;


        // if (audioListener != null) audioListener.enabled = isSoundOn;
    }

    private void UpdateButtonsState()
    {

        soundOnButton.gameObject.SetActive(isSoundOn);
        soundOnImage.enabled = isSoundOn;
        soundOnButton.enabled = isSoundOn;

        soundOffButton.gameObject.SetActive(!isSoundOn);
        soundOffImage.enabled = !isSoundOn;
        soundOffButton.enabled = !isSoundOn;
    }
}
