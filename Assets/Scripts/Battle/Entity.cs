using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action<Entity> OnDie;

    [SerializeField] private EntityData _entityData;

    private BaseStats _baseStats;
    private EntityStats _characterStats;
    private Role _role;

    protected StatCalculator _stats;
    protected List<Skill> _skills = new List<Skill>();

    private float _currentHp;
    private float _maxHp;

    private List<Entity> _adjacentAllies = new List<Entity>();

    public BaseStats BaseStats => _baseStats;
    public EntityStats CharacterStats => _characterStats;
    public Role Role => _role;

    public StatCalculator Stats => _stats;
    public IReadOnlyList<Skill> Skills => _skills;

    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;
    public virtual bool IsDead => _currentHp <= 0;

    public List<Entity> AdjacentAllies => _adjacentAllies;

    protected virtual void Awake()
    {
        // Als EntityData aanwezig is, initialiseer deze entity automatisch
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

        // HP initialiseren op max waarde
        _maxHp = _stats.GetStatValue(StatModifier.StatType.Hp);
        _currentHp = _maxHp;

        _skills.Clear();
        // Skills kunnen later via SetSkills toegevoegd worden
    }

    public virtual void PerformAction()
    {
        // Indien dood, sla beurt over en eindig beurt
        if (IsDead)
        {
            BattleManager.Instance.EndTurn();
            return;
        }

        // Kies target op basis van type Entity (Character of Enemy)
        Entity target = this is Character
            ? BattleManager.Instance.GetRandomAliveEnemy()
            : BattleManager.Instance.GetRandomAliveCharacter();

        if (target != null)
        {
            // Bereken en pas schade toe
            float damage = CalculateDamage(1f, StatModifier.StatType.Atk);
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
                    // Schade berekenen en toepassen op target
                    float damage = CalculateDamage(pSkill.Power, pSkill.BaseStatForDamage);
                    target.TakeDamage(damage);
                    Debug.Log($"{name} valt {target.name} aan met {pSkill.Name} voor {damage} schade.");
                    break;

                case SkillType.Heal:
                    // Genezen van target
                    float healAmount = CalculateHeal(pSkill.Power, pSkill.BaseStatForDamage);
                    target.Heal(healAmount);
                    Debug.Log($"{name} geneest {target.name} voor {healAmount} HP met {pSkill.Name}.");
                    break;

                case SkillType.Buff:
                    if (pSkill.BuffType.HasValue)
                    {
                        // Buff toepassen met gegeven parameters
                        StatModifier buff = new StatModifier(pSkill.BuffType.Value, pSkill.BuffAmount, pSkill.BuffTurns, pSkill.IsBuffPercent);
                        target.ApplyBuff(buff);
                        Debug.Log($"{name} bufft {target.name} met {pSkill.BuffType.Value} +{pSkill.BuffAmount}{(pSkill.IsBuffPercent ? "%" : "")} voor {pSkill.BuffTurns} beurt(en) via {pSkill.Name}.");
                    }
                    break;

                case SkillType.Debuff:
                    if (pSkill.BuffType.HasValue)
                    {
                        // Debuff toepassen (negatieve buff)
                        StatModifier debuff = new StatModifier(pSkill.BuffType.Value, -Mathf.Abs(pSkill.BuffAmount), pSkill.BuffTurns, pSkill.IsBuffPercent);
                        target.ApplyBuff(debuff);
                        Debug.Log($"{name} geeft een debuff aan {target.name}: {pSkill.BuffType.Value} {debuff.Value} ({pSkill.BuffTurns} beurten)");
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
        // Laat buffs/debuffs aftellen per beurt
        _stats.TickModifiers();
    }

    protected float CalculateDamage(float pPowerMultiplier, StatModifier.StatType pBaseStat)
    {
        float baseValue = Stats.GetStatValue(pBaseStat);
        return baseValue * pPowerMultiplier;
    }

    protected float CalculateHeal(float pPowerMultiplier, StatModifier.StatType pBaseStat)
    {
        float baseValue = Stats.GetStatValue(pBaseStat);
        return baseValue * pPowerMultiplier;
    }

    public virtual void TakeDamage(float pRawDamage)
    {
        // Defensie vermindert de schade volgens een formule
        float def = Stats.GetStatValue(StatModifier.StatType.Def);
        float defMultiplier = 1f - (def / (def + 200f));
        float finalDamage = Mathf.Max(Mathf.Floor(pRawDamage * defMultiplier), 1f);

        _currentHp -= finalDamage;
        Debug.Log($"{name} neemt {finalDamage} schade, HP: {_currentHp}/{StatModifier.StatType.Hp}");

        if (_currentHp <= 0)
            Die();
    }

    public virtual void Heal(float pAmount)
    {
        // Geneest, maar niet boven max HP
        _currentHp = Mathf.Min(_currentHp + pAmount, MaxHp);
        Debug.Log($"{name} wordt geheeld voor {pAmount}, huidige HP: {_currentHp}/{StatModifier.StatType.Hp}");
    }

    public virtual void Revive(float hpAmount)
    {
        if (!IsDead) return;

        float maxHp = Stats.GetStatValue(StatModifier.StatType.Hp);
        _currentHp = Mathf.Min(hpAmount, maxHp);

        // Visuele effecten kunnen hier toegevoegd worden
        Debug.Log($"{name} is herrezen met {CurrentHp} HP.");
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} is overleden.");
        gameObject.SetActive(false);

        OnDie.Invoke(this);
    }

    public void SetSkills(List<Skill> pSkills)
    {
        _skills = pSkills ?? new List<Skill>();
    }

    // ** Wordt momenteel niet gebruikt, maar houdt buren up-to-date voor bv buff targeting **
    public void UpdateAdjacents(List<Entity> pAllAllies)
    {
        AdjacentAllies.Clear();

        // Voeg buren toe binnen bepaalde afstand, excl. zichzelf en doden
        float maxDistance = 2.0f;

        foreach (Entity ally in pAllAllies)
        {
            if (ally == this) continue;
            if (Vector3.Distance(transform.position, ally.transform.position) <= maxDistance && !ally.IsDead)
            {
                AdjacentAllies.Add(ally);
            }
        }
    }
}
