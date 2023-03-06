using UnityEngine;
using System;

public class TickBehaviour
{
    private float damage;
    private float baseDamageTick;
    private float baseMoveSpeed;
    private int damageTick;
    private int damageTickMax;
    private bool isDamaging;
    private int moduloTick;
    private int stacks = 0;
    private EffectsEnum effectType;
    Entity3D entity = null;

    delegate void ApplyTickDamage();
    ApplyTickDamage applyEffect;


    public TickBehaviour(float _damage, int _tickToDamage, int _moduloToTick, EffectsEnum _effectType, Entity3D _entity)
    {
        damageTick = 0;
        damageTickMax = _tickToDamage;
        baseMoveSpeed = _entity.MovementSpeed;
        isDamaging = true;
        entity = _entity;
        damage = _damage;
        baseDamageTick = _damage;
        moduloTick = _moduloToTick;
        effectType = _effectType;
        stacks = 1;
        TimeTickSystem.OnTick += TimeTickSystem_OnTick;
        SetApplyingType(_effectType);
        entity.CallEffect(effectType, true);

    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (isDamaging)
        {
            damageTick++;
            if (damageTick >= damageTickMax)
            {
                EndEffect();
            }
            else
            {
                if (damageTick % moduloTick == 0) 
                {
                    applyEffect();
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

    private void ApplySlow()
    {
        Debug.Log(entity.MovementSpeed);
        float newSpeed = entity.MovementSpeed / ((damage / 100f) + 1);
        entity.MovementSpeed = newSpeed;
    }


    public void AddStack(int stack)
    {
        damage += stack;
        stacks++;
        ResetTick();
    }

    public void ResetTick()
    {
        damageTick = 0;
        isDamaging = true;
        applyEffect();
    }

    private void EndEffect()
    {
        isDamaging = false;
        entity.CallEffect(effectType, false);
        this.damage = baseDamageTick;
        entity.StatusDictionary.Remove(effectType);
        stacks = 0;
        if (effectType == EffectsEnum.Ice)
            entity.MovementSpeed = baseMoveSpeed;
    }

    private void SetApplyingType(EffectsEnum _effectType)
    {
        switch (_effectType)
        {
            case EffectsEnum.Burn:
                applyEffect = ApplyBurn;
                break;
            case EffectsEnum.Poison:
                applyEffect = ApplyPoison;
                break;
            case EffectsEnum.Null:
                break;
            case EffectsEnum.Ice:
                applyEffect = ApplySlow;
                applyEffect();
                break;
        }
    }

}

