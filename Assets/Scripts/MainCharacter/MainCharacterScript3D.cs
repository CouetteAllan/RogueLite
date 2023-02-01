using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacterScript3D : Entity3D
{
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private AnimationCurve decelerationCurve;
    private float timeCurve = 0f;

    [SerializeField] private float invincibleTime = 2f;
    [SerializeField] private float dashInvincibleTime = 1f;
    [SerializeField] private float dashForce = 1f;
    private bool isDashing = false;
    private bool canDash = true;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private TrailRenderer trail;

    [SerializeField] private Vector2 input;
    private Vector2 lastInput;

    public PlayerInputAction playerInputAction;
    private PlayerInput playerInput;
    [HideInInspector] public Vector2 mouseAim;

    [HideInInspector] public bool isFlipped = false;
    private float time = 0f;

    private PlayerAttack3D playerAttackScript;
    private Weapons weaponPickedUpData;

    private bool isMoving => input != Vector2.zero;

    public bool startWithWeapon = false;
    [SerializeField] private Weapons weapon;

    private bool canMove = true;

    private Coroutine invincibleCoroutine;
    public delegate void PlayerChangeHealth(float value);
    public PlayerChangeHealth OnPlayerChangeHealth;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        playerInputAction = new PlayerInputAction();
        playerAttackScript = GetComponent<PlayerAttack3D>();

        playerInputAction.Player.Enable();
        playerInputAction.Player.Attack.performed += playerAttackScript.OnAttack;
        //playerInputAction.Player.MouseAim.performed += OnMouseChangePos;
        playerInputAction.Player.Dash.performed += Dash;
        playerInputAction.Player.Move.canceled += Move_canceled;
        playerInputAction.Player.Move.started += Move_started;
    }

   





    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //GameManager.Instance.InitPlayer(this);
        if (startWithWeapon)
        {
            PickUpWeapon(weapon);
            playerAttackScript.SetAttackSpeed(weaponPickedUpData.attackRate);
        }
        trail.emitting = false;
        health = maxHealth;
        //UIManager.Instance.SetMaxHealth(maxHealth,this);
        //UIManager.Instance.SetUIHealth(maxHealth);
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
        if (Mathf.Abs(input.x) > 0.009f || Mathf.Abs(input.y) > 0.009f)
            playerAttackScript.LastInput = lastInput;
        if(input.x > 0.01f && isFlipped)
        {
            Flip();
        }
        else if(input.x < -0.01f && !isFlipped)
        {
            Flip();
        }

        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("ZSpeed", lastInput.y);

    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (isDashing)
            return;

        Vector3 targetMovement = new Vector3(input.x, 0, input.y).normalized;

        AnimationCurve speedcurve = isMoving ? accelerationCurve : decelerationCurve;

        rb.velocity = targetMovement * movementSpeed * speedcurve.Evaluate(timeCurve);
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        timeCurve = 0f;
    }

    private void Move_started(InputAction.CallbackContext context)
    {
        timeCurve = 0f;
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!canMove || !canDash)
            return;

        StartCoroutine(DashCoroutine());
        
    }

    IEnumerator DashCoroutine()
    {
        if (invincibleCoroutine != null)
            StopCoroutine(invincibleCoroutine);
        invincibleCoroutine = StartCoroutine(DashInvincibilityHandle(dashInvincibleTime));
        //un delay de 6frames (je devrai peut-être adapter en seconde)
        yield return new WaitForSeconds(0.07f);


        isDashing = true;
        //Va rapidement dans une direction
        this.rb.velocity = Vector3.zero;
        this.rb.AddForce(new Vector3(lastInput.x,0,lastInput.y) * dashForce, ForceMode.Impulse);
        trail.emitting = true;


        //Ne peut pas input de direction pendant un certains temps
        StartCoroutine(CantMoveTimer(0.4f));
    }

    IEnumerator DashCooldownTimer(float timer)
    {
        canDash = false;
        yield return new WaitForSeconds(timer);
        canDash = true;
        yield break;
    }

    public void Flip()
    {
        isFlipped = !isFlipped;
        Vector3 currentScale = animator.gameObject.transform.localScale;
        currentScale.x *= -1;
        animator.gameObject.transform.localScale = currentScale;
        

    }

    public void PickUpWeapon(Weapons weapon)
    {
        weaponPickedUpData = weapon;
        playerAttackScript.SetActualWeapon(weaponPickedUpData);
        this.damage = weaponPickedUpData.damage;
    }

    /*private void OnMouseChangePos(InputAction.CallbackContext context)
    {
        mouseAim = context.action.ReadValue<Vector2>();
        Vector2 realMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseAim.x,mouseAim.y,Camera.main.nearClipPlane));
        Vector2 dir = realMousePos - rb.position;
        playerAttackScript.MouseAimDir = dir;
    }*/

    public Animator GetAnimator()
    {
        return animator;
    }

    public Rigidbody GetRigidbody2D()
    {
        return rb;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        IPickable3D pickable = collision.GetComponent<IPickable3D>();
        if(pickable != null)
        {
            pickable.OnPick(this);
        }

        /*if(collision.TryGetComponent<IExit>(out IExit exit))
        {
            exit.OnExit(this);
        }*/

    }

    public override void OnHit(float _value, IHitSource3D source)
    {

        base.OnHit(_value, source);
        StartCoroutine(InvincibilityHandle(invincibleTime));
    }

    public override void ChangeHealth(float _value, IHitSource3D source = null)
    {
        base.ChangeHealth(_value, source);
        OnPlayerChangeHealth(health + _value);
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
        //playerInputAction.Player.MouseAim.performed -= OnMouseChangePos;
    }

    IEnumerator InvincibilityHandle(float time)
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
        isDashing = false;

        yield break;
    }

    IEnumerator CantMoveTimer(float timer)
    {
        canMove = false;
        yield return new WaitForSeconds(timer);
        canMove = true;
        trail.emitting = false;
        isDashing = false;
        StartCoroutine(DashCooldownTimer(dashCooldown));



    }

}
