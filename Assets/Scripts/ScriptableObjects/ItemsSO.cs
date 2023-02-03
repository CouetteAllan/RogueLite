using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemsSO : ScriptableObject
{
    public Sprite sprite;
    public RuntimeAnimatorController animator;

    /*public enum Effect
    {
        Heal,
        Money,
        Bonus
    }
    public Effect effect;*/

    public AudioClip sound;
    public abstract void DoEffect(MainCharacterScript3D player);

}