using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IHittable
{
    void OnHit(float _value, IHitSource source);

    bool GotHit { get; set; }
}

public interface IPickable
{
    void OnPick(Entity entity);
}

public interface IHitSource
{
    Rigidbody2D SourceRigidbody2D { get; }
    float Damage { get; }

    bool IsDead { get; }
}

public interface IExit
{
    void OnExit(MainCharacterScript player);
}
