using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnchant : IEnchantType
{
    public float fireDamage = 1.0f;
    FireBehaviour fireEffect;
    public FireEnchant()
    {

    }

    public FireEnchant(float damage) : this()
    {
        fireDamage = damage;
    }

    public void EnchantEffect(IHittable3D hitObject)
    {

        Entity3D entity = hitObject as Entity3D;
        int tickToBurn = 15;
        fireEffect = new FireBehaviour(fireDamage, tickToBurn, entity);
        
    }

}

public class FireBehaviour
{
    private float damage;
    private int fireTick;
    private int fireTickMax;
    private bool isBurning;
    Entity3D entity = null;

    public FireBehaviour(float _damage, int _tickToBurn, Entity3D _entity)
    {
        fireTick = 0;
        fireTickMax = _tickToBurn;
        isBurning = true;
        entity = _entity;
        damage = _damage;
        TimeTickSystem.OnTick += TimeTickSystem_OnTick;
        entity.CallEffect(EffectsEnum.Burn, true);
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (isBurning)
        {
            fireTick++;
            if(fireTick >= fireTickMax)
            {
                isBurning = false;
                entity.CallEffect(EffectsEnum.Burn, false);
            }
            else
            {
                if(fireTick % 3 == 0) //fire tick modulo 3, donc toutes les 3 secondes:
                {
                    ApplyBurn();
                }
            }
        }
    }

    private void ApplyBurn()
    {
        //Setcolor pop up text in a red color
        entity?.ChangeHealth(-damage);
    }

    public void ResetTick()
    {
        fireTick = 0;
        isBurning = true;
    }

    ~FireBehaviour()
    {
        TimeTickSystem.OnTick -= TimeTickSystem_OnTick;

    }
}
