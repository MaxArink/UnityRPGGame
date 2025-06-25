using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class Character : Entity
{
    private ICharacter _character;
    
    //private List<Skill> _skills;

    private bool _isDown = false;
    private bool _isDead = false;
    
    public bool IsDown => _isDown;
    public override bool IsDead => _isDead;

    public List<CharacterSkill> AvailableSkills = new List<CharacterSkill>();


    protected override void Awake()
    {
        base.Awake();
        _character = GetComponent<ICharacter>();
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

        Skill chosenSkill = Skills[UnityEngine.Random.Range(0, Skills.Count)];

        Debug.Log($"{name} gebruikt de skill {chosenSkill.Name}");

        TargetingType targetingType = BattleManager.Instance.TargetingUtils.ConvertToTargetingType(chosenSkill.TargetType);
        
        bool targetsAllies = chosenSkill.SkillType == SkillType.Heal || chosenSkill.SkillType == SkillType.Buff;

        List<Entity> targets = BattleManager.Instance.TargetingService.GetTargets(targetingType, this, targetsAllies);

        Debug.Log($"{name} targets voor {chosenSkill.Name}: {string.Join(", ", targets.ConvertAll(t => t.name))}");

        ExecuteSkill(chosenSkill, targets);

        BattleManager.Instance.EndTurn();

        //base.PerformAction();
    }

    public override void TakeDamage(float pRawDamage)
    {
        // Bijvoorbeeld: speler kan shield of buffs hebben die schade verder reduceren
        float modifiedDamage = pRawDamage; // Je kan hier eigen logica toevoegen

        // Eigen logica van mij
        base.TakeDamage((float)Math.Round(modifiedDamage));


        //if (BaseStats.Hp <= 0 && !_isDown)
        //{
        //    _isDown = true;
        //    Debug.Log($"{name} is verslagen!");

        //}

        if (BaseStats.Hp <= 0 && !_isDead)
        {
            _isDead = true;
            Debug.Log($"{name} is verslagen!");
        }
        // Extra character-specifieke logica...
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
