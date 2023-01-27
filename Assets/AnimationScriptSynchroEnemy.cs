using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptSynchroEnemy : MonoBehaviour
{

    [SerializeField] private EnemyEntity enemy;
    public void StartAttack()
    {
        enemy.StartAttack();
    }

    public void EndAttack()
    {
        enemy.EndAttack();
    }
}