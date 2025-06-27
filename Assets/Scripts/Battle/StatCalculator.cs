using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Verwerkt statberekening met tijdelijke buffs/debuffs (StatModifiers).
/// </summary>
public class StatCalculator
{
    private BaseStats _baseStats; // Basiswaarden zoals HP, ATK, etc.
    private List<StatModifier> _modifiers = new List<StatModifier>(); // Actieve modificaties

    public StatCalculator(BaseStats pBaseStats)
    {
        _baseStats = pBaseStats;
    }

    /// <summary>
    /// Herinitialiseert met andere base stats.
    /// </summary>
    public void Initialize(BaseStats pBaseStats)
    {
        _baseStats = pBaseStats;
    }

    /// <summary>
    /// Voeg een nieuwe modifier toe (buff/debuff).
    /// </summary>
    public void AddModifier(StatModifier pModifier)
    {
        _modifiers.Add(pModifier);
    }

    /// <summary>
    /// Verwijdert een specifieke modifier.
    /// </summary>
    public void RemoveModifier(StatModifier pModifier)
    {
        _modifiers.Remove(pModifier);
    }

    /// <summary>
    /// Haalt de huidige waarde van een stat op, inclusief modifiers.
    /// </summary>
    public float GetStatValue(StatModifier.StatType pType)
    {
        if (_baseStats == null)
            return 0f;

        // Bepaal basiswaarde per stat type
        float baseValue = pType switch
        {
            StatModifier.StatType.Hp => _baseStats.Hp,
            StatModifier.StatType.Atk => _baseStats.Atk,
            StatModifier.StatType.Def => _baseStats.Def,
            StatModifier.StatType.Speed => _baseStats.Speed,
            _ => 0
        };

        float flatBonus = 0f;      // +5 ATK bijvoorbeeld
        float percentBonus = 0f;   // +10% ATK bijvoorbeeld

        // Tel modifiers op per type
        foreach (StatModifier mod in _modifiers)
        {
            if (mod.Type != pType)
                continue;

            if (mod.IsPercent)
                percentBonus += mod.Value;
            else
                flatBonus += mod.Value;
        }

        // Bereken uiteindelijke waarde
        float value = baseValue + flatBonus;
        value *= (1 + percentBonus / 100f);

        return value;
    }

    /// <summary>
    /// Laat 1 beurt verstrijken voor alle modifiers (vermindert Turns). Verwijdert verlopen buffs.
    /// </summary>
    public void TickModifiers()
    {
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            StatModifier mod = _modifiers[i];
            mod.Turns--;

            if (mod.Turns <= 0)
                _modifiers.RemoveAt(i);
            else
                _modifiers[i] = mod; // Structs zijn immutable, dus opnieuw instellen
        }
    }
}
