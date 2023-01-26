using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour, IHitSource
{
    private Weapons actualWeapon;
    private float attackSpeed = 1f;
    private PlayerInputAction playerInputAction;
    private MainCharacterScript player;
    [SerializeField] GameObject hand;
    [SerializeField] LayerMask layerAttack;

    public Vector2 range;
    public Vector2 offset;

    private Rigidbody2D rb2D;
    public GameObject hitbox;


    public bool isAttacking = false;
    public Vector2 MouseAimDir { get; set; }

    [SerializeField] private float pushForceForward = 6f;
    private Vector2 HandPosition => hand.transform.position;


    public Rigidbody2D SourceRigidbody2D => this.rb2D;
    public float Damage => actualWeapon.damage;
    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        
        player = GetComponent<MainCharacterScript>();
    }



    public void OnAttack(InputAction.CallbackContext context)
    {
        
        //Animation d'attaque
        player.GetAnimator().SetTrigger("Attack");

        rb2D.velocity = Vector2.zero;
        if(MouseAimDir.normalized.x > 0.01f && player.isFlipped == true)
        {
            player.Flip();
        }
        else if (MouseAimDir.normalized.x < -0.01f && player.isFlipped == false)
        {
            player.Flip();
        }

    }

    public void ActivateHitBox()
    {
        if (actualWeapon == null)
            return;

        rb2D.AddForce(MouseAimDir.normalized * pushForceForward, ForceMode2D.Impulse);
        hitbox.SetActive(true);

        offset = rb2D.position + MouseAimDir.normalized * 1.5f;
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(offset, actualWeapon.range / 20,layerAttack);
        foreach (var enemy in enemiesHit)
        {
            IHittable hitObject =  enemy.GetComponent<EnemyEntity>();
            hitObject.OnHit(-actualWeapon.damage,this);
        }

        hitbox.transform.position = new Vector3(offset.x,offset.y,0);
    }

    public void DeActivateHitBox()
    {
        //desactive la hitbox en fin d'animation
        hitbox.SetActive(false);

    }

    public void SetActualWeapon(Weapons weapon)
    {
        this.actualWeapon = weapon;
        hand.GetComponent<SpriteRenderer>().sprite = actualWeapon.sprite;
    }
    
}
