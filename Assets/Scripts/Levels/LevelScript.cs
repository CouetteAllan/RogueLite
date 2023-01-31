using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChangeCameraScript))]
public class LevelScript : MonoBehaviour, IExit
{
    private bool canExit = false;
    [SerializeField] public GameObject nextRoom;
    [SerializeField] private GameObject nextPosToGo;
    [SerializeField] private GameObject nextVirtualCamera;
    [SerializeField] private ChangeCameraScript changeCameraScript;

    private void Awake()
    {
        changeCameraScript = GetComponent<ChangeCameraScript>();
    }

    public void OnExit(MainCharacterScript player)
    {
        if (!canExit)
            return;
        player.GetRigidbody2D().position = nextPosToGo.transform.position;
        changeCameraScript.ChangeCamera(nextVirtualCamera);
    }

    private void Start()
    {
        SpawnManager.Instance.NoMoreEnemies += CanExit;
    }

    private void CanExit()
    {
        canExit = true;
    }
}
