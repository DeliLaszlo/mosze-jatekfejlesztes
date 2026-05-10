using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("UI References")]
    [SerializeField] public string menuSceneName = "TempMainMenu"; 
    [SerializeField] private GameObject pauseMenuUI; 
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;

        SyncSlidersWithMixer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                ClosePauseMenu();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); 
        Time.timeScale = 0f;        
        GameIsPaused = true;
    }

    public void ClosePauseMenu()
    {
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 1f;         
        GameIsPaused = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(menuSceneName);
    }

    private void SyncSlidersWithMixer()
    {
        if (SoundMixerManager.instance == null) return;

        masterSlider.value = SoundMixerManager.instance.GetMasterVolume();
        musicSlider.value = SoundMixerManager.instance.GetMusicVolume();
        sfxSlider.value = SoundMixerManager.instance.GetSFXVolume();

        masterSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetSFXVolume);
    }
}