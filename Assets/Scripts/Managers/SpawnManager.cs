using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public EnemySO[] enemySOs;
    public EnemyEntity[] enemiesPrefab;


    public void SetAllSpawners()
    {
        foreach (var enemy in enemiesPrefab)
        {
            enemy.enemyData = enemySOs[Random.Range(0, 2)];
        }
    }


    void Start()
    {
        SetAllSpawners();
    }
}
