using UnityEngine;

public class BonusScoreManager : MonoBehaviour
{
    public static BonusScoreManager Instance { get; private set; }

    private int currentBonus = 1000;
    private float timer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer -= 1f;
            currentBonus--;

            if (currentBonus <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public int GetBonusScore()
    {
        return currentBonus;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}