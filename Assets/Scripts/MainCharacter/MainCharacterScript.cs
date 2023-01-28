using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacterScript : Entity
{
    [SerializeField] private float invincibleTime = 2f;
    [SerializeField] private float dashInvincibleTime = 1f;
    [SerializeField] private float dashForce = 1f;
    [SerializeField] private TrailRenderer trail;

    [SerializeField] private Vector2 input;
    private Vector2 lastInput;

    public PlayerInputAction playerInputAction;
    private PlayerInput playerInput;
    [HideInInspector] public Vector2 mouseAim;

    [HideInInspector] public bool isFlipped = false;
    private float time = 0f;

    private PlayerAttack playerAttackScript;
    private Weapons weaponPickedUpData;

    private bool isMoving => input != Vector2.zero;

    public bool startWithWeapon = false;
    [HideInInspector] public Weapons weapon;

    private bool canMove = true;

    private Coroutine invincibleCoroutine;
    public delegate void PlayerChangeHealth(float value);
    public PlayerChangeHealth OnPlayerChangeHealth;



    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
        playerAttackScript = GetComponent<PlayerAttack>();

        playerInputAction.Player.Enable();
        playerInputAction.Player.Attack.performed += playerAttackScript.OnAttack;
        playerInputAction.Player.MouseAim.performed += OnMouseChangePos;
        playerInputAction.Player.Dash.performed += Dash;
    }



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerInput = GetComponent<PlayerInput>();
        GameManager.Instance.InitPlayer(this);
        if (startWithWeapon)
        {
            PickUpWeapon(weapon);
        }
        trail.emitting = false;
        health = maxHealth;
        UIManager.Instance.SetMaxHealth(maxHealth,this);
        UIManager.Instance.SetUIHealth(maxHealth);
    }



    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            input = playerInputAction.Player.Move.ReadValue<Vector2>();
            if (Mathf.Abs(input.x) > 0.009f || Mathf.Abs(input.y) > 0.009f)
            {
                lastInput = input;

            }
        }
        if (Mathf.Abs(input.x) > 0.03f && Mathf.Abs(input.y) > 0.03f )
            playerAttackScript.LastInput = input;
        if(input.x > 0.01f && isFlipped)
        {
            Flip();
        }
        else if(input.x < -0.01f && !isFlipped)
        {
            Flip();
        }

        animator.SetBool("IsMoving", isMoving);
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        var targetSpeed = movementSpeed * input;
        var speedDiff = targetSpeed - rb2D.velocity;
        var accelRate = new Vector2(Mathf.Abs(targetSpeed.x) > 0.01f ? acceleration.x : decceleration.x, Mathf.Abs(targetSpeed.y) > 0.01f ? acceleration.y : decceleration.y);
        var movement = new Vector2(Mathf.Pow(Mathf.Abs(speedDiff.x) * accelRate.x, velocityPower.x) * Mathf.Sign(speedDiff.x), Mathf.Pow(Mathf.Abs(speedDiff.y) * accelRate.y, velocityPower.y) * Mathf.Sign(speedDiff.y));

        this.rb2D.AddForce(movement,ForceMode2D.Impulse);
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!canMove)
            return;


        //Invincible
        if (invincibleCoroutine != null)
            StopCoroutine(invincibleCoroutine);
        invincibleCoroutine = StartCoroutine(DashInvincibilityHandle(dashInvincibleTime));

        //Va rapidement dans une direction
        this.rb2D.AddForce(lastInput.normalized * dashForce,ForceMode2D.Impulse);
        trail.emitting = true;


        //Ne peut pas input de direction pendant un certains temps
        StartCoroutine(CantMoveTimer(0.3f));
    }

    public void Flip()
    {
        isFlipped = !isFlipped;
        this.transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

    }

    public void PickUpWeapon(Weapons weapon)
    {
        weaponPickedUpData = weapon;
        playerAttackScript.SetActualWeapon(weaponPickedUpData);
        this.damage = weaponPickedUpData.damage;
    }

    private void OnMouseChangePos(InputAction.CallbackContext context)
    {
        mouseAim = context.action.ReadValue<Vector2>();
        Vector2 realMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseAim.x,mouseAim.y,Camera.main.nearClipPlane));
        Vector2 dir = realMousePos - rb2D.position;
        playerAttackScript.MouseAimDir = dir;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public Rigidbody2D GetRigidbody2D()
    {
        return rb2D;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        IHitSource hitSourceObject = collision.GetComponent<IHitSource>();
        if(hitSourceObject != null)
        {
            this.OnHit(-hitSourceObject.Damage, hitSourceObject);
        }
    }

    public override void OnHit(float _value, IHitSource source)
    {
        if (isInvincible)
            return;

        base.OnHit(_value, source);
        OnPlayerChangeHealth(health + _value);
        StartCoroutine(InvincibilityHandle(invincibleTime));
    }

    public override void Die()
    {
        GameManager.Instance.PlayerDeath();
        base.Die();
    }

    private void OnDestroy()
    {
        playerInputAction.Player.Disable();
        playerInputAction.Player.Attack.performed -= playerAttackScript.OnAttack;
        playerInputAction.Player.MouseAim.performed -= OnMouseChangePos;
    }

    IEnumerator InvincibilityHandle(float time)
    {
        isInvincible = true;
        Physics2D.IgnoreLayerCollision(6, 8, true);
        Physics2D.IgnoreLayerCollision(6, 9, true);
        Debug.Log("La collision entre le player et le projectile est ignor�e: " + Physics2D.GetIgnoreLayerCollision(6, 9));
        SpriteRenderer sprite = graphObject.GetComponent<SpriteRenderer>();

        sprite.color = new Color(1f, 1f, 1f, 0.4f);
        yield return new WaitForSeconds(time);
        Physics2D.IgnoreLayerCollision(6, 8, false);
        Physics2D.IgnoreLayerCollision(6, 9, false);
        sprite.color = new Color(1f, 1f, 1f, 1f);
        isInvincible = false;
        yield break;
    }
    
    IEnumerator DashInvincibilityHandle(float time)
    {
        isInvincible = true;

        Physics2D.IgnoreLayerCollision(6, 8, true);
        Physics2D.IgnoreLayerCollision(6, 9, true);
        SpriteRenderer sprite = graphObject.GetComponent<SpriteRenderer>();

        sprite.color = new Color(1f, 1f, 1f, 0.4f);
        yield return new WaitForSeconds(time);
        Physics2D.IgnoreLayerCollision(6, 8, false);
        Physics2D.IgnoreLayerCollision(6, 9, false);
        sprite.color = new Color(1f, 1f, 1f, 1f);
        isInvincible = false;

        yield break;
    }

    IEnumerator CantMoveTimer(float timer)
    {
        canMove = false;
        yield return new WaitForSeconds(timer);
        canMove = true;
        trail.emitting = false;


    }

}
