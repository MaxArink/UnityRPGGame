using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected BaseStats _baseStats;
    [SerializeField] protected EntityStats _characterStats = null;
    [SerializeField] protected Role _role = null;

    protected StatCalculator _stats;
    protected float _currentHp;

    public BaseStats BaseStats => _baseStats;
    public StatCalculator Stats => _stats;

    //// Level tijdelijk nog maar niet gebruiken: 29-5-2025
    //[SerializeField] private int _level = 1;
    //public int Level { get => _level; protected set => _level = value; }

    protected virtual void Awake()
    {
        _stats = new StatCalculator(_baseStats);

        _currentHp = _stats.GetStatValue(StatModifier.StatType.Hp);
    }

    public void Initialize(EntityData pData)
    {
        _baseStats = pData.BaseStats;
        _characterStats = pData.EntityStats;
        _role = pData.Role;
        name = pData.name;
    }

    // Maak de methode virtual
    public virtual void TakeDamage(float rawDamage)
    {
        float def = _stats.GetStatValue(StatModifier.StatType.Def);
        float damage = Mathf.Max(rawDamage - def, 1);
        _currentHp -= damage;

        Debug.Log($"{name} takes {damage} damage, HP left: {_currentHp}");

        if (_currentHp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} died.");
        // Standaard dood-logica
    }
}
