using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public CanvasGroup[] panels; 
    public float secondsPerImage = 3f;
    public float fadeDuration = 1f;
    public string nextScene = "level1";
    
    public GameObject spaceButtonUI;

    private int currentIndex = 0;
    private Coroutine timerCoroutine;
    private bool isFading = false;

    void Start()
    {
        if (spaceButtonUI != null)
        {
            spaceButtonUI.SetActive(true);
        }

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].alpha = (i == 0) ? 1f : 0f;
            panels[i].gameObject.SetActive(i == 0);
        }

        timerCoroutine = StartCoroutine(AutoSwitch());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isFading)
        {
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            SwitchToNext();
        }
    }

    IEnumerator AutoSwitch()
    {
        yield return new WaitForSeconds(secondsPerImage);
        
        if (!isFading)
        {
            SwitchToNext();
        }
    }

    void SwitchToNext()
    {
        if (currentIndex < panels.Length - 1)
        {
            StartCoroutine(CrossfadePanel(currentIndex, currentIndex + 1));
        }
        else
        {
            MusicManager.instance.PlayMusicForScene(nextScene);
            SceneManager.LoadScene(nextScene);
        }
    }

    IEnumerator CrossfadePanel(int fadeOutIndex, int fadeInIndex)
    {
        isFading = true;
        currentIndex++;

        if (currentIndex > 0 && spaceButtonUI != null)
        {
            spaceButtonUI.SetActive(false);
        }

        CanvasGroup outPanel = panels[fadeOutIndex];
        CanvasGroup inPanel = panels[fadeInIndex];

        inPanel.gameObject.SetActive(true);
        inPanel.alpha = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            outPanel.alpha = Mathf.Lerp(1f, 0f, t);
            inPanel.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        outPanel.alpha = 0f;
        outPanel.gameObject.SetActive(false);
        inPanel.alpha = 1f;

        isFading = false;

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(AutoSwitch());
    }
}