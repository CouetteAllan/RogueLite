using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enchant", menuName = "Items/Bonus/Enchant")]
public class EnchantSO : BonusSO
{
    public IEnchantType enchantType;
    public EffectsEnum enchanteffect;

    public float effectDamage = 1.0f;

    private void CreateEnchant()
    {
        switch (enchanteffect)
        {
            case EffectsEnum.Null:
                break;
            case EffectsEnum.Burn:
                enchantType = new FireEnchant(effectDamage);
                break;
            case EffectsEnum.Poison:
                enchantType = new PoisonEnchant(effectDamage);
                break;
        }
    }

    public override void DoEffect(MainCharacterScript3D player)
    {
        CreateEnchant();
        player.SetEnchant(enchantType);
    }
}
