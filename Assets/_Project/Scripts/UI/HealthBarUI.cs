
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text healthText;

    [Header("Animation smooth de la barre de vie")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease animationEase = Ease.OutQuad;

    private Tween fillTween;

    private void Awake()
    {
        if (health == null)
            Debug.LogError("HealthBarUI : ERREUR health non assigne", this);

        if (fillImage == null)
            Debug.LogError("HealthBarUI : ERREUR fill non assigne", this);

        if (healthText == null)
            Debug.LogError("HealthBarUI : ERREUR health text non assigne", this);
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnHealthChanged += UpdateHealthBar;
    }

    private void Start()
    {
        if (health != null)
            RefreshHealthBar();
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnHealthChanged -= UpdateHealthBar;

        fillTween?.Kill();
    }

    private void RefreshHealthBar()
    {
        UpdateHealthBar(health.CurrentHealth, health.MaxHealth, true);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        UpdateHealthBar(currentHealth, maxHealth, false);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth, bool instant)
    {
        if (fillImage == null || maxHealth <= 0f)
            return;

        float normalizedHealth = Mathf.Clamp01(currentHealth / maxHealth);

        // Mise à jour de la valeur vie
        if (healthText != null)
        {
            healthText.text =
                $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }

        fillTween?.Kill();

        // Animation smooth de la barre
        if (instant)
        {
            fillImage.fillAmount = normalizedHealth;
        }
        else
        {
            fillTween = fillImage .DOFillAmount(normalizedHealth, animationDuration).SetEase(animationEase);
        }
    }
}
