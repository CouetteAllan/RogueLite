using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{
    public EnemySO enemyData;
    public GameObject graphObject;

    private new string name = "";

    private MainCharacterScript player;
    private Vector2 playerPos;

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
        player = GameManager.Instance.player;
    }
    void Update()
    {
        if(player != null)
            playerPos = player.GetRigidbody2D().position;
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {

        Vector2 targetSpeed = new Vector2(this.rb2D.position.x - playerPos.x, this.rb2D.position.y - playerPos.y) * movementSpeed;
        var speedDiff = targetSpeed - this.rb2D.velocity;
        var accelRate = new Vector2(Mathf.Abs(targetSpeed.x) > 0.01f ? acceleration.x : decceleration.x, Mathf.Abs(targetSpeed.y) > 0.01f ? acceleration.y : decceleration.y);
        var movement = new Vector2(Mathf.Pow(Mathf.Abs(speedDiff.x) * accelRate.x, velocityPower.x) * Mathf.Sign(speedDiff.x), Mathf.Pow(Mathf.Abs(speedDiff.y) * accelRate.y, velocityPower.y) * Mathf.Sign(speedDiff.y));

        this.rb2D.AddForce(movement, ForceMode2D.Impulse);
    }

    public float GetHealth()
    {
        return this.health;
    }

    public override void ChangeHealth(float _value, Entity sender = null)
    {
        animator.SetTrigger("Hurt");
        base.ChangeHealth(_value,sender);
    }
}
