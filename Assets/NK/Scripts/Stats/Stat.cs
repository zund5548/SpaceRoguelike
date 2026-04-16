using System;
using System.Collections.Generic;
using UnityEngine;
namespace Stats
{
    [Serializable]
    public class Stat
    {
        float baseValue;
        List<StatModifier> modifiers = new();
        public Stat(float baseValue)
        {
            this.baseValue = baseValue;
        }
        public float Value
        {
            get
            {
                float resultValue = baseValue;
                foreach (var mod in modifiers)
                {
                    if (mod.type == ModType.Add)resultValue += mod.value;
                }
                // foreach (var mod in modifiers)
                // {
                //     if (mod.type == ModType.Multiply)resultValue *= mod.value;
                // }
                float addedValue = resultValue;
                foreach (var mod in modifiers)
                {
                    if (mod.type == ModType.Percent)resultValue += addedValue * mod.value / 100f;
                }
                return resultValue;
            }
        }
        public void AddModifier(StatModifier mod)
        {
            modifiers.Add(mod);
        }
        public void RemoveModifier(object source)
        {
            modifiers.RemoveAll(m => m.source == source);
        }
    }
    [Serializable]
    public enum StatType
    {
        Power,
        Shield,
        Hull,
        ShieldResistance,
        HullResistance,
        ShotIntervalReduction,
    }
    [Serializable]
    public enum ModType
    {
        Add,
        //Multiply,
        Percent
    }
    [Serializable]
    public class StatModifier
    {
        public float value;
        public ModType type;
        public object source;
        public StatModifier(float value, ModType type, object source = null)
        {
            this.value = value;
            this.type = type;
            this.source = source;
        }
    }
}

