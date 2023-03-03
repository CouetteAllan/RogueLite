using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEnchant : IEnchantType
{
    public float poisonDamage = 1.0f;
    public void EnchantEffect(IHittable3D hitObject)
    {
        //color le text en rouge
        //empoisonne l'ennemi
        //fais des dégâts bonus
        Entity3D entity = hitObject as Entity3D;
    }

}

public class PoisonBehaviour
{
    private int stack = 0;
    private int tickDamage;
    PoisonBehaviour(float damage, int ticksToDamage)
    {

    }

    public void AddStack(int _stack)
    {
        stack += _stack;
    }
}
