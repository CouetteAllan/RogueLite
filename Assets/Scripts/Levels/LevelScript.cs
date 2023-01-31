using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChangeCameraScript))]
public class LevelScript : MonoBehaviour, IExit
{
    private bool canExit = false;
    [SerializeField] private bool isStart = false;
    [SerializeField] private GameObject nextRoom;
    private RoomSpawnHandle nextRoomScript;
    private GameObject currentRoom;
    private RoomSpawnHandle currentRoomScript;
    [SerializeField] private GameObject nextPosToGo;
    [SerializeField] private GameObject nextVirtualCamera;
    [SerializeField] private ChangeCameraScript changeCameraScript;

    private void Awake()
    {
        changeCameraScript = GetComponent<ChangeCameraScript>();
        nextRoomScript = nextRoom.GetComponent<RoomSpawnHandle>();

        currentRoom = this.transform.parent.gameObject;
        currentRoomScript = currentRoom.GetComponent<RoomSpawnHandle>();
    }

    public void OnExit(MainCharacterScript player)
    {
        if (!canExit && !isStart)
            return;
        player.GetRigidbody2D().position = nextPosToGo.transform.position;
        changeCameraScript.ChangeCamera(nextVirtualCamera);
        nextRoomScript.SpawnEnemy?.Invoke();
    }

    private void Start()
    {
        if (isStart)
            return;
        currentRoomScript.NoMoreEnemies += CanExit;
    }

    private void CanExit()
    {
        canExit = true;
    }

    private void OnDisable()
    {
        currentRoomScript.NoMoreEnemies -= CanExit;

    }
}
