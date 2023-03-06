using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Enchant", menuName = "Items/Bonus/Enchant")]
public class EnchantSO : BonusSO
{
    public List<IEnchantType> enchantTypesList = new List<IEnchantType>();

    public EnchantTypeAndDamage[] enchantsTypesAndEffect;

    private void CreateEnchant()
    {
        foreach(EnchantTypeAndDamage enchant in enchantsTypesAndEffect)
        {
            switch (enchant.enchanteffect)
            {
                case EffectsEnum.Null:
                    break;
                case EffectsEnum.Burn:
                    enchantTypesList.Add(new FireEnchant(enchant.baseDamage));
                    break;
                case EffectsEnum.Poison:
                    enchantTypesList.Add(new PoisonEnchant(enchant.baseDamage));
                    break;
            }
        }
        
    }

    public override void DoEffect(MainCharacterScript3D player)
    {
        CreateEnchant();
        foreach(var enchant in enchantTypesList)
        {
            player.AddEnchant(enchant);

        }
    }
}

[Serializable]
public struct EnchantTypeAndDamage
{
    public EffectsEnum enchanteffect;
    public float baseDamage;
}
