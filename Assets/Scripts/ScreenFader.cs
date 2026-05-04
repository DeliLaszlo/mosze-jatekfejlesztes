using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
        }
    }

    private void Start()
    {
        StartCoroutine(FadeRoutine(0f, 1f));
    }

    public static void FadeOut(float duration = 1f)
    {
        ScreenFader fader = FindAnyObjectByType<ScreenFader>();
        if (fader != null)
        {
            fader.StopAllCoroutines();
            fader.StartCoroutine(fader.FadeRoutine(1f, duration));
        }
    }

    public static void FadeScreen(float duration = 1f, float waitTime = 0.5f)
    {
        ScreenFader fader = FindAnyObjectByType<ScreenFader>();
        if (fader != null)
        {
            fader.StopAllCoroutines();
            fader.StartCoroutine(fader.FullFadeCycle(duration, waitTime));
        }
    }

    public static void FadeIn(float duration = 1f)
    {
        ScreenFader fader = FindAnyObjectByType<ScreenFader>();
        if (fader != null)
        {
            fader.StopAllCoroutines();
            fader.StartCoroutine(fader.FadeRoutine(0f, duration));
        }
    }

    private IEnumerator FullFadeCycle(float duration, float waitTime)
    {
        yield return StartCoroutine(FadeRoutine(1f, duration));
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(FadeRoutine(0f, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        if (targetAlpha > 0f) fadeImage.raycastTarget = true;

        float startAlpha = fadeImage.color.a;
        float timeElapsed = 0f;

        if (duration <= 0f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
            if (targetAlpha == 0f) fadeImage.raycastTarget = false;
            yield break;
        }

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);

        if (targetAlpha == 0f) fadeImage.raycastTarget = false;
    }
}