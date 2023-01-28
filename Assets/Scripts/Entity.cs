using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour, IHittable
{
    [SerializeField] protected Vector2 acceleration;
    [SerializeField] protected Vector2 decceleration;
    [SerializeField] protected Vector2 velocityPower;

    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float damage;
    [SerializeField] protected float movementSpeed;

    [SerializeField] protected float pushForce = 10f;
    [SerializeField] protected GameObject graphObject;

    protected bool isDead = false;


    protected Rigidbody2D rb2D;
    [SerializeField] protected Animator animator;

    public delegate void DeathEvent();
    public DeathEvent OnDeath;

    public delegate void OnHitEvent();
    public OnHitEvent EventOnHit;
    protected bool isInvincible = false;
    public bool IsInvincible { get => isInvincible; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    public virtual void ChangeHealth(float _value, IHitSource source)
    {
        if(_value < 0)
        {
            animator.SetTrigger("Hurt");    
            if (source != null)
            {
                Vector2 pushDirection = this.rb2D.position - source.SourceRigidbody2D.position;
                this.rb2D.AddForce(pushDirection.normalized * pushForce, ForceMode2D.Impulse);
            }
        }

        health = Mathf.Clamp(health += _value, 0, maxHealth);

        if (health <= 0)
            this.Die();
    }

    public virtual void Die()
    {
        isDead = true;
        Destroy(this.gameObject);
        OnDeath?.Invoke();
    }

    public virtual void OnHit(float _value, IHitSource source)
    {
        ChangeHealth(_value,source);
        EventOnHit?.Invoke();
    }
}
