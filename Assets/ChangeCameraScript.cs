using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject virtualCam;
    public void ChangeCamera(GameObject _nextVirtualCamera)
    {
        virtualCam.SetActive(false);
        _nextVirtualCamera.SetActive(true);
    }

    public GameObject GetActualCameraRoom()
    {
        return virtualCam;
    }
}
