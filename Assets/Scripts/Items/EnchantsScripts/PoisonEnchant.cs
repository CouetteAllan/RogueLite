using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEnchant : IEnchantType
{
    public float poisonDamage = 1.0f;

    public PoisonEnchant() { }

    public PoisonEnchant(float damage) : this()
    {
        poisonDamage = damage;
    }

    public void EnchantEffect(IHittable3D hitObject)
    {
        Entity3D entity = hitObject as Entity3D;
        if (entity == null)
            return;
        int tickToPoison = 20; //4 seconds
        int moduloTick = 4; //every 4 ticks = every 0.8 seconds


        if (entity.StatusDictionary.TryGetValue(EffectsEnum.Poison, out TickBehaviour tick))
        {
            tick.AddStack(1);
        }
        else
        {
            entity.StatusDictionary.Add(EffectsEnum.Poison, new TickBehaviour(poisonDamage, tickToPoison, moduloTick, EffectsEnum.Poison, entity));
        }


    }

}

