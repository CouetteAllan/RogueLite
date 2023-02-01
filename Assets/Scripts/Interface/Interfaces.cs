using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IHittable
{
    void OnHit(float _value, IHitSource source);

    bool GotHit { get; set; }
}

public interface IHittable3D
{
    void OnHit(float _value, IHitSource3D source);

    bool GotHit { get; set; }
}

public interface IPickable
{
    void OnPick(Entity entity);
}

public interface IPickable3D
{
    void OnPick(Entity3D entity);
}

public interface IHitSource
{
    Rigidbody2D SourceRigidbody2D { get; }
    float Damage { get; }

    bool IsDead { get; }
}

public interface IHitSource3D
{
    Rigidbody SourceRigidbody { get; }
    float Damage { get; }

    bool IsDead { get; }
}

public interface IExit
{
    void OnExit(MainCharacterScript player);
}
