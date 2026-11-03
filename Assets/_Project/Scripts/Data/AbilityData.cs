
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/Ability Data")]
public class AbilityData : ScriptableObject
{
    [Header("Informations")]
    public string abilityName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;

    [Header("Combat")]
    public float damage;
    public float cooldown;

    [Header("Projectile")]
    public float projectileSpeed;
    public Color effectColor = Color.white;
    public GameObject projectilePrefab;

    [Header("Impact")]
    public GameObject impactPrefab;

    [Header("Animation")]
    public string animationTrigger;
}
