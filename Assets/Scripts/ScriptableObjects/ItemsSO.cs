using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items")]
public class ItemsSO : ScriptableObject
{
    public Sprite sprite;
    public RuntimeAnimatorController animator;

    public enum Effect
    {
        Heal,
        DamageBonus,
        Coin
    }
    public Effect effect;
    public float bonusAmountEffect;
    public AudioClip sound;


}