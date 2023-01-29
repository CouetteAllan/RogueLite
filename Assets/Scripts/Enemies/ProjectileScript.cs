using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour,IHitSource, IHittable
{
    private Rigidbody2D rb2D;
    [SerializeField] float projectileDamage;
    private Vector2 direction;
    private float speed;
    public Rigidbody2D SourceRigidbody2D => rb2D;

    public float Damage => projectileDamage;

    public bool IsInvincible { get => false; set => throw null; }

    public bool IsDead => false;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        rb2D.velocity = direction * speed;
        StartCoroutine(Lifespan(3f));
    }

    void IHittable.OnHit(float _value, IHitSource source)
    {
            Destroy(this.gameObject);   
    }

    public void SetProjectile(Vector2 direction, float speed, float damage)
    {
        this.direction = direction;
        this.speed = speed;
        this.projectileDamage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IHittable>(out IHittable hitObject))
        {
            hitObject.OnHit(-Damage, this);
            Destroy(this.gameObject);
        }
    }

    IEnumerator Lifespan(float lifeSpan)
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
