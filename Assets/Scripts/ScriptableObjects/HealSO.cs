using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Heal", menuName = "Items/Heal")]
public class HealSO : ItemsSO
{
    public event Action OnPickUpHealItem;
    public float healAmount = 10f;
    public override void DoEffect(MainCharacterScript3D player)
    {
        player.ChangeHealth(healAmount);
        //jouer un audio
        //jouer un event peut-être ?
        OnPickUpHealItem?.Invoke();
    }
}
