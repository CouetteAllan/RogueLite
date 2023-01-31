using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnHandle : MonoBehaviour
{
    public delegate void RoomSpawnEvent();
    public RoomSpawnEvent SpawnEnemy;
    public RoomSpawnEvent NoMoreEnemies;
    public RoomSpawnEvent NoMoreWaves;
    [SerializeField] private GameObject blockingPaths;

    private int enemyNbInRoom = 0;
    private int nbWaves = 0;
    bool noWave = false;

    private SpawnPointScript[] spawns;

    private void Awake()
    {
        SetUpSpawners();
    }

    public void AddEnemy()
    {
        enemyNbInRoom++;
        if (!blockingPaths.activeInHierarchy)
            blockingPaths.SetActive(true);
    }

    public void SubstractEnemy()
    {
        enemyNbInRoom--;
        if (enemyNbInRoom <= 0)
        {
            NoMoreEnemies?.Invoke();
            Debug.Log("Je sui sappelé");
            if (noWave)
            {
                NoMoreWaves?.Invoke();
                blockingPaths.SetActive(false);

            }
        }
    }

    public void NoMoreWavesLeft()
    {
        noWave = true;
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
