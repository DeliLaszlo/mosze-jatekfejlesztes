using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        if (masterSlider != null) masterSlider.value = PlayerPrefs.GetFloat("masterVolume", 1f);
        if (musicSlider != null) musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1f);
        if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1f);

        if (masterSlider != null) masterSlider.onValueChanged.AddListener(val => SoundMixerManager.instance.SetMasterVolume(val));
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(val => SoundMixerManager.instance.SetMusicVolume(val));
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(val => SoundMixerManager.instance.SetSFXVolume(val));
    }
}