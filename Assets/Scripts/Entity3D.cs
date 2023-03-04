using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EffectsEnum
{
    Burn,
    Poison
}
public class Entity3D : MonoBehaviour, IHittable3D, IEffectable
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


    protected Rigidbody rb;
    protected Animator animator;

    public delegate void DeathEvent();
    public DeathEvent OnDeath;

    public delegate void OnHitEvent();
    public OnHitEvent EventOnHit;

    public event Action<float> OnChangeHealth;
    public event Action<bool> OnBurn;
    public event Action<bool> OnPoison;

    private EnemyEffectScript effectScript;
    protected bool isInvincible = false;
    public bool GotHit { get => isInvincible; set => isInvincible = value; }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    public virtual void ChangeHealth(float _value, Color color, IHitSource3D source = null)
    {
        if (isDead)
            return;
        if(_value < 0)
        {
            animator.SetTrigger("Hurt");    
            if (source != null)
            {
                Vector2 pushDirection = this.rb.position - source.SourceRigidbody.position;
                this.rb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
            }
        }

        health = Mathf.Clamp(health += _value, 0, maxHealth);

        DamagePoolingScript.Instance.CreatePopUp(Mathf.RoundToInt(_value), this.rb.position + Vector3.up * 0.7f, color);

        if (health <= 0)
            this.Die();

        OnChangeHealth?.Invoke(health);
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

    public virtual void OnHit(float _value, IHitSource3D source)
    {
        if (isDead || isInvincible)
            return;
        ChangeHealth(_value,Color.white,source);
        EventOnHit?.Invoke();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public float GetEntityMaxHealth()
    {
        return this.maxHealth;
    }

    public void SetEntityMaxHealth(float _maxHealth)
    {
        maxHealth = _maxHealth;
    }

    public void CallEffect(EffectsEnum effect, bool display)
    {
        switch (effect)
        {
            case EffectsEnum.Burn:
                OnBurn?.Invoke(display);
                break;
            case EffectsEnum.Poison:
                OnPoison?.Invoke(display);
                break;
        }
    }

    public void AddEffect(IEnchantType effect)
    {
        throw new NotImplementedException();
    }

    public void TickEffect(float duration, IEnchantType effect)
    {
        effect.EnchantEffect(this);
    }
    
}