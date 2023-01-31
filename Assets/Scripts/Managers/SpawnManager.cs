using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] GameObject[] spawnerPoints;

    public delegate void EnemySpawnerEvent();
    public EnemySpawnerEvent OnEnemySpawn;
    public EnemySpawnerEvent OnEnemySetSpawn;


    public void SetAllSpawners()
    {

        foreach (var spawner in spawnerPoints)
        {
            spawner.GetComponent<SpawnPointScript>().SpawnRandomObject();
        }

    }



}
