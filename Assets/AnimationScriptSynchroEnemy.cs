using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptSynchroEnemy : MonoBehaviour
{


    [SerializeField] private EnemyEntity enemy;

    private void Awake()
    {
        enemy = this.GetComponent<EnemyEntity>();
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
