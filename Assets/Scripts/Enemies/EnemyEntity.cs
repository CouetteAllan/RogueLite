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
    private EnemySO.Behaviour behaviour;
    public LayerMask playerLayer;

    private delegate void MovementType();
    private MovementType updateMovement;

    public delegate void AttackType();
    public AttackType startAttack;

    public Rigidbody2D SourceRigidbody2D => this.rb2D;

    public float Damage => enemyData.damage;

    private Vector2 lastPlayerPos;

    [SerializeField] private GameObject particle;
    private void InitEnemyData()
    {
        animator = graphObject.GetComponent<Animator>();
        this.animator.runtimeAnimatorController = enemyData.animator;
        graphObject.GetComponent<SpriteRenderer>().sprite = enemyData.enemySprite;

        this.health = enemyData.health;
        this.damage = enemyData.damage;
        this.name = enemyData.name;
        this.movementSpeed = enemyData.speed;
        this.behaviour = enemyData.behaviour;
    }

    protected override void Start()
    {
        base.Start();
        InitEnemyData();
        switch (behaviour)
        {
            case EnemySO.Behaviour.Melee:
                updateMovement = UpdateMovementTowardPlayer;
                startAttack = StartMeleeAttack;
                break;
            case EnemySO.Behaviour.Ranged:
                updateMovement = UpdateMovementRunAwayFromPlayer;
                startAttack = StartRangedAttack;
                break;
        }
        StartCoroutine(AIFindTarget());
       
    }

    private void UpdateMovementTowardPlayer()
    {
        //Un vecteur direction
        Vector2 direction = playerRB.position - this.rb2D.position;
        //Une velocit�
        Vector2 movement = direction.normalized * movementSpeed / 2;

        this.rb2D.velocity = movement;
        
    }

    private void UpdateMovementRunAwayFromPlayer()
    {
        //Un vecteur direction
        Vector2 direction = this.rb2D.position - playerRB.position;
        //Une velocit�
        Vector2 movement = direction.normalized * movementSpeed / 2;

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

    #region AITasks

    IEnumerator AIFindTarget()
    {
        player = null;
        playerRB = null;

        Collider2D playerInRange;
        switch (behaviour)
        {
            case EnemySO.Behaviour.Melee:
                playerInRange = Physics2D.OverlapCircle(this.rb2D.position, enemyData.rangeRadius * 3, playerLayer);
                if (playerInRange != null)
                {
                    player = playerInRange.GetComponent<MainCharacterScript>();
                    playerRB = playerInRange.attachedRigidbody;
                }
                //cours vers le joueur;
                break;
            /*case EnemySO.Behaviour.Ranged:
                playerInRange = Physics2D.OverlapCircle(this.rb2D.position, enemyData.rangeRadius - 1f, playerLayer); //joueur trop proche ! il faut fuir
                if (playerInRange != null)
                {
                    player = playerInRange.GetComponent<MainCharacterScript>();
                    playerRB = playerInRange.attachedRigidbody;
                }
                break;*/
        }


        
        //tant que joueur pas trouv�
        while (player == null)
        {
            playerInRange = Physics2D.OverlapCircle(this.rb2D.position, enemyData.rangeRadius * 4, playerLayer);
            if (playerInRange != null)
            {
                player = playerInRange.GetComponent<MainCharacterScript>();
                playerRB = playerInRange.attachedRigidbody;
            }
            yield return new WaitForSeconds(0.1f);
        }
        switch (behaviour)
        {
            case EnemySO.Behaviour.Melee:
                StartCoroutine(AIMoveToPlayer());
                //cours vers le joueur;
                break;
            case EnemySO.Behaviour.Ranged:
                StartCoroutine(AIMoveInRange(enemyData.rangeRadius));
                //fuis le joueur ou du moins va en range d'attaque � distance
                break;
        }
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
            updateMovement();
            Collider2D playerInRange = Physics2D.OverlapCircle(this.rb2D.position , enemyData.rangeRadius, playerLayer);
            if (playerInRange != null)
            {
                inAttackRange = true;
            }

            yield return new WaitForFixedUpdate();
        }
        inAttackRange = false;
        lastPlayerPos = playerRB.position;
        StartCoroutine(AIAttack());
        yield break;

    }

    IEnumerator AIMoveInRange(float range)
    {
        while (!inAttackRange)
        {
            UpdateMovementTowardPlayer();
            Collider2D playerInRange = Physics2D.OverlapCircle(this.rb2D.position, range, playerLayer);
            if (playerInRange != null)
            {
                inAttackRange = true;
            }

            yield return new WaitForFixedUpdate();
        }
        inAttackRange = false;
        StartCoroutine(AIAttack());
        yield break;
    }

    IEnumerator AIRunAwayFromPlayer()
    {
        while (inAttackRange)
        {
            updateMovement();
            Collider2D playerInRange = Physics2D.OverlapCircle(this.rb2D.position, enemyData.rangeRadius, playerLayer);
            if (playerInRange != null)
            {
                inAttackRange = true;
            }

            yield return new WaitForFixedUpdate();
        }
        inAttackRange = false;
        lastPlayerPos = playerRB.position;
        StartCoroutine(AIAttack());
        yield break;
    }

    //TelegraphTime est le temps d'animation possible avant que l'ennemi attaque. Le temps de voir le coup en gros. Apr�s la petite animation de pr�paration, le coup sera port�.
    IEnumerator AIAttack()
    {
        this.rb2D.velocity = this.rb2D.velocity / 2;
        this.animator.SetTrigger("Attack");
        yield return null;
        yield return StartCoroutine(WaitForAttack());
        yield return new WaitForSeconds(1f); //apr�s 1sec, reprend son mouvement (c'est un peu sa vitesse d'attaque)
        StartCoroutine(AIFindTarget());
        yield break;
    }
    #endregion
    public void StartMeleeAttack()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle((this.rb2D.position + lastPlayerPos.normalized) * 1.2f, enemyData.rangeRadius / 2, playerLayer);
        particle.transform.position = this.rb2D.position + lastPlayerPos.normalized;
        particle.GetComponent<ParticleSystem>().Play();
        if (playerInRange != null)
        {
            player.OnHit(-damage,this);
        }
    }

    public void StartRangedAttack()
    {
        //instancier un projectile dans la direction du player.
        GameObject projectile = Instantiate(enemyData.projectile, this.rb2D.position, Quaternion.identity);
        projectile.GetComponent<ProjectileScript>().SetDirectionAndSpeed((playerRB.position - this.rb2D.position).normalized, 8f);
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
