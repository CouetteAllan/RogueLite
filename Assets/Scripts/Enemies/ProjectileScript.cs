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
    private bool gotHit = false;
    public bool GotHit { get => false; set => gotHit = value; }
    private bool isFromPlayer;
    public bool IsDead => false;
    Coroutine coroutine;

    [SerializeField] private LayerMask layer;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        rb2D.velocity = direction * speed;
        coroutine = StartCoroutine(Lifespan(3f));
        
    }

    void IHittable.OnHit(float _value, IHitSource source)
    {
        Vector2 newDir = -direction;
        rb2D.velocity = newDir * speed * 1.2f;
        StopCoroutine(coroutine);
        StartCoroutine(Lifespan(2f));
        gotHit = true;
    }

    public void SetProjectile(Vector2 direction, float speed, float damage, Entity sender = null)
    {
        this.direction = direction;
        this.speed = speed;
        this.projectileDamage = damage;
        if(sender != null)
        {
            isFromPlayer = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<MainCharacterScript>(out MainCharacterScript hitObject))
        {
            hitObject.OnHit(-Damage, this);
            Destroy(this.gameObject);
        }
        if (gotHit)
        {
            if (collision.TryGetComponent<EnemyEntity>(out EnemyEntity enemy))
            {
                if (isFromPlayer)
                {
                    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(rb2D.position, 2f, layer);
                    Debug.Log(hitObjects.Length);
                    foreach (var hit in hitObjects)
                    {
                        hit.GetComponent<IHittable>().OnHit(-Damage, this);
                    }
                }
                else
                    enemy.OnHit(-Damage, this);
                Destroy(this.gameObject);
            }
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
