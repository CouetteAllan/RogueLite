using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(ChangeCameraScript))]
public class LevelScript : MonoBehaviour, IExit
{
    private bool canExit = false;
    [SerializeField] private bool isStart = false;
    private RoomSpawnHandle nextRoomScript;
    private GameObject currentRoom;
    private RoomSpawnHandle currentRoomScript;
    [SerializeField] private GameObject nextPosToGo;
    [SerializeField] private GameObject previousPosToGo;
    [SerializeField] private GameObject nextVirtualCamera;
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private ChangeCameraScript changeCameraScript;

    private void Awake()
    {
        changeCameraScript = GetComponent<ChangeCameraScript>();

        virtualCamera = nextVirtualCamera.GetComponent<CinemachineVirtualCamera>();
        currentRoom = this.transform.parent.gameObject;
        currentRoomScript = currentRoom.GetComponent<RoomSpawnHandle>();
    }

    public void OnExit(MainCharacterScript3D player)
    {
        if (!canExit && !isStart)
            return;
        player.GetRigidbody2D().position = nextPosToGo.transform.position;
        changeCameraScript.ChangeCamera(nextVirtualCamera);
        if(virtualCamera.Follow == null)
        {
            virtualCamera.Follow = player.gameObject.transform;
        }
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

    public void SetNextRoom(GameObject nextRoom, GameObject _nextPosToGo, GameObject _nextVirtualCamera)
    {
        this.nextRoomScript = nextRoom.GetComponent<RoomSpawnHandle>();
        nextPosToGo = _nextPosToGo;
        nextVirtualCamera = _nextVirtualCamera;
        virtualCamera = nextVirtualCamera.GetComponent<CinemachineVirtualCamera>();
    }

    public GameObject GetPreviousTp()
    {
        return previousPosToGo;
    }
}
