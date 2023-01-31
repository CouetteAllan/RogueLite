using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject virtualCam;

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<MainCharacterScript>(out MainCharacterScript player) && !other.isTrigger)
        {
            virtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<MainCharacterScript>(out MainCharacterScript player) && !other.isTrigger)
        {
            virtualCam.SetActive(false);
        }
    }*/

    public void ChangeCamera(GameObject _nextVirtualCamera)
    {
        virtualCam.SetActive(false);
        _nextVirtualCamera.SetActive(true);
    }
}
