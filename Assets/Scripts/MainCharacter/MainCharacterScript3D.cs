using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using CharacterStats;


public class MainCharacterScript3D : Entity3D,IHealable
{
    public static Action<float> playerChangeHealth;

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

    [HideInInspector] public Vector2 mouseAim;

    [HideInInspector] public bool isFlipped = false;
    private float time = 0f;

    private PlayerAttack3D playerAttackScript;
    private Weapons weaponPickedUpData;

    private bool isMoving => input != Vector2.zero;


    public bool startWithWeapon = false;
    [SerializeField] private Weapons weapon;

    private bool canMove = true;
    public bool CanMove { get => canMove; set => canMove = value; }

    private Coroutine invincibleCoroutine;

    [SerializeField]
    private PlayerStats currentPlayerStats;

    

    private void OnDisable()
    {
        //playerInputAction.Player.MouseAim.performed -= OnMouseChangePos;
        InputManager.playerInputAction.Player.Dash.performed -= Dash;
        InputManager.playerInputAction.Player.Move.canceled -= Move_canceled;
        InputManager.playerInputAction.Player.Move.started -= Move_started;
    }

    private void Awake()
    {
        //InputManager.playerInputAction.Player.Enable();
        InputManager.playerInputAction.Player.Dash.performed += Dash;
        InputManager.playerInputAction.Player.Move.canceled += Move_canceled;
        InputManager.playerInputAction.Player.Move.started += Move_started;
        InputManager.playerInputAction.Player.Interact.performed += Interact_performed;
        currentPlayerStats = GetComponent<PlayerStats>();
        Debug.Log("current damage: " + damage);
        currentPlayerStats.OnMaxHealthChange += OnMaxHealthChange;
        
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        Collider[] interacts = Physics.OverlapSphere(this.rb.position, 2f);
        foreach (var i in interacts)
        {
            if(i.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.OnInteract(this);
            }
        }
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //GameManager.Instance.InitPlayer(this);
        playerAttackScript = GetComponent<PlayerAttack3D>();
        if (startWithWeapon)
        {
            PickUpWeapon(weapon);
            playerAttackScript.SetAttackSpeed(weaponPickedUpData.attackRate);
            Debug.Log(currentPlayerStats.GetStat(StatType.Damage).Value);
        }
        trail.emitting = false;
        maxHealth = currentPlayerStats.GetStat(StatType.MaxHealth).Value;
        health = maxHealth;
        UIManager.Instance.SetMaxHealth(maxHealth,this);
        UIManager.Instance.SetUIHealth(maxHealth);
        animator = GetComponent<Animator>();
    }



    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            input = InputManager.playerInputAction.Player.Move.ReadValue<Vector2>();
            if (Mathf.Abs(input.x) > 0.009f || Mathf.Abs(input.y) > 0.009f)
            {
                lastInput = input;

            }
        }
        else
            input = Vector3.zero;
        if (Mathf.Abs(input.x) > 0.009f || Mathf.Abs(input.y) > 0.009f)
            playerAttackScript.LastInput = lastInput;
        if (input.x > 0.01f && isFlipped)
        {
            Flip();
        }
        else if (input.x < -0.01f && !isFlipped)
        {
            Flip();
        }

        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("ZSpeed", lastInput.y);

        timeCurve += Time.deltaTime;

    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (isDashing)
            return;

        Vector3 targetMovement = new Vector3(input.x, 0, input.y);

        AnimationCurve speedcurve = isMoving ? accelerationCurve : decelerationCurve;

        rb.velocity = new Vector3(targetMovement.x * (currentPlayerStats.GetStat(StatType.MovementSpeed).Value * speedcurve.Evaluate(timeCurve)), rb.velocity.y, targetMovement.z * (currentPlayerStats.GetStat(StatType.MovementSpeed).Value * speedcurve.Evaluate(timeCurve)));
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

    private void OnMaxHealthChange()
    {
        UIManager.Instance.SetMaxHealth(currentPlayerStats.GetStat(StatType.MaxHealth).Value);
        health += currentPlayerStats.GetStat(StatType.MaxHealth).Value - currentPlayerStats.GetStat(StatType.MaxHealth).baseValue;
    }

    public void OnHeal(float heal)
    {
        ChangeHealth(heal);
    }
    

    IEnumerator DashCoroutine()
    {
        if (invincibleCoroutine != null)
            StopCoroutine(invincibleCoroutine);
        invincibleCoroutine = StartCoroutine(DashInvincibilityHandle(dashInvincibleTime));
        //un delay de 6frames environ
        yield return new WaitForSeconds(0.07f);


        isDashing = true;
        //Va rapidement dans une direction
        this.rb.velocity = Vector3.zero;
        this.rb.AddForce(new Vector3(lastInput.x, 0, lastInput.y) * (dashForce + (currentPlayerStats.GetStat(StatType.MovementSpeed).Value/15) ), ForceMode.Impulse);
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
        weaponPickedUpData.bonusDamage = new StatModifier(weaponPickedUpData.damage, ModType.Flat, weapon);
        currentPlayerStats.AddModifier(weaponPickedUpData.bonusDamage, StatType.Damage);
        playerAttackScript.SetActualWeapon(weaponPickedUpData);
    }


    public Animator GetAnimator()
    {
        return animator;
    }

    public Rigidbody GetRigidbody2D()
    {
        return rb;
    }


    private void OnTriggerEnter(Collider collision)
    {

        IPickable3D pickable = collision.GetComponent<IPickable3D>();
        if (pickable != null)
        {
            pickable.OnPick(this);
            Debug.Log(currentPlayerStats.GetStat(StatType.Damage).Value);

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
        if (isDead)
            return;
        if (_value < 0)
        {
            animator.SetTrigger("Hurt");
            if (source != null)
            {
                Vector2 pushDirection = this.rb.position - source.SourceRigidbody.position;
                this.rb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
            }
        }

        health = Mathf.Clamp(health += _value, 0, currentPlayerStats.GetStat(StatType.MaxHealth).Value);

        if (health <= 0)
            this.Die();
        playerChangeHealth(health + _value);
    }
    public override void Die()
    {
        GameManager.Instance.PlayerDeath();
        base.Die();
    }

    private void OnDestroy()
    {
        InputManager.playerInputAction.Player.Disable();
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

    public PlayerStats GetPlayerStats()
    {
        return this.currentPlayerStats;
    }


}
