using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour, IHittable
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float damage;
    [SerializeField] protected float movementSpeed;

    [Space]
    [Tooltip("Force appliquée lorsque l'entitée est frappée")]
    [SerializeField] protected float pushForce = 10f;
    [SerializeField] protected GameObject graphObject;

    protected bool isDead = false;


    protected Rigidbody2D rb2D;
    protected Animator animator;

    public delegate void DeathEvent();
    public DeathEvent OnDeath;

    public delegate void OnHitEvent();
    public OnHitEvent EventOnHit;
    protected bool isInvincible = false;
    public bool GotHit { get => isInvincible; set => isInvincible = value; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    public virtual void ChangeHealth(float _value, IHitSource source = null)
    {
        if (isDead)
            return;
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
        OnDeath?.Invoke();
        StopAllCoroutines();
        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        this.animator.enabled = false;
        Quaternion targetRotation = Quaternion.Euler(0, 0, -90);
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            graphObject.transform.rotation = Quaternion.Lerp(graphObject.transform.rotation, targetRotation,  Time.deltaTime * 2f );

            yield return null;
        }
        graphObject.transform.rotation = targetRotation;
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }

    public virtual void OnHit(float _value, IHitSource source)
    {
        if (isDead || isInvincible)
            return;
        ChangeHealth(_value,source);
        EventOnHit?.Invoke();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
