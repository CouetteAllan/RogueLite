using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IHittable
{
    void OnHit(float _value, IHitSource source);
}

public interface IPickable
{
    void OnPick(Entity entity);
}

public interface IHitSource
{
    Rigidbody2D SourceRigidbody2D { get; }
    float Damage { get; }
}
