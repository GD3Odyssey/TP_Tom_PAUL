
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [Header("Liste des competences")]
    [SerializeField] private List<AbilityData> abilities = new List<AbilityData>();

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    [SerializeField] private Transform projectileSpawnPoint;

    private readonly HashSet<AbilityData> coolingDown = new HashSet<AbilityData>();
    private readonly Dictionary<AbilityData, Coroutine> cooldownCoroutines = new Dictionary<AbilityData, Coroutine>();

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
        cooldownCoroutines.Clear();

        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] == null)
                Debug.LogWarning($"AbilityManager : capacité non renseignée {i}", this);
        }

        Debug.Log($"AbilityManager : {abilities.Count} compétences configurées", this);
    }

    // Utilisation via index dans la liste
    public bool TryUseAbility(int abilityIndex)
    {
        if (abilityIndex < 0 || abilityIndex >= abilities.Count)
        {
            Debug.LogWarning("AbilityManager : identifiant de compétence invalide", this);
            return false;
        }

        return TryUseAbility(abilities[abilityIndex]);
    }

    // Utilisation via ScriptableObject => AbilityData
    public bool TryUseAbility(AbilityData ability)
    {
        if (ability == null)
        {
            Debug.LogWarning("AbilityManager : erreur compétence nulle", this);
            return false;
        }

        if (!abilities.Contains(ability))
        {
            Debug.LogWarning("AbilityManager : erreur compétence absente de la liste", this);
            return false;
        }

        if (coolingDown.Contains(ability))
            return false;

        if (animator == null || target == null || projectileSpawnPoint == null)
        {
            Debug.LogError("AbilityManager : erreur il manque une ref", this);
            return false;
        }

        if (ability.projectilePrefab == null)
        {
            Debug.LogWarning($"AbilityManager : aucun projectile assigné {ability.abilityName}.", this);
            return false;
        }

        LaunchAbility(ability);

        return true;
    }

    private void LaunchAbility(AbilityData ability)
    {
        // animation d'attaque du personnage
        animator.ResetTrigger(ability.animationTrigger);
        animator.SetTrigger(ability.animationTrigger);

        // projectile instantié
        GameObject projectile = Instantiate(ability.projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        // cooldown
        StartCooldown(ability);

        float distance = Vector3.Distance(projectile.transform.position, target.position);

        float duration = distance / Mathf.Max(ability.projectileSpeed, 0.01f);

        // Trajectoire gérée via DOTween
        projectile.transform.DOMove(target.position, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => OnProjectileImpact(projectile, ability));
    }

    private void OnProjectileImpact(GameObject projectile, AbilityData ability)
    {
        // Gestion des dégâts et VFX d'impact
        if (target != null)
        {
            target.SendMessage("TakeDamage", ability.damage, SendMessageOptions.DontRequireReceiver);

            if (ability.impactPrefab != null)
            {
                Instantiate(ability.impactPrefab, target.position, Quaternion.identity);
            }
        }

        if (projectile != null)
            Destroy(projectile);
    }

    // cooldown
    private void StartCooldown(AbilityData ability)
    {
        if (ability.cooldown <= 0f)
        {
            OnCooldownFinished?.Invoke(ability);
            return;
        }

        coolingDown.Add(ability);

        OnCooldownStarted?.Invoke(ability, ability.cooldown);

        Coroutine routine = StartCoroutine(CooldownRoutine(ability));
        cooldownCoroutines[ability] = routine;
    }

    private IEnumerator CooldownRoutine(AbilityData ability)
    {
        yield return new WaitForSeconds(ability.cooldown);

        coolingDown.Remove(ability);
        cooldownCoroutines.Remove(ability);

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
