using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    [SerializeField] GameObject[] objectsThatCanSpawn;
    [Range(0, 100)] [SerializeField] float chanceToSpawn;
    [SerializeField] private EnemySO[] enemy_SOs;
    private RoomSpawnHandle actualRoom;
    private int nbOfWaves = 2;

    public void SpawnRandomObject()
    {
        float chance = chanceToSpawn / 100;
        if(Random.Range(0f,1f) < chance)
        {
            GameObject instantiateGO = Instantiate(objectsThatCanSpawn[/*Random.Range(0, objectsThatCanSpawn.Length)*/0],this.transform.position,Quaternion.identity);
            if (instantiateGO.TryGetComponent<EnemyEntity>(out EnemyEntity enemyEntity))
            {
                actualRoom.AddEnemy();
                enemyEntity.enemyData = enemy_SOs[Random.Range(0, 2)];
                enemyEntity.StartEnemy(actualRoom);
            }
            
        }
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        actualRoom.SpawnEnemy -= SpawnRandomObject;

    }

    public void SetActualRoom(RoomSpawnHandle room)
    {
        actualRoom = room;
        actualRoom.SpawnEnemy += SpawnRandomObject;
    }

}
