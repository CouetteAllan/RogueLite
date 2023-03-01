using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectScript : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem burnEffect;
    [SerializeField]
    private EnemyEntity3D enemy;
    private void OnEnable()
    {
        enemy.OnBurn += PlayBurn;
    }

    private void PlayBurn(bool play)
    {
        if(play)
            burnEffect.Play();
        else
            burnEffect.Stop();
    }


}
