using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum StatType
{
    CriticalChance,
    CriticalMultiplier,
    Damage,
    MaxHealth,
    MovementSpeed,
    AttackSpeed
}


public class PlayerStats
{
    private Dictionary<StatType, float> playerStats;
    float critChance = 0f;
    float critMultiplier = 2f;
    float movementSpeedBonus = 0f;
    float actualMaxHealth = 0f;
    float bonusDamage = 0f;
    float bonusAttckSpeed = 0f;

    private MainCharacterScript3D player;

    public event Action OnStatChanged;

    public PlayerStats(MainCharacterScript3D _player)
    {
        playerStats = new Dictionary<StatType, float>();
        this.player = _player;
        actualMaxHealth = _player.GetEntityMaxHealth();
    }

    private void InitplayerStats()
    {

    }

    public float GetStatValue(StatType type)
    {
        if (playerStats.TryGetValue(type, out float value))
            return value;
        else
        {
            Debug.LogError("$No value found of type ");
            return 0;
        }
            
    }

    public void SetAttckSpeed(float _attackSpeed)
    {
        bonusAttckSpeed += _attackSpeed;
        OnStatChanged?.Invoke();
    }

    public void SetDamage(float _damage)
    {
        bonusDamage += _damage;
        OnStatChanged?.Invoke();

    }
    public void SetBonusHealth(float _health)
    {
        actualMaxHealth += _health;
        player.SetEntityMaxHealth(actualMaxHealth);
        OnStatChanged?.Invoke();

    }
    public void SetSpeed(float _speed)
    {
        movementSpeedBonus += _speed;
        OnStatChanged?.Invoke();

    }
    public void SetCritMultiplier(float _critMult)
    {
        critMultiplier += _critMult;
        OnStatChanged?.Invoke();

    }
    public void SetCritChance(float _critChance)
    {
        critChance += _critChance;
        OnStatChanged?.Invoke();

    }

    public float GetAttckSpeed()
    {
        return bonusAttckSpeed;
    }

    public float GetDamage()
    {
        return bonusDamage;
    }
    public float GetBonusHealth()
    {
        return actualMaxHealth;
    }
    public float GetSpeed()
    {
        return movementSpeedBonus;
    }
    public float GetCritMultiplier()
    {
        return critMultiplier;
    }
    public float GetCritChance()
    {
        return critChance;
    }
}
