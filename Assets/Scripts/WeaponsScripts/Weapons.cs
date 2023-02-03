using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterStats;

[CreateAssetMenu(fileName = "NewWeapon",menuName = "Weapon",order = 0)]
public class Weapons : ScriptableObject
{
    public Sprite sprite;
    public StatModifier bonusDamage;
    public float damage;
    public float attackRate;
    public float range;

    public string weaponName;
    public string description;

    public enum WeaponType
    {
        Melee,
        Ranged
    }

    public WeaponType weaponType;
    public GameObject projectile;

}
