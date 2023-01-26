using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class EnemyEntity : Entity, IHitSource
{
    public EnemySO enemyData;
    public GameObject graphObject;

    private new string name = "";

    private MainCharacterScript player;
    private Vector2 playerPos;
    private bool canMove = true;
    private bool inAttackRange;
    public LayerMask playerLayer;

    public Rigidbody2D SourceRigidbody2D => this.rb2D;

    public float Damage => enemyData.damage;

    private void InitEnemyData()
    {
        animator = graphObject.GetComponent<Animator>();
        this.animator.runtimeAnimatorController = enemyData.animator;
        graphObject.GetComponent<SpriteRenderer>().sprite = enemyData.enemySprite;

        this.health = enemyData.health;
        this.damage = enemyData.damage;
        this.name = enemyData.name;
        this.movementSpeed = enemyData.speed;
    }


    protected override void Start()
    {
        base.Start();
        InitEnemyData();
        StartCoroutine(AIMoveToPlayer());
    }
    void Update()
    {
        if (player != null)
            playerPos = player.GetRigidbody2D().position;
        else
        {
            if (GameManager.Instance.GetPlayer() == null)
                return;
            else
                player = GameManager.Instance.GetPlayer();
        }

        Collider2D playerInRange = Physics2D.OverlapCircle(this.rb2D.position, enemyData.rangeRadius, playerLayer);
        if (playerInRange != null)
        {
            Debug.Log("En range d'attaque");
        }   

    }


    private void UpdateMovement()
    {
        //Un vecteur direction
        Vector2 direction = this.rb2D.position - playerPos;
        //Une velocité

        
    }

    public float GetHealth()
    {
        return this.health;
    }

    public override void OnHit(float _value, IHitSource source)
    {

        StartCoroutine(WaitForMoving(0.2f));
        base.OnHit(_value, source);

    }

    public IEnumerator WaitForMoving(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    private void OnDrawGizmosSelected()
    {
        if(rb2D != null)
            Gizmos.DrawWireSphere(this.rb2D.position, enemyData.rangeRadius);
    }

    IEnumerator AIMoveToPlayer()
    {
        while (!inAttackRange)
        {
            UpdateMovement();


            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(AIAttack(enemyData.attackSpeed));

    }

    //TelegraphTime est le temps d'animation possible avant que l'ennemi attaque. Le temps de voir le coup en gros. Après la petite animation de préparation, le coup sera porté.
    IEnumerator AIAttack(float telegraphTime)
    {
        StopCoroutine(AIMoveToPlayer());
        yield return new WaitForSeconds(telegraphTime);
        //do attack here après le télégraph

        yield return new WaitForSeconds(1f); //après 0.5sec, reprend son mouvement (c'est un peu sa vitesse d'attaque)
        StartCoroutine(AIMoveToPlayer());
    }

}
