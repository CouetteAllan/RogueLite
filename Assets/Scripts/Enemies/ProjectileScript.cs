using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour,IHitSource3D, IHittable3D
{
    private Rigidbody rb;
    [SerializeField] float projectileDamage;
    private Vector3 direction;
    private float speed;
    public Rigidbody SourceRigidbody => rb;

    public float Damage => projectileDamage;
    private bool gotHit = false;
    public bool GotHit { get => false; set => gotHit = value; }
    private bool isFromPlayer;
    public bool IsDead => false;
    Coroutine coroutine;

    [SerializeField] private LayerMask layer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.velocity = direction * speed;

        coroutine = StartCoroutine(Lifespan(3f));
        
    }

    void IHittable3D.OnHit(float _value, IHitSource3D source)
    {
        Vector3 newDir = -direction;
        rb.velocity = newDir * speed * 1.2f;
        StopCoroutine(coroutine);
        StartCoroutine(Lifespan(2f));
        gotHit = true;
    }

    public void SetProjectile(Vector3 direction, float speed, float damage, Entity sender = null)
    {
        this.direction = direction;
        this.speed = speed;
        this.projectileDamage = damage;
        if(sender != null)
        {
            isFromPlayer = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.TryGetComponent<MainCharacterScript3D>(out MainCharacterScript3D hitObject))
        {
            hitObject.OnHit(-Damage, this);
            Destroy(this.gameObject);
        }
        if (gotHit)
        {
            if (collision.TryGetComponent<EnemyEntity3D>(out EnemyEntity3D enemy))
            {
                if (isFromPlayer)
                {
                    Collider[] hitObjects = Physics.OverlapSphere(rb.position, 2f, layer);
                    foreach (var hit in hitObjects)
                    {
                        hit.GetComponent<IHittable3D>().OnHit(-Damage / 2, this);
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
