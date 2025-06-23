using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Warrior : MonoBehaviour, ICharacter
{
    private Character _character => GetComponent<Character>();

    private float _atk => _character.Stats.GetStatValue(StatModifier.StatType.Atk);

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

    public CharacterSkill BasicSkill()
    {
        // Power 1 = 100%
        Skill warriorBS = new Skill
        {
            Name = "Strike",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 1.0f
        };

        return new CharacterSkill(
            warriorBS,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _atk * warriorBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    public CharacterSkill SkillOne()
    {
        Skill warriorSO = new Skill
        {
            Name = "Slash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SplashEnemy,
            Power = 1.0f
        };


        return new CharacterSkill(
            warriorSO,
            targets =>
            {
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * warriorSO.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Slash voor {dmg}.");
                }
            });
    }

    public CharacterSkill SkillTwo()
    {
        Skill warriorST = new Skill
        {
            Name = "Bigger Slash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SplashEnemy,
            Power = 1.5f
        };

        return new CharacterSkill(
            warriorST,
            targets =>
            {
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * warriorST.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Bigger Slash voor {dmg}.");
                }
            });
    }
}
