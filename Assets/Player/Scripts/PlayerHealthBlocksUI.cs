using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBlocksUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealthManager playerHealth;
    [SerializeField] private RectTransform blocksContainer;

    [Header("Block Layout")]
    [SerializeField] private Vector2 blockSize = new Vector2(16f, 16f);
    [SerializeField] private float spacing = 4f;

    [Header("Colors")]
    [SerializeField] private Color highHealthColor = new Color(0.2f, 0.85f, 0.3f, 1f);
    [SerializeField] private Color mediumHealthColor = new Color(1f, 0.65f, 0.2f, 1f);
    [SerializeField] private Color lowHealthColor = new Color(0.95f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.35f);

    [Header("Thresholds")]
    [SerializeField, Range(0f, 1f)] private float mediumHealthThreshold = 0.66f;
    [SerializeField, Range(0f, 1f)] private float lowHealthThreshold = 0.33f;

    private readonly List<Image> blocks = new List<Image>();

    private void Awake()
    {
        if (blocksContainer == null)
        {
            blocksContainer = transform as RectTransform;
        }

        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealthManager>();
        }

        EnsureHorizontalLayout();
    }

    private void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning($"{name}: No PlayerHealthManager found for health UI.", this);
            return;
        }

        BuildBlocks(playerHealth.MaxHealth);
        RefreshBlocks(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged += RefreshBlocks;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= RefreshBlocks;
        }
    }

    private void BuildBlocks(int blockCount)
    {
        ClearBlocks();

        int clampedCount = Mathf.Max(1, blockCount);
        for (int i = 0; i < clampedCount; i++)
        {
            GameObject blockObject = new GameObject($"HP_Block_{i + 1}", typeof(RectTransform), typeof(Image));
            blockObject.transform.SetParent(blocksContainer, false);

            RectTransform rect = blockObject.GetComponent<RectTransform>();
            rect.sizeDelta = blockSize;

            Image image = blockObject.GetComponent<Image>();
            image.color = highHealthColor;
            blocks.Add(image);
        }
    }

    private void ClearBlocks()
    {
        for (int i = blocksContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(blocksContainer.GetChild(i).gameObject);
        }

        blocks.Clear();
    }

    private void RefreshBlocks(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0)
        {
            return;
        }

        if (blocks.Count != maxHealth)
        {
            BuildBlocks(maxHealth);
        }

        float healthRatio = Mathf.Clamp01((float)currentHealth / maxHealth);
        Color activeColor = GetActiveColor(healthRatio);

        for (int i = 0; i < blocks.Count; i++)
        {
            bool isFilled = i < currentHealth;
            blocks[i].color = isFilled ? activeColor : emptyColor;
        }
    }

    private Color GetActiveColor(float healthRatio)
    {
        if (healthRatio <= lowHealthThreshold)
        {
            return lowHealthColor;
        }

        if (healthRatio <= mediumHealthThreshold)
        {
            return mediumHealthColor;
        }

        return highHealthColor;
    }

    private void EnsureHorizontalLayout()
    {
        if (blocksContainer == null)
        {
            return;
        }

        HorizontalLayoutGroup layoutGroup = blocksContainer.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = blocksContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
        }

        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.spacing = spacing;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (blocksContainer != null)
        {
            HorizontalLayoutGroup layoutGroup = blocksContainer.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup != null)
            {
                layoutGroup.spacing = spacing;
            }
        }

        if (mediumHealthThreshold < lowHealthThreshold)
        {
            mediumHealthThreshold = lowHealthThreshold;
        }
    }
#endif
}
