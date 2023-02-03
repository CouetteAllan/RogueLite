using UnityEngine;


[CreateAssetMenu(fileName = "New Bonus", menuName = "Items/Bonus/Object")]
public class StatsBonusSO : ItemsSO
{
    public BonusEffectSO bonusEffectData;
    public override void DoEffect(MainCharacterScript3D player)
    {
        switch (bonusEffectData.bonusEffect)
        {
            case BonusEffectSO.BonusEffect.DamageBonus:
                //Jouer un event ?
                player.GetPlayerStats().SetDamage(bonusEffectData.amount);
                Debug.Log("Bonus Damage !");
                break;

            case BonusEffectSO.BonusEffect.AttackSpeedBonus:
                player.GetPlayerStats().SetAttckSpeed(bonusEffectData.amount);
                break;

            case BonusEffectSO.BonusEffect.HealthBonus:
                player.GetPlayerStats().SetBonusHealth(bonusEffectData.amount);
                break;

            case BonusEffectSO.BonusEffect.CritMultiplierBonus:
                player.GetPlayerStats().SetCritMultiplier(bonusEffectData.amount);
                break;

            case BonusEffectSO.BonusEffect.CritChanceBonus:
                player.GetPlayerStats().SetCritChance(bonusEffectData.amount);
                break;

            case BonusEffectSO.BonusEffect.MovementSpeedBonus:
                player.GetPlayerStats().SetSpeed(bonusEffectData.amount);
                break;
        }
    }
}
