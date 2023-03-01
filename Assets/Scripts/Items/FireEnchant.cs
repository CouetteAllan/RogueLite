using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnchant : IEnchantType
{
    public float fireDamage = 1.0f;
    public void EnchantEffect(IHittable3D hitObject)
    {
        //color le text en rouge
        //brule l'ennemi
        //fais des dégâts bonus
        Entity3D entity = hitObject as Entity3D;
        entity.Burn();
    }

}
