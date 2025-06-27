public struct StatModifier
{
    public enum StatType { Hp, Atk, Def, Speed }
    public StatType Type;
    /// <summary>
    /// kan positief of negatief zijn
    /// </summary>
    public float Value;
    /// <summary>
    /// Voor hoeveel turns de buff actief is
    /// </summary>
    public int Turns;
    /// <summary>
    /// bepaalt of Value absoluut of % is
    /// </summary>
    public bool IsPercent;

    public StatModifier(StatType pType, float pValue, int pTurn,bool pIsPercent = false)
    {
        Type = pType;
        Value = pValue;
        Turns = pTurn;
        IsPercent = pIsPercent;
    }
}
