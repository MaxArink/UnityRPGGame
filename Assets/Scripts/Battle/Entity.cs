using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
            BattleManager.Instance.EndTurn();
            return;
        }

        Entity target = this is Character
            ? BattleManager.Instance.GetRandomAliveEnemy()
            : BattleManager.Instance.GetRandomAliveCharacter();

        if (target != null)
        {
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
                    // Power is een factor, bv 0.6 voor 60%
                    float damage = CalculateDamage(pSkill.Power, pSkill.BaseStatForDamage);
                    target.TakeDamage(damage);
                    Debug.Log($"{name} valt {target.name} aan met {pSkill.Name} voor {damage} schade.");
                    break;
                case SkillType.Heal:
                    float healAmount = CalculateHeal(pSkill.Power, pSkill.BaseStatForDamage/*, skill.IsPercentHeal*/);
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

                case SkillType.Debuff:
                    if (pSkill.BuffType.HasValue)
                    {
                        StatModifier debuff = new StatModifier(pSkill.BuffType.Value, -Mathf.Abs(pSkill.BuffAmount), pSkill.BuffTurns, pSkill.IsBuffPercent);

                        target.ApplyBuff(debuff); // of target.ApplyDebuff(modifier) als je die apart hebt
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
        _stats.TickModifiers();
    }

    protected float CalculateDamage(float pPowerMultiplier, StatModifier.StatType pBaseStat)
    {
        float baseValue = Stats.GetStatValue(pBaseStat);
        return baseValue * pPowerMultiplier;
    }

    protected float CalculateHeal(float pPowerMultiplier, StatModifier.StatType pBaseStat/*, bool pIsPercent*/)
    {
        float baseValue = Stats.GetStatValue(pBaseStat);
        return baseValue * pPowerMultiplier;// als ik dat andere wil toeveogen kan dat

        //if (pIsPercent)
        //{
        //    // Bijvoorbeeld: 0.2f = 20% van baseValue
        //    return baseValue * pPowerMultiplier;
        //}
        //else
        //{
        //    // Bijvoorbeeld: 1.5f = flat power factor op baseValue
        //    return baseValue * pPowerMultiplier;
        //}
    }

    public virtual void TakeDamage(float pRawDamage)
    {
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
        _currentHp = Mathf.Min(_currentHp + pAmount, MaxHp);
        Debug.Log($"{name} wordt geheeld voor {pAmount}, huidige HP: {_currentHp}/{StatModifier.StatType.Hp}");
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} is overleden.");
        gameObject.SetActive(false);

        OnDie.Invoke(this);
        //OnDie=null;
        //BattleManager.Instance?.RemoveFromTurnOrder(this);
    }

    public void SetSkills(List<Skill> pSkills)
    {
        _skills = pSkills ?? new List<Skill>();
    }

    // Helper om buren up-to-date te houden
    public void UpdateAdjacents(List<Entity> pAllAllies)
    {
        AdjacentAllies.Clear();

        // Simpel voorbeeld: voeg buren toe die 'naast' deze entity staan
        // Je bepaalt hier de logica, bijvoorbeeld afstand of custom rules

        float maxDistance = 2.0f; // bv max 2 units afstand om als buur te tellen

        foreach (var ally in pAllAllies)
        {
            if (ally == this) continue;
            if (Vector3.Distance(transform.position, ally.transform.position) <= maxDistance && !ally.IsDead)
            {
                AdjacentAllies.Add(ally);
            }
        }
    }
}
