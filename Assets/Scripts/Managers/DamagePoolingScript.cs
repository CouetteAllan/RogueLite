using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePoolingScript : Singleton<DamagePoolingScript>
{
    [SerializeField] GameObject popUpTextPrefab;

    private List<GameObject> pooledObjects = new List<GameObject>();
    private int amountToPool = 15;

    void Start()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject go = Instantiate(popUpTextPrefab);
            go.SetActive(false);
            pooledObjects.Add(go);
        }
    }

    public GameObject GetPoolObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    public DamagePopUpScript CreatePopUp(int value, Vector3 pos, Color color = new Color())
    {
        //GameObject popUp = Instantiate(popUpTextPrefab, pos, Quaternion.identity);
        GameObject popUp = GetPoolObject();
        if (popUp)
        {
            popUp.SetActive(true);
            popUp.transform.position = pos;
            DamagePopUpScript popUpScript = popUp.GetComponent<DamagePopUpScript>();
            popUpScript.SetUp(value);
            popUpScript.StartAnimation();
            popUpScript.SetColorText(color);
            return popUpScript;
        }

        return null;
    }

}
