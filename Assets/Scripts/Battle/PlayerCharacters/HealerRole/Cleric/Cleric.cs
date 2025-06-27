using UnityEngine;
using System.Collections.Generic;

public class Cleric : MonoBehaviour, ICharacter
{
    private Character _character => GetComponent<Character>();
    private float _hp => _character.Stats.GetStatValue(StatModifier.StatType.Hp);

    // Stel skills in
    public void InitializeSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            BasicSkill().ToSkill(),
            SkillOne().ToSkill(),
            SkillTwo().ToSkill()
        };
        _character.SetSkills(skills);
    }

    // Basis aanval
    public SkillHandler BasicSkill()
    {
        Skill clericBS = new Skill
        {
            Name = "Whack",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.3f
        };

        return new SkillHandler(
            clericBS,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _hp * clericBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    // Heal één bondgenoot
    public SkillHandler SkillOne()
    {
        Skill clericSO = new Skill
        {
            Name = "Heal",
            SkillType = SkillType.Heal,
            TargetType = TargetType.SingleAlly,
            Power = 1f
        };

        return new SkillHandler(
            clericSO,
            targets =>
            {
                if (targets.Count > 0)
                {
                    Entity ally = targets[0];
                    float healAmount = _hp * clericSO.Power;
                    ally.Heal(healAmount);
                    Debug.Log($"{_character.name} geneest {ally.name} voor {healAmount} HP.");
                }
            });
    }

    // Heal alle bondgenoten
    public SkillHandler SkillTwo()
    {
        Skill clericST = new Skill
        {
            Name = "Group Heal",
            SkillType = SkillType.Heal,
            TargetType = TargetType.AOEAlly,
            Power = 0.5f
        };

        return new SkillHandler(
            clericST,
            targets =>
            {
                float healAmount = _hp * clericST.Power;
                foreach (Entity ally in targets)
                {
                    ally.Heal(healAmount);
                    Debug.Log($"{_character.name} geneest {ally.name} voor {healAmount} HP.");
                }
            });
    }
}
