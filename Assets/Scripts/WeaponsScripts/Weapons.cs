using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewWeapon",menuName = "Weapon",order = 0)]
public class Weapons : ScriptableObject
{
    public Sprite sprite;
    public float damage;
    public float attackRate;
    public float range;

    public string weaponName;
    public string description;

}
