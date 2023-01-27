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


    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        rb2D.velocity = direction * speed;
    }

    void IHittable.OnHit(float _value, IHitSource source)
    {
            Destroy(this.gameObject);   
    }

    public void SetDirectionAndSpeed(Vector2 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IHittable>(out IHittable hitObject))
        {
            hitObject.OnHit(Damage, this);
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
