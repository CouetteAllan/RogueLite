using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnchant : IEnchantType
{
    public float fireDamage = 1.0f;
    TickBehaviour fireEffect;
    public FireEnchant() { }

    public FireEnchant(float damage) : this()
    {
        fireDamage = damage;
    }

    public void EnchantEffect(IHittable3D hitObject)
    {
        Entity3D entity = hitObject as Entity3D;
        int tickToBurn = 15;
        int moduloTick = 3; //damage every 3 ticks so every 0.6 seconds
        fireEffect = new TickBehaviour(fireDamage, tickToBurn, moduloTick,EffectsEnum.Burn,entity);
        
    }

}

