using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnHandle : MonoBehaviour
{
    public delegate void RoomSpawnEvent();
    public RoomSpawnEvent SpawnEnemy;
    public RoomSpawnEvent NoMoreEnemies;

    private int enemyNbInRoom = 0;

    private SpawnPointScript[] spawns;

    private void Awake()
    {
        SetUpSpawners();
    }

    public void AddEnemy()
    {
        enemyNbInRoom++;
    }

    public void SubstractEnemy()
    {
        enemyNbInRoom--;
        if (enemyNbInRoom <= 0)
        {
            NoMoreEnemies?.Invoke();
        }
    }

    public void SetUpSpawners()
    {
        GameObject spawnerParent = null;
        spawnerParent = transform.Find("Spawners").gameObject;

        spawns = spawnerParent.transform.GetComponentsInChildren<SpawnPointScript>();
        foreach (var s in spawns)
        {
            s.SetActualRoom(this);
        }
    }

}
