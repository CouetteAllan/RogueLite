using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy",menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public float health;
    public RuntimeAnimatorController animator;
    public float speed;
    public new string name;
    public float damage;
    public float rangeRadius = 2f;

    public Sprite enemySprite;
    public float radiusHitBox;

    public enum Behaviour
    {
        Ranged,
        Melee,
        Pute
    }

    public Behaviour behaviour;

}
