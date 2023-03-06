using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectScript : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem burnEffect;
    [SerializeField]
    private ParticleSystem poisonEffect;
    [SerializeField]
    private ParticleSystem iceEffect;
    [SerializeField]
    private EnemyEntity3D enemy;
    private void OnEnable()
    {
        enemy.OnBurn += PlayBurn;
        enemy.OnPoison += Enemy_OnPoison;
        enemy.OnIced += Enemy_OnIced;
    }

    

    private void OnDisable()
    {
        enemy.OnBurn -= PlayBurn;
        enemy.OnPoison -= Enemy_OnPoison;
        enemy.OnIced -= Enemy_OnIced;

    }

    private void Enemy_OnPoison(bool play)
    {
        if (play)
            poisonEffect?.Play();
        else
            poisonEffect?.Stop();
    }

    private void Enemy_OnIced(bool play)
    {
        if (play)
            iceEffect?.Play();
        else
            iceEffect?.Stop();
    }

    private void PlayBurn(bool play)
    {
        if(play)
            burnEffect?.Play();
        else
            burnEffect?.Stop();
    }


}
