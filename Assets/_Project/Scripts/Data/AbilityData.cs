
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/Ability Data")]
public class AbilityData : ScriptableObject
{
    [Header("Infos")]
    public string abilityName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;

    [Header("Stats")]
    public float damage;
    public float cooldown;

    [Header("Projectile")]
    public float projectileSpeed;
    public Color effectColor = Color.white;
    public GameObject projectilePrefab;

    [Header("VFX")]
    public GameObject impactPrefab;
    public string animationTrigger;
}
