using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class EnemyEntity : Entity, IHitSource
{
    public EnemySO enemyData;

    private new string name = "";

    private MainCharacterScript player;
    private Vector2 playerPos;
    private Rigidbody2D playerRB;
    private bool canMove = true;
    private bool inAttackRange;
    public LayerMask playerLayer;

    public Rigidbody2D SourceRigidbody2D => this.rb2D;

    public float Damage => enemyData.damage;

    private Vector2 lastPlayerPos;
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
        StartCoroutine(AIFindTarget());
    }

    private void UpdateMovement()
    {
        //Un vecteur direction
        Vector2 direction = playerRB.position - this.rb2D.position;
        //Une velocité
        Vector2 movement = direction.normalized * movementSpeed;

        this.rb2D.velocity = movement;
        
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

    IEnumerator AIFindTarget()
    {
        player = null;
        playerRB = null;
        Collider2D playerInRange = Physics2D.OverlapCircle(this.rb2D.position, enemyData.rangeRadius * 3, playerLayer);
        if (playerInRange != null)
        {
            player = playerInRange.GetComponent<MainCharacterScript>();
            playerRB = playerInRange.attachedRigidbody;
        }
        //tant que joueur pas trouvé
        while (player == null)
        {
            playerInRange = Physics2D.OverlapCircle(this.rb2D.position, enemyData.rangeRadius * 3, playerLayer);
            if (playerInRange != null)
            {
                player = playerInRange.GetComponent<MainCharacterScript>();
                playerRB = playerInRange.attachedRigidbody;
            }
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(AIMoveToPlayer());
        yield break;
    }

    IEnumerator AIPatrol()
    {
        yield return null;
    }
    IEnumerator AIMoveToPlayer()
    {
        while (!inAttackRange)
        {
            UpdateMovement();
            Collider2D playerInRange = Physics2D.OverlapCircle(this.rb2D.position , enemyData.rangeRadius, playerLayer);
            if (playerInRange != null)
            {
                inAttackRange = true;
            }

            yield return new WaitForFixedUpdate();
        }
        inAttackRange = false;
        StartCoroutine(AIAttack(enemyData.attackSpeed));
        yield break;

    }

    //TelegraphTime est le temps d'animation possible avant que l'ennemi attaque. Le temps de voir le coup en gros. Après la petite animation de préparation, le coup sera porté.
    IEnumerator AIAttack(float telegraphTime)
    {
        this.rb2D.velocity = this.rb2D.velocity / 2;
        lastPlayerPos = playerRB.position;
        this.animator.SetTrigger("Attack");
        yield return null;
        yield return StartCoroutine(WaitForAttack());
        yield return new WaitForSeconds(1f); //après 1sec, reprend son mouvement (c'est un peu sa vitesse d'attaque)
        StartCoroutine(AIFindTarget());
        yield break;
    }

    public void StartAttack()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle((this.rb2D.position + lastPlayerPos.normalized) * 1.2f, enemyData.rangeRadius / 2, playerLayer);
        if (playerInRange != null)
        {
            player.OnHit(-damage,this);
        }
    }


    private bool endAttack = false;
    IEnumerator WaitForAttack()
    {

        while (!endAttack)
        {
            yield return null;
        }
        endAttack = false;
        yield break;
    }
    public void EndAttack()
    {
        endAttack = true;
    }

}
