using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float maxSpeed;

    protected bool isDead = false;


    protected Rigidbody2D rb2D;
    protected Animator animator;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.rb2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
    }

    // Update is called once per frame

    protected void ChangeHealth(float _value)
    {
        health += _value;
        if (health <= 0)
            this.Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        Destroy(this.gameObject);
    }
}
