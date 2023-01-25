using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{
    public EnemySO enemyData;
    public GameObject graphObject;

    private new string name = "";

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
        
    }

    public float GetHealth()
    {
        return this.health;
    }

    public override void ChangeHealth(float _value, Entity sender = null)
    {
        animator.SetTrigger("Hurt");
        base.ChangeHealth(_value);
    }
}
