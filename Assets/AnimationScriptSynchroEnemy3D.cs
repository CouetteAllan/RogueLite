using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptSynchroEnemy3D : MonoBehaviour
{


    [SerializeField] private EnemyEntity3D enemy;

    private void Awake()
    {
        enemy = this.GetComponent<EnemyEntity3D>();
    }
    public void StartAttack()
    {
        enemy.startAttack();
    }

    public void EndAttack()
    {
        enemy.EndAttack();
    }
}
