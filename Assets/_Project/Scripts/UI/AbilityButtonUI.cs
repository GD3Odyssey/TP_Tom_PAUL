
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TMP_Text abilityNameText;
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Cooldown smooth")]
    [SerializeField] private float disabledAlpha = 0.4f;

    private AbilityData ability;
    private AbilityManager abilityManager;

    private Tween fillTween;
    private Tween alphaTween;

    public void Setup(AbilityData data, AbilityManager manager)
    {
        ability = data;
        abilityManager = manager;

        if (iconImage == null)
            iconImage = GetComponent<Image>();

        if (button == null)
            button = GetComponent<Button>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (abilityNameText == null)
            abilityNameText = GetComponentInChildren<TMP_Text>(true);

        if (cooldownOverlay == null)
        {
            Transform overlay = transform.Find("CooldownOverlay");

            if (overlay != null)
                cooldownOverlay = overlay.GetComponent<Image>();
        }

        if (ability == null || abilityManager == null || iconImage == null || button == null || canvasGroup == null || cooldownOverlay == null)
        {
            Debug.LogError($"AbilityButtonUI : ERREUR reference manquante sur {gameObject.name}", this);

            button.interactable = false;
            return;
        }

        // Affichage
        if (ability.icon != null)
            iconImage.sprite = ability.icon;

        if (abilityNameText != null)
            abilityNameText.text = ability.abilityName;

        // Etat initial
        cooldownOverlay.fillAmount = 0f;
        canvasGroup.alpha = 1f;
        button.interactable = true;

        button.onClick.RemoveListener(HandleClick);
        button.onClick.AddListener(HandleClick);

        abilityManager.OnCooldownStarted += HandleCooldownStarted;
        abilityManager.OnCooldownFinished += HandleCooldownFinished;
    }

    private void HandleClick()
    {
        if (abilityManager == null || ability == null)
            return;

        abilityManager.TryUseAbility(ability);
    }

    // Cooldown
    private void HandleCooldownStarted(AbilityData data, float duration)
    {
        if (data != ability)
            return;

        fillTween?.Kill();
        alphaTween?.Kill();

        button.interactable = false;

        // L'overlay se vide progressivement pendant le cooldown
        cooldownOverlay.fillAmount = 1f;

        fillTween = cooldownOverlay .DOFillAmount(0f, duration).SetEase(Ease.Linear);

        // Le bouton retrouve progressivement sa couleur
        canvasGroup.alpha = disabledAlpha;

        alphaTween = canvasGroup .DOFade(1f, duration).SetEase(Ease.Linear);
    }

    // Fin du cooldown
    private void HandleCooldownFinished(AbilityData data)
    {
        if (data != ability)
            return;

        fillTween?.Kill();
        alphaTween?.Kill();

        cooldownOverlay.fillAmount = 0f;
        canvasGroup.alpha = 1f;

        button.interactable = true;
    }

    private void OnDestroy()
    {
        if (abilityManager != null)
        {
            abilityManager.OnCooldownStarted -= HandleCooldownStarted;
            abilityManager.OnCooldownFinished -= HandleCooldownFinished;
        }

        fillTween?.Kill();
        alphaTween?.Kill();
    }
}
