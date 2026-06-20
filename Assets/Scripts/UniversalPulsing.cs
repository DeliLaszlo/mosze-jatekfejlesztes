using System.Collections;
using UnityEngine;

public class UniversalPulsing : MonoBehaviour
{
    [Header("Pulse settings")]
    [SerializeField] private float approachSpeed = 0.0005f;
    [SerializeField] private float growthBound = 1.3f;
    [SerializeField] private float shrinkBound = 0.8f;
    [SerializeField] private bool playOnAwake = true;

    private float currentRatio = 1f;
    private Vector3 originalScale;
    private Coroutine pulseRoutine;

    private void Awake()
    {
        originalScale = transform.localScale;
        currentRatio = 1f;

        if (playOnAwake)
        {
            StartPulsing();
        }
    }

    public void StartPulsing()
    {
        if (pulseRoutine == null)
        {
            pulseRoutine = StartCoroutine(Pulse());
        }
    }

    public void StopPulsing()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }
    }

    private IEnumerator Pulse()
    {
        while (true)
        {
            yield return ScaleTo(growthBound);
            yield return ScaleTo(shrinkBound);
        }
    }

    private IEnumerator ScaleTo(float targetRatio)
    {
        while (!Mathf.Approximately(currentRatio, targetRatio))
        {
            currentRatio = Mathf.MoveTowards(currentRatio, targetRatio, approachSpeed);
            transform.localScale = originalScale * currentRatio;

            yield return null;
        }
    }
}