using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{

    public delegate void EnemySpawnerEvent();
    public EnemySpawnerEvent SpawnEnemy;
    public EnemySpawnerEvent NoMoreEnemies;

    [SerializeField] private int nbEnemies = 0;


    public void AddEnemy()
    {
        nbEnemies++;
    }

    public void SubstractEnemy()
    {
        nbEnemies--;
        if(nbEnemies <= 0)
        {
            NoMoreEnemies?.Invoke();
        }
    }

}
