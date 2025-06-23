using UnityEngine;

[System.Serializable]
public class Skill
{
    public string Name;
    public SkillType SkillType;
    public TargetType TargetType;
    public float Power; // kan attack, heal, of buff waarde zijn
    public float Range; // voor splash

    // voor buffs
    public StatModifier.StatType? BuffType; // nullable, want niet elke skill bufft
    public float BuffAmount; // bijv. +10 def
    public int BuffTurns;
    public bool IsBuffPercent;
}