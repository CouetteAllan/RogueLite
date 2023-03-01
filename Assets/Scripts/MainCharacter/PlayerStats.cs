using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CharacterStats;

public enum StatType
{
    CritChance,
    CritMultiplier,
    Damage,
    MovementSpeed,
    MaxHealth,
    AttackSpeed
}

[Serializable]
public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private PlayerSingleStat critChance;
    [SerializeField]
    private PlayerSingleStat critMultiplier;
    [SerializeField]
    private PlayerSingleStat Damage;
    [SerializeField]
    private PlayerSingleStat movementSpeed ;
    [SerializeField]
    private PlayerSingleStat maxHealth;
    [SerializeField]
    private PlayerSingleStat attackSpeed;

    private List<EnchantingContext> items = new List<EnchantingContext>();

    [SerializeField]
    private Dictionary<StatType, PlayerSingleStat> stats = new Dictionary<StatType, PlayerSingleStat>();

    private MainCharacterScript3D player;

    public event Action OnMaxHealthChange;
    public event Action<StatType> OnPlayerStatChange;

    private void Awake()
    {
        InitplayerStats();
        player = GetComponent<MainCharacterScript3D>();

    }
    private void InitplayerStats()
    {
        AddNewStat(StatType.CritChance, critChance);
        AddNewStat(StatType.CritMultiplier, critMultiplier);
        AddNewStat(StatType.Damage, Damage);
        AddNewStat(StatType.MovementSpeed, movementSpeed);
        AddNewStat(StatType.MaxHealth, maxHealth);
        AddNewStat(StatType.AttackSpeed, attackSpeed);
    }

    public void AddNewStat(StatType type, PlayerSingleStat stat)
    {
        stats.Add(type, stat);
    }

    public PlayerSingleStat GetStat(StatType type)
    {
        return stats[type]; 
    }

    public void AddModifier(StatModifier mod, StatType type)
    {
        GetStat(type).AddModifier(mod);
        OnPlayerStatChange(type);
        if (type == StatType.MaxHealth)
        {
            OnMaxHealthChange?.Invoke();
        }
    }

    public void RemoveModifier(StatModifier mod, StatType type)
    {
        GetStat(type).RemoveModifier(mod);
    }

    public void RemoveAllModifiersFromSource(object source, StatType type)
    {
        GetStat(type).RemoveAllModifiersFromSource(source);
    }
}
