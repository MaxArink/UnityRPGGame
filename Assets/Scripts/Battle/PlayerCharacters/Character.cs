using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class Character : Entity
{
    private ICharacter _character;

    private bool _isDown = false;
    private bool _isDead = false;

    public bool IsDown => _isDown;
    public override bool IsDead => _isDead;

    protected override void Awake()
    {
        base.Awake();
        // Haal de ICharacter component op voor extra karakter-functionaliteiten
        _character = GetComponent<ICharacter>();
    }

    public override void Initialize(EntityData pData)
    {
        base.Initialize(pData);
        // Hier kan aanvullende initialisatie als nodig
    }

    public override void PerformAction()
    {
        if (Skills == null || Skills.Count == 0)
        {
            Debug.LogWarning($"{name} heeft geen skills.");
            BattleManager.Instance.EndTurn();
            return;
        }

        BattleUIManager.Instance.ShowSkillOptions(this);
    }

    public void PerformSkill(Skill pSkill, List<Entity> pTargets)
    {
        ExecuteSkill(pSkill, pTargets);
        Debug.Log($"{name} gebruikt de skill {pSkill.Name}");
        TickBuffs();
        BattleManager.Instance.EndTurn();
    }

    public override void TakeDamage(float pRawDamage)
    {
        float modifiedDamage = pRawDamage; // Hier kun je logica toevoegen voor schade-aanpassingen

        base.TakeDamage((float)Math.Round(modifiedDamage));

        // Check of character dood is en update status
        if (BaseStats.Hp <= 0 && !_isDead)
        {
            _isDead = true;
            Debug.Log($"{name} is verslagen!");
        }
    }

    protected override void Die()
    {
        base.Die();
        _isDead = true;
    }

    public Skill GetRandomSkill()
    {
        if (Skills == null || Skills.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, Skills.Count);
        return Skills[index];
    }

    public List<Skill> GetSkills()
    {
        return _skills;
    }
}
