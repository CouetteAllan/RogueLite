using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceEnchant : IEnchantType
{
    float percentMovementSpeedReduce = 30.0f;

    public IceEnchant() { }

    public IceEnchant(float _percentMovementSpeedReduce) : this()
    {
        percentMovementSpeedReduce = _percentMovementSpeedReduce;
    }

    public void EnchantEffect(IHittable3D hitObject)
    {
        Entity3D entity = hitObject as Entity3D;
        int tickToSlow = 25;
        int moduloTick = 25; //damage every 5 ticks so every seconds
        if (entity.StatusDictionary.TryGetValue(EffectsEnum.Ice, out TickBehaviour tick))
        {

            tick.ResetTick();
        }
        else
        {
            entity.StatusDictionary.Add(EffectsEnum.Ice, new TickBehaviour(percentMovementSpeedReduce, tickToSlow, moduloTick, EffectsEnum.Ice, entity));
        }
    }
}
