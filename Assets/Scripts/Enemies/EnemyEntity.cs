using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class EnemyEntity : Entity, IHitSource
{
    public EnemySO enemyData;
    

    private MainCharacterScript player;
    private Vector2 playerPos;
    private Rigidbody2D playerRB;
    private bool canMove = true;
    private bool inAttackRange;
    private float radiusHitbox;
    private EnemySO.Behaviour behaviour;
    public LayerMask playerLayer;
    private bool isFlipped = false;

    private delegate void MovementType();
    private MovementType updateMovement;

    public delegate void AttackType();
    public AttackType startAttack;

    public Rigidbody2D SourceRigidbody2D => this.rb2D;

    public float Damage => enemyData.damage;

    public bool IsDead => isDead;

    private Vector2 lastPlayerPos;

    [SerializeField] private GameObject particle;
    private RoomSpawnHandle actualRoom;

    [SerializeField] private HealthBarBehaviour healthBar;
    private void InitEnemyData()
    {
        animator = GetComponent<Animator>();
        this.animator.runtimeAnimatorController = enemyData.animator;

        this.maxHealth = enemyData.health;
        this.health = maxHealth;
        this.damage = enemyData.damage;
        this.name = enemyData.name;
        this.movementSpeed = enemyData.speed;
        this.behaviour = enemyData.behaviour;
        this.radiusHitbox = enemyData.radiusHitBox;
    }

    private void Awake()
    {
        this.EventOnHit += GotCanceled;
        this.OnDeath += StopAllCoroutines;
    }

    protected override void Start()
    {
        base.Start();
        /*switch (behaviour)
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
        GetComponent<CircleCollider2D>().radius = this.radiusHitbox;*/
       
    }

    public void StartEnemy(RoomSpawnHandle _actualRoom)
    {
        base.Start();
        actualRoom = _actualRoom;
        InitEnemyData();
        switch (behaviour)
        {
            case EnemySO.Behaviour.Melee:
                updateMovement = UpdateMovementTowardPlayer;
                startAttack = StartMeleeAttack;
                break;
            case EnemySO.Behaviour.Ranged:
                startAttack = StartRangedAttack;
                break;
        }
        StartCoroutine(AIFindTarget());
        GetComponent<CircleCollider2D>().radius = this.radiusHitbox;
        //healthBar.SetMaxHealth(maxHealth,this);
        this.EventOnHit += GotCanceled;
        this.OnDeath += StopAllCoroutines;
    }

    private void UpdateMovementTowardPlayer()
    {
        if (!canMove)
        {
            rb2D.velocity = Vector2.zero;
            return;
        }
        //Un vecteur direction
        Vector2 direction = playerRB.position - this.rb2D.position;
        //Une velocit?
        Vector2 movement = direction.normalized * movementSpeed / 2;


        this.rb2D.velocity = movement;

        if(isFlipped && rb2D.velocity.x > 0.01f)
        {
            Flip();
        }

        if(!isFlipped && rb2D.velocity.x < -0.01f)
        {
            Flip();

        }


    }

    public float GetHealth()
    {
        return this.health;
    }

    public override void OnHit(float _value, IHitSource source)
    {

        StartCoroutine(WaitForMoving(0.2f));
        base.OnHit(_value, source);
        healthBar.SetHealth(health);

    }

    public override void Die()
    {
        actualRoom.SubstractEnemy();
        base.Die();
    }

    public IEnumerator WaitForMoving(float seconds)
    {
        canMove = false;
        animator.SetBool("IsMoving", canMove);
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    public void Flip()
    {
        isFlipped = !isFlipped;
        Vector3 currentScale = this.gameObject.transform.localScale;
        currentScale.x *= -1;
        this.gameObject.transform.localScale = currentScale;

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
        }
        //tant que joueur pas trouv?
        while (player == null)
        {
            playerInRange = Physics2D.OverlapCircle(this.rb2D.position, enemyData.rangeRadius * 6, playerLayer);
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
                //fuis le joueur ou du moins va en range d'attaque ? distance
                break;
        }
        yield break;
    }

    IEnumerator AIMoveToPlayer()
    {
        animator.SetBool("IsMoving", canMove);
        while (!inAttackRange)
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

    IEnumerator AIMoveInRange(float range)
    {
        animator.SetBool("IsMoving", canMove);
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

    /*IEnumerator AIRunAwayFromPlayer()
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
    }*/

    IEnumerator AIAttack()
    {
        animator.SetBool("IsMoving", false);
        this.rb2D.velocity = this.rb2D.velocity / 2;
        this.animator.SetTrigger("Attack");
        yield return null;
        yield return StartCoroutine(WaitForAttack());
        gotCanceled = false;
        yield return new WaitForSeconds(0.5f); //apr?s 0.5sec, reprend son mouvement (c'est un peu sa vitesse d'attaque)
        StartCoroutine(AIFindTarget());
        yield break;
    }
    #endregion
    public void StartMeleeAttack()
    {
        this.rb2D.AddForce((lastPlayerPos - this.rb2D.position).normalized * 3,ForceMode2D.Impulse);
        StartCoroutine(ActiveFrameAttack());
        particle.transform.position = (lastPlayerPos - this.rb2D.position ) + this.rb2D.position;
        particle.GetComponent<ParticleSystem>().Play();
    }
    IEnumerator ActiveFrameAttack()
    {
        bool playerHitOnce = false;
        endAttack = false;
        while (!endAttack)
        {
            if (!playerHitOnce)
            {
                Collider2D playerInRange = Physics2D.OverlapCircle((lastPlayerPos - this.rb2D.position) + this.rb2D.position, enemyData.rangeRadius / 2, playerLayer);
                if (playerInRange != null)
                {
                    player.OnHit(-damage, this);
                    playerHitOnce = true;
                }
            }
            yield return new WaitForSeconds(0.15f);

        }
        endAttack = false;
        yield break;

    }

    public void StartRangedAttack()
    {
        //instancier un projectile dans la direction du player.
        GameObject projectile = Instantiate(enemyData.projectile, this.rb2D.position, Quaternion.identity);
        projectile.GetComponent<ProjectileScript>().SetProjectile((playerRB.position - this.rb2D.position).normalized, 12f,damage);
    }


    private bool endAttack = false;
    private bool gotCanceled = false;
    IEnumerator WaitForAttack()
    {

        while (!endAttack && !gotCanceled)
        {
            yield return null;
        }
        endAttack = true;
        yield break;
    }
    public void EndAttack()
    {
        endAttack = true;
    }

    private void GotCanceled()
    {
        gotCanceled = true;
    }

}
