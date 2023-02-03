using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterStats
{
    public enum ModType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult= 300
    }

    [System.Serializable]
    public class StatModifier
    {
        [SerializeField]
        public readonly float Value;

        public ModType Type;

        public readonly int Order;

        public readonly object Source;

        public StatModifier(float _value, ModType type, int _order, object source)
        {
            this.Value = _value;
            Type = type;
            Order = _order;
            Source = source;
        }

        public StatModifier(float _value, ModType type) : this(_value, type, (int)type, null) { }
        public StatModifier(float _value, ModType type, int order) : this(_value, type, order, null) { }
        public StatModifier(float _value, ModType type, object source) : this(_value, type, (int)type, source) { }
    }
}
