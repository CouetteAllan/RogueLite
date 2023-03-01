using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EnchantingContext
{
    IEnchantType _enchantType;


    public void DoEffectOnHit(IHittable3D hitObject)
    {
        _enchantType?.EnchantEffect(hitObject);
    }

    public void SetEnchantType(IEnchantType enchant) => this._enchantType = enchant;
}
