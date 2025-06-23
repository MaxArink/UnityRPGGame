using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemy : Entity
{
    private IEnemy _enemy;

    private bool _isDead = false;
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

        //_availableSkills.Clear();
        //if (_character != null)
        //{
        //    _availableSkills.Add(_enemy.BasicSkill());
        //    _availableSkills.Add(_character.SkillOne());
        //    _availableSkills.Add(_character.SkillTwo());
        //    // Voeg meer toe als je meer skills hebt
        //}
    }

    public override void PerformAction()
    {
        base.PerformAction();
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
        }
        // Extra enemy-specifieke logica...
    }

    protected override void Die()
    {
        base.Die();
        _isDead = true;
    }
}
