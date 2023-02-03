using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bonus", menuName = "Items/Bonus/Effect")]

public class BonusEffectSO : ScriptableObject
{

    public string description = "Insert Bonus description to display";
    public new string name = "Bonus Name";

    public enum BonusEffect
    {
        DamageBonus,
        AttackSpeedBonus,
        HealthBonus,
        CritMultiplierBonus,
        CritChanceBonus,
        MovementSpeedBonus,
    }

    public BonusEffect bonusEffect;

    public float amount;

}
