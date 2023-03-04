using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickBehaviour
{
    private float damage;
    private int damageTick;
    private int damageTickMax;
    private bool isDamaging;
    private int moduloTick;
    private EffectsEnum effectType;
    Entity3D entity = null;

    delegate void ApplyTickDamage();
    ApplyTickDamage applyDamage;

    public TickBehaviour(float _damage, int _tickToDamage, int _moduloToTick, EffectsEnum _effectType, Entity3D _entity)
    {
        damageTick = 0;
        damageTickMax = _tickToDamage;
        isDamaging = true;
        entity = _entity;
        damage = _damage;
        moduloTick = _moduloToTick;
        effectType = _effectType;
        TimeTickSystem.OnTick += TimeTickSystem_OnTick;
        entity.CallEffect(effectType, true);
        SetApplyingType(_effectType);
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (isDamaging)
        {
            damageTick++;
            if (damageTick >= damageTickMax)
            {
                isDamaging = false;
                entity.CallEffect(effectType, false);
            }
            else
            {
                if (damageTick % moduloTick == 0) 
                {
                    applyDamage();
                }
            }
        }
    }

    private void ApplyBurn()
    {
        //Setcolor pop up text in a red color
        entity?.ChangeHealth(-damage, Color.red);
    }

    private void ApplyPoison()
    {
        //Setcolor pop up text in a purple color
        entity?.ChangeHealth(-damage, new Color(0.5f, 0, 0.5f));
    }

    public void AddStack(int stack)
    {
        damage += stack;
        ResetTick();
    }

    public void ResetTick()
    {
        damageTick = 0;
        isDamaging = true;
        applyDamage();
    }

    private void SetApplyingType(EffectsEnum _effectType)
    {
        switch (_effectType)
        {
            case EffectsEnum.Burn:
                applyDamage = ApplyBurn;
                break;
            case EffectsEnum.Poison:
                applyDamage = ApplyPoison;
                break;
        }
    }

    /*~TickBehaviour()
    {
        TimeTickSystem.OnTick -= TimeTickSystem_OnTick;

    }*/
}

