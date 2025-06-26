using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemy : Entity
{
    private IEnemy _enemy;

    private bool _isDead = false;

    private int _currentSkillIndex = 0;

    /// <summary>
    /// Kan denk ik wel een event zijn voor vernoetigen van zelf zodat hij ook uit de list met enemy's gehaalt wordt
    /// </summary>
    public override bool IsDead => _isDead;

    protected override void Awake()
    {
        base.Awake();
        _enemy = GetComponent<IEnemy>();
    }

    public override void Initialize(EntityData pData)
    {
        base.Initialize(pData);
    }

    public override void PerformAction()
    {
        if (Skills == null || Skills.Count == 0)
        {
            Debug.LogWarning($"{name} heeft geen skills.");
            BattleManager.Instance.EndTurn();
            return;
        }

        // Pak skill volgens volgorde (rondlopend)
        Skill chosenSkill = Skills[_currentSkillIndex];
        _currentSkillIndex = (_currentSkillIndex + 1) % Skills.Count;

        Debug.Log($"{name} gebruikt {chosenSkill.Name}");

        // TargetType omzetten naar TargetingType
        TargetingType targetingType = BattleManager.Instance.TargetingUtils.ConvertToTargetingType(chosenSkill.TargetType);

        // Bepaal of de skill allies target (Heal/Buff) of enemies
        bool targetsAllies = chosenSkill.SkillType == SkillType.Heal || chosenSkill.SkillType == SkillType.Buff;

        // Haal targets op via TargetingService
        List<Entity> targets = BattleManager.Instance.TargetingService.GetTargets(targetingType, this, targetsAllies);

        // Voer de skill uit via Entity's ExecuteSkill
        ExecuteSkill(chosenSkill, targets);

        BattleManager.Instance.EndTurn();
    }

    public override void TakeDamage(float pRawDamage)
    {
        // Bijvoorbeeld: vijanden hebben 10% minder schade van kritieke hits - nee enemy gaat waschijnlijk gewoon veel meer hp hebben dan characters
        float modifiedDamage = pRawDamage;
        base.TakeDamage((float)Math.Round(modifiedDamage));

        if (BaseStats.Hp <= 0 && !_isDead)
        {
            _isDead = true;
            Debug.Log($"{name} is verslagen!");
            Die();
        }
        // Extra enemy-specifieke logica...
    }

    protected override void Die()
    {
        base.Die();
        _isDead = true;
    }
}
