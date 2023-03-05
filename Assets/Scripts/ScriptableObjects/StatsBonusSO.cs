using UnityEngine;
using CharacterStats;

[CreateAssetMenu(fileName = "New Bonus", menuName = "Items/Bonus/Object")]
public class StatsBonusSO : BonusSO
{
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

    public ModType type;

    public float amount;
    public override void DoEffect(MainCharacterScript3D player)
    {
        switch (bonusEffect)
        {
            case BonusEffect.DamageBonus:
                //Jouer un event ?
                player.GetPlayerStats().AddModifier(new StatModifier(amount, type, this), StatType.Damage);
                Debug.Log("Bonus Damage !");
                break;

            case BonusEffect.AttackSpeedBonus:
                player.GetPlayerStats().AddModifier(new StatModifier(amount, type, this), StatType.AttackSpeed);

                break;

            case BonusEffect.HealthBonus:
                player.ChangeHealth(amount,Color.green);
                player.GetPlayerStats().AddModifier(new StatModifier(amount, type, this), StatType.MaxHealth);

                break;

            case BonusEffect.CritMultiplierBonus:
                player.GetPlayerStats().AddModifier(new StatModifier(amount, type, this), StatType.CritMultiplier);

                break;

            case BonusEffect.CritChanceBonus:
                player.GetPlayerStats().AddModifier(new StatModifier(amount, type, this), StatType.CritChance);

                break;

            case BonusEffect.MovementSpeedBonus:
                player.GetPlayerStats().AddModifier(new StatModifier(amount, type, this), StatType.MovementSpeed);

                break;
        }
    }
}
