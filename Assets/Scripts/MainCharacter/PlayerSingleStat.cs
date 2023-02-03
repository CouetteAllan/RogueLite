using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

namespace CharacterStats
{


    [Serializable]
    public class PlayerSingleStat //Une stat du joueur (ou de l'entité ?) à attribuer (ça peut être les dégâts, le crit etc)
    {
        public float baseValue;
        protected float lastBaseValue = float.MinValue;

        protected float _value;
        protected bool isDirty = true;
        public float Value
        {
            get
            {
                if (isDirty || baseValue != lastBaseValue)
                {
                    lastBaseValue = baseValue;
                    _value = CalculateValue();
                    isDirty = false;
                }
                return _value;

            }
        }

        protected readonly List<StatModifier> statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        public PlayerSingleStat()
        {
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        public PlayerSingleStat(float _baseValue) : this()
        {
            this.baseValue = _baseValue;

        }

        public float GetBaseValue()
        {
            return baseValue;
        }

        public void AddModifier(StatModifier modifier)
        {
            isDirty = true;
            statModifiers.Add(modifier);
            statModifiers.Sort(CompareModifierOrder);
        }

        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order > b.Order)
                return 1;
            else if (a.Order < b.Order)
                return -1;
            else
                return 0;
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (statModifiers.Remove(modifier))
            {
                isDirty = true;
                return true;
            }
            else
                return false;
        }

        public bool RemoveAllModifiersFromSource(object source)
        {
            bool didFind = false;
            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].Source == source)
                {
                    isDirty = true;
                    statModifiers.RemoveAt(i);
                    didFind = true;
                }
            }
            return didFind;
        }

        protected virtual float CalculateValue()
        {
            float finalValue = baseValue;
            float sumPercentAdd = 0;
            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier mod = statModifiers[i];

                if (mod.Type == ModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == ModType.PercentMult)
                {
                    finalValue *= 1 + mod.Value;
                }
                else
                {
                    sumPercentAdd += mod.Value;
                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != ModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }

            }
            return (float)Math.Round(finalValue, 4);
        }
    }
}
