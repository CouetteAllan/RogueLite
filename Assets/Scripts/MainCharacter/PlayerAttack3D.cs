using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerAttack3D : MonoBehaviour, IHitSource3D
{
    public event Action OnCritLanded;

    private bool isUsingGamePad;
    [SerializeField] GameObject hand;

    private Camera _cam;
    private Weapons actualWeapon;
    private float attackSpeed = 1f;
    private PlayerInput playerInput;
    private MainCharacterScript3D player;
    private Animator animator;
    [SerializeField] LayerMask layerAttack;
    [SerializeField] LayerMask layerClick;

    public Vector3 range;
    public Vector3 offset;

    private Rigidbody rb;
    public GameObject hitbox;


    public bool isAttacking = false;
    public Vector2 MouseAimDir { get; set; }
    private Vector2 lastInput;
    public Vector2 LastInput { set => lastInput = value; }
    private Vector3 lastInput3D => new Vector3(lastInput.x, 0, lastInput.y);

    [SerializeField] private float pushForceForward = 6f;

    public delegate void Attack();
    public Attack attackHandle;
    public Attack aimingHandle;


    public Rigidbody SourceRigidbody => this.rb;
    public float Damage => player.GetPlayerStats().GetStat(StatType.Damage).Value;

    public bool IsDead => false;

    private Coroutine attackCoroutine;
    private delegate void AttackType();
    private AttackType attackType;

    private List<IHittable3D> enemiesThatBeenHit = new List<IHittable3D>();


    private void OnDisable()
    {
        InputManager.playerInputAction.Player.Attack.performed -= OnAttack;
    }
    void Awake()
    {
        InputManager.playerInputAction.Player.Attack.performed += OnAttack;
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.onControlsChanged += PlayerInput_onControlsChanged;

    }

    private void Start()
    {
        _cam = Camera.main;
        if (isUsingGamePad)
        {
            attackHandle = GamePadAttackHandle;
            aimingHandle = GamePadAimingHandle;

        }
        else
        {
            attackHandle = MouseAttackHandle;
            aimingHandle = MouseAimingHandle;

        }
        player = GetComponent<MainCharacterScript3D>();
        animator = this.GetComponent<Animator>();
        animator.SetFloat("AttackSpeedModifier", attackSpeed);
    }

    private void PlayerInput_onControlsChanged(PlayerInput obj)
    {
        if (obj.currentControlScheme == "Gamepad")
        {
            attackHandle = GamePadAttackHandle;
            aimingHandle = GamePadAimingHandle;
            isUsingGamePad = true;
        }
        else
        {
            attackHandle = MouseAttackHandle;
            aimingHandle = MouseAimingHandle;
            isUsingGamePad = false;
        }


    }

    public void SetAttackSpeed(float _attackspeed)
    {
        attackSpeed = _attackspeed;
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isAttacking)
            return;
        //Animation d'attaque
        animator.SetTrigger("Attack");
        rb.velocity = rb.velocity / 2f;
        isAttacking = true;
        player.CanMove = false;
        //la visée est choisit en fonction de la manette ou bien de la souris
        aimingHandle();
    }

    public void ActivateHitBox()
    {
        if (actualWeapon == null)
            return;
        attackHandle();
    }

    private Vector3 GetMousePosInTheWorld()
    {
        var ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, 100000f, layerClick);
        return hitInfo.point;
    }
    private void MouseAimingHandle()
    {
        var mousePos = GetMousePosInTheWorld();
        var mouseAimDir = mousePos - rb.position;
        mouseAimDir.y = 0;
        if (mouseAimDir.normalized.x > 0.01f && player.isFlipped == true)
        {
            player.Flip();
        }
        else if (mouseAimDir.normalized.x < -0.01f && player.isFlipped == false)
        {
            player.Flip();
        }
    }

    private void MouseAttackHandle()
    {
        var mousePos = GetMousePosInTheWorld();
        var mouseAimDir = mousePos - rb.position;
        mouseAimDir.y = 0;
        rb.AddForce(mouseAimDir.normalized * pushForceForward, ForceMode.Impulse);
        hitbox.SetActive(true);
        offset = rb.position + mouseAimDir.normalized;

        attackType();
    }

    private void GamePadAimingHandle()
    {
        Collider[] enemiesInFront = Physics.OverlapSphere(rb.position + lastInput3D.normalized * 1.7f, 2.6f, layerAttack);
        Vector3 nearestDir = lastInput3D.normalized;
        Vector3 nearestPos;
        float minDistSquared = Mathf.Infinity;
        Vector3 currentPos = this.rb.position;

        foreach (var enemy in enemiesInFront)
        {
            Rigidbody rbEnemy = enemy.GetComponent<Rigidbody>();

            Vector3 directionToTarget = rbEnemy.position - currentPos;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < minDistSquared)
            {
                minDistSquared = dSqrToTarget;
                nearestPos = rbEnemy.position;
                nearestDir = nearestPos - currentPos;
            }

        }
        lastInput = new Vector2(nearestDir.x, nearestDir.z).normalized;
    }

    private void GamePadAttackHandle()
    {
        rb.AddForce(lastInput3D.normalized * pushForceForward, ForceMode.Impulse);
        hitbox.SetActive(true);

        offset = rb.position + lastInput3D.normalized ;

        attackType();
        
    }

    private void MeleeAttack()
    {
        if (enemiesThatBeenHit != null)
        {
            foreach (IHittable3D enemy in enemiesThatBeenHit)
            {
                enemy.GotHit = false;
            }
        }
        enemiesThatBeenHit.Clear();
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(ActiveFrameAttack(isUsingGamePad));
        hitbox.transform.position = offset;
        hitbox.transform.localScale = new Vector3((actualWeapon.range / 20) * 1.7f, 0.5f, (actualWeapon.range / 20) * 1.7f);
    }

    IEnumerator ActiveFrameAttack(bool usingGamePad)
    {
        var mousePos = GetMousePosInTheWorld();
        var mouseAimDir = mousePos - rb.position;
        mouseAimDir.y = 0;

        float damageOutput = -player.GetPlayerStats().GetStat(StatType.Damage).Value;
        bool critStrike = UnityEngine.Random.Range(0f, 1f) <= player.GetPlayerStats().GetStat(StatType.CritChance).Value;
        if (critStrike)
        {
            Debug.Log("coup critique !");
            damageOutput *= player.GetPlayerStats().GetStat(StatType.CritMultiplier).Value;
        }

        Debug.Log(usingGamePad);
        while (isAttacking)
        {
            if(usingGamePad)
                offset = this.rb.position + lastInput3D.normalized;
            else
                offset = rb.position + mouseAimDir.normalized;

            hitbox.transform.position = offset;
            Collider[] enemiesHit = Physics.OverlapSphere(offset, actualWeapon.range / 20, layerAttack);
            if(enemiesHit != null)
            {
                foreach (var enemy in enemiesHit)
                {
                    IHittable3D hitObject = enemy.GetComponent<IHittable3D>();
                    hitObject.OnHit(damageOutput, this);
                    if (critStrike)
                        OnCritLanded?.Invoke();
                    if (!hitObject.GotHit)
                    {
                        enemiesThatBeenHit.Add(hitObject);
                        hitObject.GotHit = true;
                    }

                }
            }
            
            yield return new WaitForSeconds(0.015f);
        }

        yield break;

    }

    private void RangedAttack()
    {
        hitbox.transform.position = new Vector3(offset.x, offset.y, 0);
        GameObject projectile = Instantiate(actualWeapon.projectile, this.rb.position, Quaternion.identity);
        projectile.layer = 6;
        projectile.GetComponent<ProjectileScript>().GotHit = true;
        //projectile.GetComponent<ProjectileScript>().SetProjectile(offset - rb2D.position, 12f, actualWeapon.damage,player);
    }

    public void DeActivateHitBox()
    {
        //desactive la hitbox en fin d'animation
        hitbox.SetActive(false);
        isAttacking = false;
        player.CanMove = true;
    }

    public void SetActualWeapon(Weapons weapon)
    {
        this.actualWeapon = weapon;
        this.attackSpeed = weapon.attackRate;
        switch (actualWeapon.weaponType)
        {
            case Weapons.WeaponType.Melee:
                attackType = MeleeAttack;
                break;
            case Weapons.WeaponType.Ranged:
                attackType = RangedAttack;
                break;
        }
        hand.GetComponent<SpriteRenderer>().sprite = actualWeapon.sprite;
    }


}
