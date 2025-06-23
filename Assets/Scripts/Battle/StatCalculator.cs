using System.Collections.Generic;
using UnityEngine;

public class StatCalculator
{
    private BaseStats _baseStats;
    private List<StatModifier> _modifiers = new List<StatModifier>();

    public StatCalculator(BaseStats pBaseStats)
    {
        _baseStats = pBaseStats;
    }

    public void Initialize(BaseStats pBaseStats)
    {
        _baseStats = pBaseStats;
    }

    public void AddModifier(StatModifier pModifier)
    {
        _modifiers.Add(pModifier);
    }

    public void RemoveModifier(StatModifier pModifier)
    {
        _modifiers.Remove(pModifier);
    }

    public float GetStatValue(StatModifier.StatType pType)
    {
        if (_baseStats == null)
            return 0f;

        float baseValue = pType switch
        {
            StatModifier.StatType.Hp => _baseStats.Hp,
            StatModifier.StatType.Atk => _baseStats.Atk,
            StatModifier.StatType.Def => _baseStats.Def,
            StatModifier.StatType.Speed => _baseStats.Speed,
            _ => 0
        };

        float flatBonus = 0f;
        float percentBonus = 0f;

        foreach (StatModifier mod in _modifiers)
        {
            if (mod.Type != pType)
                continue;

            if (mod.IsPercent)
                percentBonus += mod.Value;
            else
                flatBonus += mod.Value;
        }

        float value = baseValue + flatBonus;
        value *= (1 + percentBonus / 100f);

        return value;
    }

    public void TickModifiers()
    {
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            StatModifier mod = _modifiers[i];
            mod.Turns--;

            if (mod.Turns <= 0)
                _modifiers.RemoveAt(i);
            else
                _modifiers[i] = mod; // update struct met verminderde Turns
        }
    }
}
