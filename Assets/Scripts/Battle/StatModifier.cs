public struct StatModifier
{
    public enum StatType { Hp, Atk, Def, Speed }
    public StatType Type;
    public float Value;    // kan positief of negatief zijn
    public bool IsPercent; // bepaalt of Value absoluut of % is

    public StatModifier(StatType type, float value, bool isPercent = false)
    {
        Type = type;
        Value = value;
        IsPercent = isPercent;
    }
}
