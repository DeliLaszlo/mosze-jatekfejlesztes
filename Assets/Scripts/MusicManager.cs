using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SceneMusic
{
    public string sceneName;
    public AudioClip musicClip;
}

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Component References")]
    [SerializeField] private AudioSource audioSource;

    [Header("Music Settings")]
    [Tooltip("Add your scene names and their corresponding music tracks here.")]
    [SerializeField] private SceneMusic[] levelMusicMap;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    public void PlayMusicForScene(string sceneName)
    {
        foreach (SceneMusic mapping in levelMusicMap)
        {
            if (mapping.sceneName == sceneName)
            {
                if (audioSource.clip != mapping.musicClip)
                {
                    audioSource.clip = mapping.musicClip;
                    audioSource.Play();
                }
                
                return;
            }
        }
    }
}