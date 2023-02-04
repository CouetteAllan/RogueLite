using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    private MainCharacterScript3D player;
    private PlayerStats playerStats;
    private Animator animator;
    [SerializeField] private ParticleSystem particleEffect;
    private void Awake()
    {
        player = GetComponent<MainCharacterScript3D>();
        playerStats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
        playerStats.OnPlayerStatChange += PlayerStats_OnPlayerStatChange;

    }

    private void PlayerStats_OnPlayerStatChange(StatType statType)
    {
        ParticleSystem.MainModule settings = particleEffect.main;

        switch (statType)
        {
            case StatType.CritChance:
                settings.startColor = new Color(224f / 255f, 93f / 255f, 0);
                break;
            case StatType.CritMultiplier:
                break;
            case StatType.Damage:
                settings.startColor = Color.red;
                break;
            case StatType.MovementSpeed:
                settings.startColor = Color.yellow;

                break;
            case StatType.MaxHealth:
                settings.startColor = Color.green;
                break;
            case StatType.AttackSpeed:
                break;
        }
        particleEffect.Play();
        animator.SetTrigger("PowerUp");
    }
}