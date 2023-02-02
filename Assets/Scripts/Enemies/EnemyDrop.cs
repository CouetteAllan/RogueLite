using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] ItemsSO[] itemToDrop;
    [SerializeField] GameObject itemPrefab;

    [Range(0,100)] [SerializeField] float dropChance = 40;
    private ItemsSO itemPicked = null;
    private EnemyEntity3D enemy;
    private void Awake()
    {
        enemy = GetComponent<EnemyEntity3D>();
        enemy.OnDeath += PickItem;
    }

    public void DropItem()
    {
        GameObject gameObject = Instantiate(itemPrefab, this.transform.position, Quaternion.identity);
        gameObject.GetComponent<ItemsScript>().InitItem(itemPicked);
    }

    public void PickItem()
    {
        float percentDropChance = dropChance / 100;

        if (Random.Range(0f, 1f) < percentDropChance)
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
