using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity, IHitSource
{
    public EnemySO enemyData;
    public GameObject graphObject;

    private new string name = "";

    private MainCharacterScript player;
    private Vector2 playerPos;
    private bool canMove = true;

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
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (canMove == false)
        {
            return;
        }
        Vector2 targetSpeed = (playerPos - this.rb2D.position).normalized * movementSpeed / 5;
        var speedDiff = targetSpeed - this.rb2D.velocity;
        var accelRate = new Vector2(Mathf.Abs(targetSpeed.x) > 0.01f ? acceleration.x : decceleration.x, Mathf.Abs(targetSpeed.y) > 0.01f ? acceleration.y : decceleration.y);
        var movement = new Vector2(Mathf.Pow(Mathf.Abs(speedDiff.x) * accelRate.x, velocityPower.x) * Mathf.Sign(speedDiff.x), Mathf.Pow(Mathf.Abs(speedDiff.y) * accelRate.y, velocityPower.y) * Mathf.Sign(speedDiff.y));

        this.rb2D.AddForce(movement, ForceMode2D.Impulse);
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

}
