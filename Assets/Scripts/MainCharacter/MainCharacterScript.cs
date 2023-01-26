using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacterScript : Entity
{
   [SerializeField] private Vector2 input;

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

    

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
        playerAttackScript = GetComponent<PlayerAttack>();

        playerInputAction.Player.Enable();
        playerInputAction.Player.Attack.performed += playerAttackScript.OnAttack;
        playerInputAction.Player.MouseAim.performed += OnMouseChangePos;

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
    }


    // Update is called once per frame
    void Update()
    {
        input = playerInputAction.Player.Move.ReadValue<Vector2>();
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


}
