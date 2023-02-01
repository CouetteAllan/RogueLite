using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack3D : MonoBehaviour, IHitSource3D
{
    private bool isUsingGamePad;
    [SerializeField] GameObject hand;

    private Weapons actualWeapon;
    private float attackSpeed = 1f;
    private PlayerInput playerInput;
    private MainCharacterScript3D player;
    [SerializeField] LayerMask layerAttack;

    public Vector3 range;
    public Vector3 offset;

    private Rigidbody rb;
    public GameObject hitbox;


    public bool isAttacking = false;
    public Vector2 MouseAimDir { get; set; }
    private Vector2 lastInput;
    private Vector3 lastInput3D => new Vector3(lastInput.x, 0, lastInput.y);
    public Vector2 LastInput { get; set; }

    [SerializeField] private float pushForceForward = 6f;

    public delegate void Attack();
    public Attack attackHandle;
    public Attack aimingHandle;


    public Rigidbody SourceRigidbody => this.rb;
    public float Damage => actualWeapon.damage;

    public bool IsDead => false;

    private Coroutine attackCoroutine;
    private delegate void AttackType();
    private AttackType attackType;

    private List<IHittable3D> enemiesThatBeenHit = new List<IHittable3D>();
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        player = GetComponent<MainCharacterScript3D>();
    }

    private void Start()
    {
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
        playerInput = GetComponent<PlayerInput>();

        playerInput.onControlsChanged += PlayerInput_onControlsChanged;

        player.GetAnimator().SetFloat("AttackSpeedModifier", attackSpeed);
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

    public void SetAttackSpeed(float _attackspeed)
    {
        attackSpeed = _attackspeed;
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isAttacking)
            return;
        //Animation d'attaque
        player.GetAnimator().SetTrigger("Attack");
        rb.velocity = Vector2.zero;
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
        rb.AddForce(MouseAimDir.normalized * pushForceForward, ForceMode.Impulse);
        hitbox.SetActive(true);
        //offset = rb.position + MouseAimDir.normalized;

        attackType();
    }

    private void GamePadAimingHandle()
    {
        Collider[] enemiesInFront = Physics.OverlapSphere(rb.position + new Vector3(LastInput.x,0,LastInput.y).normalized * 1.7f, 2.6f, layerAttack);
        Vector3 nearestDir = new Vector3(LastInput.x, 0, LastInput.y);
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
            foreach (IHittable enemy in enemiesThatBeenHit)
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
        Vector2 mouseAim = MouseAimDir;
        
        while (isAttacking)
        {
            if(usingGamePad)
                offset = this.rb.position + lastInput3D.normalized ;
            else
                //offset = rb.position + mouseAim.normalized;

            hitbox.transform.position = offset;
            Collider[] enemiesHit = Physics.OverlapSphere(offset, actualWeapon.range / 20, layerAttack);
            if(enemiesHit != null)
            {
                foreach (var enemy in enemiesHit)
                {
                    IHittable3D hitObject = enemy.GetComponent<IHittable3D>();
                    hitObject.OnHit(-actualWeapon.damage, this);
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


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(offset, actualWeapon.range / 20);
    }
}
