using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float movementSpeed;

    [SerializeField] protected float pushForce = 10f;

    protected bool isDead = false;


    protected Rigidbody2D rb2D;
    [SerializeField] protected Animator animator;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    public virtual void ChangeHealth(float _value, Entity sender = null)
    {
        if (sender != null && _value < 0)
        {
            Vector2 pushDirection = sender.rb2D.position - this.rb2D.position;
            this.rb2D.AddForce(pushDirection.normalized * pushForce * 10, ForceMode2D.Impulse);
        }

        health += _value;
        if (health <= 0)
            this.Die();
    }

    public virtual void Die()
    {
        isDead = true;
        Destroy(this.gameObject);
    }
}
