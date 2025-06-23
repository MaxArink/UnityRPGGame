using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private EntityData _entityData;

    private BaseStats _baseStats;
    private EntityStats _characterStats;
    private Role _role;

    protected List<Skill> _skills = new List<Skill>();
    protected StatCalculator _stats;

    private float _currentHp;
    private float _maxHp;

    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;

    public StatCalculator Stats => _stats;
    public IReadOnlyList<Skill> Skills => _skills;

    public BaseStats BaseStats => _baseStats;
    public EntityStats CharacterStats => _characterStats;
    public Role Role => _role;

    public virtual bool IsDead => _currentHp <= 0;

    protected virtual void Awake()
    {
        if (_entityData != null)
            Initialize(_entityData);
    }

    public virtual void Initialize(EntityData pData)
    {
        _entityData = pData;
        _baseStats = pData.BaseStats;
        _characterStats = pData.EntityStats;
        _role = pData.Role;
        name = pData.Name;

        _stats = new StatCalculator(_baseStats);

        _maxHp = _stats.GetStatValue(StatModifier.StatType.Hp);
        _currentHp = _maxHp;

        _skills.Clear();
        // Skills kunnen hier later ingesteld worden via SetSkills of andere methode
    }

    public virtual void PerformAction()
    {
        if (IsDead)
        {
            Debug.Log($"{name} is dood en kan geen actie uitvoeren.");
            BattleManager.Instance.EndTurn();
            return;
        }

        Entity target = this is Character
            ? BattleManager.Instance.GetRandomAliveEnemy()
            : BattleManager.Instance.GetRandomAliveCharacter();

        if (target != null)
        {
            float damage = CalculateDamage(1f);
            Debug.Log($"{name} valt {target.name} aan voor {damage} schade.");
            target.TakeDamage(damage);
        }

        TickBuffs();
        BattleManager.Instance.EndTurn();
    }

    protected virtual void ExecuteSkill(Skill pSkill, List<Entity> pTargets)
    {
        if (pSkill == null)
        {
            Debug.LogWarning($"{name} probeert een lege skill te gebruiken.");
            return;
        }

        foreach (Entity target in pTargets)
        {
            if (target == null) continue;

            switch (pSkill.SkillType)
            {
                case SkillType.Attack:
                    // Power is een factor, bv 0.6 voor 60%
                    float damage = CalculateDamage(pSkill.Power);
                    target.TakeDamage(damage);
                    Debug.Log($"{name} valt {target.name} aan met {pSkill.Name} voor {damage} schade.");
                    break;

                case SkillType.Heal:
                    float healAmount = pSkill.Power;
                    target.Heal(healAmount);
                    Debug.Log($"{name} geneest {target.name} voor {healAmount} HP met {pSkill.Name}.");
                    break;

                case SkillType.Buff:
                    if (pSkill.BuffType.HasValue)
                    {
                        // Maak een StatModifier aan met power als waarde, en bv 3 turns en procentueel flag
                        StatModifier buff = new StatModifier(pSkill.BuffType.Value, pSkill.BuffAmount, pSkill.BuffTurns, pSkill.IsBuffPercent);
                        target.ApplyBuff(buff);
                        Debug.Log($"{name} bufft {target.name} met {pSkill.BuffType.Value} +{pSkill.BuffAmount}{(pSkill.IsBuffPercent ? "%" : "")} voor {pSkill.BuffTurns} beurt(en) via {pSkill.Name}.");
                    }
                    break;

                default:
                    Debug.LogWarning($"{name} gebruikt onbekende skill type {pSkill.SkillType}.");
                    break;
            }
        }
    }

    public virtual void ApplyBuff(StatModifier pModifier)
    {
        _stats.AddModifier(pModifier);
        Debug.Log($"{name} krijgt een buff: {pModifier.Type} {(pModifier.IsPercent ? $"{pModifier.Value}%" : pModifier.Value.ToString())} voor {pModifier.Turns} beurt(en).");
    }

    public virtual void TickBuffs()
    {
        _stats.TickModifiers();
    }

    protected float CalculateDamage(float pPowerMultiplier)
    {
        float atk = Stats.GetStatValue(StatModifier.StatType.Atk);
        return atk * pPowerMultiplier;
    }

    public virtual void TakeDamage(float pRawDamage)
    {
        float def = Stats.GetStatValue(StatModifier.StatType.Def);
        float flatReduction = Mathf.Floor(def / 5);
        float finalDamage = Mathf.Max(pRawDamage - flatReduction, 1f);

        _currentHp -= finalDamage;
        Debug.Log($"{name} neemt {finalDamage} schade, HP: {_currentHp}/{StatModifier.StatType.Hp}");

        if (_currentHp <= 0)
            Die();
    }

    public virtual void Heal(float pAmount)
    {
        _currentHp = Mathf.Min(_currentHp + pAmount, MaxHp);
        Debug.Log($"{_currentHp} en {CurrentHp}");
        Debug.Log($"{name} wordt geheeld voor {pAmount}, huidige HP: {_currentHp}/{StatModifier.StatType.Hp}");
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} is overleden.");
        gameObject.SetActive(false);

        //BattleManager.Instance?.RemoveFromTurnOrder(this);
    }

    public void SetSkills(List<Skill> pSkills)
    {
        _skills = pSkills ?? new List<Skill>();
    }
}
