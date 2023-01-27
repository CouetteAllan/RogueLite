using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour, IHitSource
{
    [HideInInspector] private bool isUsingGamePad;

    private Weapons actualWeapon;
    private float attackSpeed = 1f;
    private PlayerInput playerInput;
    private MainCharacterScript player;
    [SerializeField] GameObject hand;
    [SerializeField] LayerMask layerAttack;

    public Vector2 range;
    public Vector2 offset;

    private Rigidbody2D rb2D;
    public GameObject hitbox;


    public bool isAttacking = false;
    public Vector2 MouseAimDir { get; set; }
    public Vector2 LastInput { get; set; }

    [SerializeField] private float pushForceForward = 6f;
    private Vector2 HandPosition => hand.transform.position;

    public delegate void Attack();
    public Attack attackHandle;
    public Attack aimingHandle;


    public Rigidbody2D SourceRigidbody2D => this.rb2D;
    public float Damage => actualWeapon.damage;
    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        
        player = GetComponent<MainCharacterScript>();
    }

    private void Start()
    {
        if (isUsingGamePad)
            attackHandle = GamePadAttackHandle;
        else
            attackHandle = MouseAttackHandle;
        playerInput = GetComponent<PlayerInput>();

        playerInput.onControlsChanged += PlayerInput_onControlsChanged;
    }

    private void PlayerInput_onControlsChanged(PlayerInput obj)
    {
        if (obj.currentControlScheme == "Gamepad")
        {
            attackHandle = GamePadAttackHandle;
            aimingHandle = GamePadAimingHandle;
        }
        else
        {
            attackHandle = MouseAttackHandle;
            aimingHandle = MouseAimingHandle;
        }


    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isAttacking)
            return;
        //Animation d'attaque
        player.GetAnimator().SetTrigger("Attack");
        rb2D.velocity = Vector2.zero;
        isAttacking = true;
        //la visée est choisit en fonction de la manette ou bien de la souris
        aimingHandle();
    }

    public void ActivateHitBox()
    {
        if (actualWeapon == null)
            return;
        attackHandle();
    }


    private void MouseAimingHandle()
    {
        if (MouseAimDir.normalized.x > 0.01f && player.isFlipped == true)
        {
            player.Flip();
        }
        else if (MouseAimDir.normalized.x < -0.01f && player.isFlipped == false)
        {
            player.Flip();
        }
    }

    private void MouseAttackHandle()
    {
        rb2D.AddForce(MouseAimDir.normalized * pushForceForward, ForceMode2D.Impulse);
        hitbox.SetActive(true);

        offset = rb2D.position + MouseAimDir.normalized * 1.5f;
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(offset, actualWeapon.range / 20, layerAttack);
        foreach (var enemy in enemiesHit)
        {
            IHittable hitObject = enemy.GetComponent<EnemyEntity>();
            hitObject.OnHit(-actualWeapon.damage, this);
        }

        hitbox.transform.position = new Vector3(offset.x, offset.y, 0);
    }

    private void GamePadAimingHandle()
    {
        Collider2D[] enemiesInFront = Physics2D.OverlapCircleAll(rb2D.position + LastInput.normalized * 1.7f, 2, layerAttack);
        Vector2 nearestDir = LastInput;
        Vector2 nearestPos;
        float minDistSquared = Mathf.Infinity;
        Vector2 currentPos = this.rb2D.position;

        foreach (var enemy in enemiesInFront)
        {
            Rigidbody2D rbEnemy = enemy.GetComponent<Rigidbody2D>();

            Vector3 directionToTarget = rbEnemy.position - currentPos;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < minDistSquared)
            {
                minDistSquared = dSqrToTarget;
                nearestPos = rbEnemy.position;
                nearestDir = nearestPos - currentPos;
            }

        }
        LastInput = nearestDir.normalized;
    }

    private void GamePadAttackHandle()
    {
        rb2D.AddForce(LastInput* pushForceForward, ForceMode2D.Impulse);
        hitbox.SetActive(true);

        offset = rb2D.position + LastInput* 1.5f;
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(offset, actualWeapon.range / 20, layerAttack);
        foreach (var enemy in enemiesHit)
        {
            IHittable hitObject = enemy.GetComponent<EnemyEntity>();
            hitObject.OnHit(-actualWeapon.damage, this);
        }

        hitbox.transform.position = new Vector3(offset.x, offset.y, 0);
    }

    public void DeActivateHitBox()
    {
        //desactive la hitbox en fin d'animation
        hitbox.SetActive(false);
        isAttacking = false;
    }

    public void SetActualWeapon(Weapons weapon)
    {
        this.actualWeapon = weapon;
        hand.GetComponent<SpriteRenderer>().sprite = actualWeapon.sprite;
    }


    /*private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.rb2D.position + LastInput.normalized * 1.7f, 1.5f);
    }*/
}
