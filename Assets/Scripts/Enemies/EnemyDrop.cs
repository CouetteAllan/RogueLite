using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] ItemsSO[] itemToDrop;
    [SerializeField] GameObject itemPrefab;
    private ItemsSO itemPicked = null;
    private EnemyEntity enemy;
    private void Awake()
    {
        enemy = GetComponent<EnemyEntity>();
        enemy.OnDeath += PickItem;
    }

    public void DropItem()
    {
        GameObject gameObject = Instantiate(itemPrefab, this.transform.position, Quaternion.identity);
        gameObject.GetComponent<ItemsScript>().InitItem(itemPicked);
    }

    public void PickItem()
    {
        if (Random.Range(0f, 1f) < 0.5f)
        {
            itemPicked = itemToDrop[Random.Range(0, itemToDrop.Length)];
            DropItem();
        }
        else
            Debug.Log("Pas de chance");
    }

    private void OnDestroy()
    {
        enemy.OnDeath -= PickItem;
    }
}
