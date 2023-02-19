using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class EnemyEntity3D : Entity3D, IHitSource3D
{
    public EnemySO enemyData;

    private new string name = "";

    private MainCharacterScript3D player;
    private Vector3 playerPos;
    private Rigidbody playerRB;
    private bool canMove = true;
    private bool inAttackRange;
    private float range;
    private float rangeSight;
    private float radiusHitbox;
    private EnemySO.Behaviour behaviour;
    public LayerMask playerLayer;
    private bool isFlipped = false;

    private delegate void MovementType();
    private MovementType updateMovement;

    public delegate void AttackType();
    public AttackType startAttack;

    public Rigidbody SourceRigidbody => this.rb;

    public float Damage => enemyData.damage;

    public bool IsDead => isDead;

    private Vector3 lastPlayerPos;

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
        this.range = enemyData.rangeRadius;
        this.rangeSight = enemyData.rangeSight;
    }

    private void Awake()
    {
        this.EventOnHit += GotCanceled;
        this.OnDeath += StopAllCoroutines;
    }

    protected override void Start()
    {
        //StartEnemy(actualRoom);
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
                updateMovement = UpdateMovementRunAwayFromPlayer;
                startAttack = StartRangedAttack;
                break;
        }
        StartCoroutine(AIFindTarget());
        //GetComponent<CircleCollider2D>().radius = this.radiusHitbox;
        healthBar.SetMaxHealth(maxHealth,this);
        this.EventOnHit += GotCanceled;
        this.OnDeath += StopAllCoroutines;
    }

    private void UpdateMovementTowardPlayer()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        //Un vecteur direction
        Vector3 direction = playerRB.position - this.rb.position;
        direction.y = 0;
        //Une velocité
        Vector3 movement = direction.normalized * movementSpeed / 2;


        this.rb.velocity = movement;

        if(isFlipped && rb.velocity.x > 0.01f)
        {
            Flip();
        }

        if(!isFlipped && rb.velocity.x < -0.01f)
        {
            Flip();

        }


    }

    private void UpdateMovementRunAwayFromPlayer()
    {
        //Un vecteur direction
        Vector3 direction = this.rb.position - playerRB.position;
        //Une velocité
        Vector3 movement = direction.normalized * movementSpeed / 2;

        this.rb.velocity = movement;

    }

    public float GetHealth()
    {
        return this.health;
    }

    public override void OnHit(float _value, IHitSource3D source)
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

    private void OnDrawGizmosSelected()
    {
        if(rb != null)
            Gizmos.DrawSphere((lastPlayerPos - this.rb.position) + this.rb.position, enemyData.rangeRadius / 2);
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

        Collider[] playerInRange;
        switch (behaviour)
        {
            case EnemySO.Behaviour.Melee:
                playerInRange = Physics.OverlapSphere(this.rb.position, rangeSight, playerLayer);
                if (playerInRange != null)
                {
                    foreach (var p in playerInRange)
                    {
                        if (p.TryGetComponent<MainCharacterScript3D>(out MainCharacterScript3D _player))
                        {
                            player = _player;
                            playerRB = p.attachedRigidbody;
                        }
                        
                    }
                    
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


        
        //tant que joueur pas trouvé
        while (player == null)
        {
            playerInRange = Physics.OverlapSphere(this.rb.position, rangeSight, playerLayer);
            if (playerInRange != null)
            {
                foreach (var p in playerInRange)
                {
                    if (p.TryGetComponent<MainCharacterScript3D>(out MainCharacterScript3D _player))
                    {
                        player = _player;
                        playerRB = p.attachedRigidbody;
                    }

                }
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
                //fuis le joueur ou du moins va en range d'attaque à distance
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
        animator.SetBool("IsMoving", canMove);
        while (!inAttackRange)
        {
            updateMovement();
            Collider[] playerInRange = Physics.OverlapSphere(this.rb.position, enemyData.rangeRadius, playerLayer);
            if (playerInRange != null)
            {
                foreach (var p in playerInRange)
                {
                    if (p.TryGetComponent<MainCharacterScript3D>(out MainCharacterScript3D _player))
                    {
                        inAttackRange = true;
                    }

                }
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
            Collider[] playerInRange = Physics.OverlapSphere(this.rb.position, range, playerLayer);
            if (playerInRange != null)
            {
                foreach (var p in playerInRange)
                {
                    if (p.TryGetComponent<MainCharacterScript3D>(out MainCharacterScript3D _player))
                    {
                        inAttackRange = true;
                    }

                }
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
        this.rb.velocity = this.rb.velocity / 1.5f;
        this.animator.SetTrigger("Attack");
        yield return null;
        yield return StartCoroutine(WaitForAttack());
        gotCanceled = false;
        yield return new WaitForSeconds(0.5f); //après 1sec, reprend son mouvement (c'est un peu sa vitesse d'attaque)
        StartCoroutine(AIFindTarget());
        yield break;
    }
    #endregion
    public void StartMeleeAttack()
    {
        var dir = (lastPlayerPos - this.rb.position);
        dir.y = 0;
        this.rb.AddForce(dir.normalized * 5,ForceMode.Impulse);
        StartCoroutine(ActiveFrameAttack());
        particle.transform.position = (lastPlayerPos - this.rb.position ) + this.rb.position;
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
                Collider[] playerInRange = Physics.OverlapSphere((lastPlayerPos - this.rb.position) + this.rb.position, enemyData.rangeRadius / 2, playerLayer);
                if (playerInRange != null)
                {
                    foreach (var p in playerInRange)
                    {
                        if (p.TryGetComponent<MainCharacterScript3D>(out MainCharacterScript3D _player))
                        {
                            _player.OnHit(-damage, this);
                            playerHitOnce = true;
                        }

                    }
                    
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
        GameObject projectile = Instantiate(enemyData.projectile, this.rb.position + Vector3.up * 0.5f, Quaternion.identity);
        var dir = (playerRB.position - this.rb.position);
        dir.y = 0;
        projectile.GetComponent<ProjectileScript>().SetProjectile(dir.normalized, 12f,damage);
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
