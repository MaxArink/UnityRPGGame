using UnityEngine;
using System.Collections.Generic;

public class Saint : MonoBehaviour, ICharacter
{
    private Character _character => GetComponent<Character>();
    private float _hp => _character.Stats.GetStatValue(StatModifier.StatType.Hp);

    // Initialiseert alle skills van de Saint en zet ze op het character
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

    // Basic skill – simpele aanval gebaseerd op HP
    public SkillHandler BasicSkill()
    {
        Skill s = new Skill
        {
            Name = "Light Strike",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.2f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _hp * s.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Light Strike voor {dmg} schade.");
                }
            });
    }

    // Skill om een enkele ally te healen of reviven
    public SkillHandler SkillOne()
    {
        Skill s = new Skill
        {
            Name = "Blessed Touch",
            SkillType = SkillType.Heal,
            TargetType = TargetType.SingleAlly,
            Power = 0.3f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                foreach (Entity ally in targets)
                {
                    float amount = _hp * s.Power;

                    if (ally.IsDead)
                    {
                        // Als de target dood is, revive met een deel van de HP
                        ally.Revive(amount);
                        Debug.Log($"{_character.name} herleeft {ally.name} met {amount} HP.");
                    }
                    else
                    {
                        // Anders gewoon heal
                        ally.Heal(amount);
                        Debug.Log($"{_character.name} geneest {ally.name} met {amount} HP.");
                    }
                }
            });
    }

    // AOE heal of revive voor alle allies
    public SkillHandler SkillTwo()
    {
        Skill s = new Skill
        {
            Name = "Group Blessing",
            SkillType = SkillType.Heal,
            TargetType = TargetType.AOEAlly,
            Power = 0.2f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                float amount = _hp * s.Power;

                foreach (Entity ally in targets)
                {
                    if (ally.IsDead)
                    {
                        ally.Revive(amount);
                        Debug.Log($"{_character.name} herleeft {ally.name} met {amount} HP.");
                    }
                    else
                    {
                        ally.Heal(amount);
                        Debug.Log($"{_character.name} geneest {ally.name} met {amount} HP.");
                    }
                }
            });
    }
}
