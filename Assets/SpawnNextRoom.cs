using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNextRoom : MonoBehaviour
{
    [SerializeField] private List<GameObject> rooms = new List<GameObject>();
    public GameObject previousRoom;
    [SerializeField] public GameObject entryTp;
    [SerializeField] private GameObject actualCamera;

    [SerializeField] private GameObject[] posToInstatianteRooms;
    [SerializeField] private LevelScript[] exit;
    [SerializeField] private LevelScript exitTowardPreviousRoom;


    private void Awake()
    {
        SpawnRooms();
    }

    public void SpawnRooms()
    {
        if (rooms == null)
            return;
        for (int i = 0; i < rooms.Count; i++)
        {
            GameObject room = Instantiate(rooms[i], posToInstatianteRooms[i].transform.position,Quaternion.identity);
            SpawnNextRoom roomScript = room.GetComponent<SpawnNextRoom>();

            exit[i].SetNextRoom(room, roomScript.entryTp, roomScript.GetNextCamera());
            Debug.Log("succesfully spawn level" + room);
            roomScript.previousRoom = this.gameObject;
            roomScript.GetExitTowardPreviousRoom().SetNextRoom(this.gameObject, this.exit[i].GetPreviousTp()  , this.actualCamera);
            
        }
    }

    public GameObject GetNextCamera()
    {
        return actualCamera;
    }

    public LevelScript GetExitTowardPreviousRoom()
    {
        return this.exitTowardPreviousRoom;
    }
}
