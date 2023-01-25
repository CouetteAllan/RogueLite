using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    public Weapons weapon;
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = weapon.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<MainCharacterScript>(out MainCharacterScript player))
        {
            player.PickUpWeapon(this);
            Destroy(this.gameObject);
        }
    }
}
