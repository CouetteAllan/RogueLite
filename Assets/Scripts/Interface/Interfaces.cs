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
    void OnPick(MainCharacterScript3D entity);
}

public interface IPickable3D
{
    void OnPick(MainCharacterScript3D entity);
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
    void OnExit(MainCharacterScript3D player);
}

public interface IHealable
{
    void OnHeal(float heal);
}

public interface IInteractable
{
    void OnInteract(MainCharacterScript3D player);
    Transform interactTransform { get; }
}
