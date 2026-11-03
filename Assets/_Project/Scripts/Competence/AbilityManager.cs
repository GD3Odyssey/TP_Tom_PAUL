
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [Header("Liste des capacites")]
    [SerializeField] private List<AbilityData> abilities = new List<AbilityData>();

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    [SerializeField] private Transform projectileSpawnPoint;

    private readonly HashSet<AbilityData> coolingDown = new HashSet<AbilityData>();

    public IReadOnlyList<AbilityData> Abilities => abilities;

    public event Action<AbilityData, float> OnCooldownStarted;
    public event Action<AbilityData> OnCooldownFinished;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        InitializeAbilities();
    }

    public void InitializeAbilities()
    {
        StopAllCoroutines();
        coolingDown.Clear();

        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] == null)
                Debug.LogWarning($"AbilityManager : ERREUR capacite absente {i}", this);
        }

        Debug.Log($"AbilityManager : {abilities.Count} capacites initialisees", this);
    }

    // Utilisation via identifiant
    public bool TryUseAbility(int abilityIndex)
    {
        if (abilityIndex < 0 || abilityIndex >= abilities.Count)
        {
            Debug.LogWarning("AbilityManager : ERREUR identifiant invalide", this);
            return false;
        }

        return TryUseAbility(abilities[abilityIndex]);
    }

    // Utilisation via ressource AbilityData
    public bool TryUseAbility(AbilityData ability)
    {
        if (ability == null || !abilities.Contains(ability))
            return false;

        if (coolingDown.Contains(ability))
            return false;

        if (animator == null || target == null || projectileSpawnPoint == null)
        {
            Debug.LogError("AbilityManager : ERREUR reference manquante", this);
            return false;
        }

        if (ability.projectilePrefab == null)
        {
            Debug.LogWarning($"Aucun projectile assigne a {ability.abilityName}", this);
            return false;
        }

        if (ability.projectileSpeed <= 0f)
        {
            Debug.LogWarning($"Vitesse invalide pour {ability.abilityName}", this);
            return false;
        }

        LaunchAbility(ability);
        return true;
    }

    private void LaunchAbility(AbilityData ability)
    {
        // Animation du joueur
        if (!string.IsNullOrEmpty(ability.animationTrigger))
        {
            animator.ResetTrigger(ability.animationTrigger);
            animator.SetTrigger(ability.animationTrigger);
        }

        // Trajectoire du projectile
        Vector3 startPosition = projectileSpawnPoint.position;
        Vector3 targetPosition = target.position;

        // Projectile instantié
        GameObject projectile = Instantiate(ability.projectilePrefab, startPosition, projectileSpawnPoint.rotation);

        // Couleur du projectile en fonction de sa nature
        ApplyProjectileColor(projectile, ability.effectColor);

        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / ability.projectileSpeed;

        StartCooldown(ability);

        // Trajectoire via DOTween
        projectile.transform.DOMove(targetPosition, duration).SetEase(Ease.Linear).SetLink(projectile, LinkBehaviour.KillOnDestroy).OnComplete(() =>
        {
            OnProjectileImpact(projectile, ability, targetPosition);
        });
    }

    private void ApplyProjectileColor(GameObject projectile, Color color)
    {
        Renderer[] renderers = projectile.GetComponentsInChildren<Renderer>();

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        foreach (Renderer renderer in renderers)
        {
            renderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetColor("_BaseColor", color);
            propertyBlock.SetColor("_Color", color);

            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void OnProjectileImpact(GameObject projectile, AbilityData ability, Vector3 impactPosition)
    {
        // Application des degats

        if (target != null)
        {
            target.SendMessage("TakeDamage", ability.damage, SendMessageOptions.DontRequireReceiver);
        }

        // VFX
        if (ability.impactPrefab != null)
        {
            Instantiate(ability.impactPrefab, impactPosition, Quaternion.identity);
        }

        if (projectile != null)
            Destroy(projectile);
    }

    // Cooldown
    private void StartCooldown(AbilityData ability)
    {
        if (ability.cooldown <= 0f)
        {
            OnCooldownFinished?.Invoke(ability);
            return;
        }

        coolingDown.Add(ability);

        OnCooldownStarted?.Invoke(ability, ability.cooldown);

        StartCoroutine(CooldownRoutine(ability));
    }

    private IEnumerator CooldownRoutine(AbilityData ability)
    {
        yield return new WaitForSeconds(ability.cooldown);

        coolingDown.Remove(ability);

        OnCooldownFinished?.Invoke(ability);
    }

    public bool IsAbilityAvailable(AbilityData ability)
    {
        return ability != null && abilities.Contains(ability) && !coolingDown.Contains(ability);
    }

    public float GetCooldownDuration(AbilityData ability)
    {
        return ability != null ? ability.cooldown : 0f;
    }
}
