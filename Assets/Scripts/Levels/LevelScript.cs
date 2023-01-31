using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour, IExit
{
    private bool canExit = false;
    [SerializeField] public GameObject nextRoom;
    [SerializeField] private GameObject nextPosToGo;

    public void OnExit(MainCharacterScript player)
    {
        if (!canExit)
            return;
        player.GetRigidbody2D().MovePosition(nextPosToGo.transform.position);
    }
}
