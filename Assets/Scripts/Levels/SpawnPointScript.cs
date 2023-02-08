using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    [SerializeField] GameObject[] objectsThatCanSpawn;
    [Range(0, 100)] [SerializeField] float chanceToSpawn;
    [SerializeField] private EnemySO[] enemy_SOs;
    private RoomSpawnHandle actualRoom;
    private int nbOfWaves = 3;

    public void SpawnRandomObject()
    {
        float chance = chanceToSpawn / 100;
        if(Random.Range(0f,1f) < chance)
        {
            StartCoroutine(DelaySpawn(2f));
        }
        if (nbOfWaves <= 0)
            Destroy(this.gameObject);
        nbOfWaves--;
    }

    private void Spawn()
    {
        GameObject instantiateGO = Instantiate(objectsThatCanSpawn[/*Random.Range(0, objectsThatCanSpawn.Length)*/0], this.transform.position, Quaternion.identity);
        if (instantiateGO.TryGetComponent<EnemyEntity3D>(out EnemyEntity3D enemyEntity))
        {
            enemyEntity.enemyData = enemy_SOs[Random.Range(0, 2)];
            enemyEntity.StartEnemy(actualRoom);
        }
        
    }

    private void OnDestroy()
    {
        actualRoom.SpawnEnemy -= SpawnRandomObject;
        actualRoom.NoMoreEnemies -= SpawnRandomObject;
        actualRoom.NoMoreWavesLeft();
    }

    public void SetActualRoom(RoomSpawnHandle room)
    {
        actualRoom = room;
        actualRoom.SpawnEnemy += SpawnRandomObject;
        if(nbOfWaves > 1)
            actualRoom.NoMoreEnemies += SpawnRandomObject;
    }

    IEnumerator DelaySpawn(float delay)
    {
        actualRoom.AddEnemy();
        yield return new WaitForSeconds(delay);
        Spawn();
        yield break;
    }

}
