using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mage : MonoBehaviour, ICharacter
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
        Skill mageBS = new Skill
        {
            Name = "Magic Bullet",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.6f
        };

        return new CharacterSkill(
            mageBS,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _atk * mageBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    public CharacterSkill SkillOne()
    {
        Skill mageSO = new Skill
        {
            Name = "Fire Ball",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SplashEnemy,
            Power = 0.8f
        };

        return new CharacterSkill(
            mageSO,
            targets =>
            {
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * mageSO.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Fire Ball voor {dmg} schade.");
                }
            });
    }

    public CharacterSkill SkillTwo()
    {
        Skill mageST = new Skill
        {
            Name = "Inferno",
            SkillType = SkillType.Attack,
            TargetType = TargetType.AOEEnemy,
            Power = 1.2f,  // sterke AOE aanval, bv 100% van ATK
            
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = 10f,
            BuffTurns = 2,
            IsBuffPercent = true
        };

        return new CharacterSkill(
            mageST,
            targets =>
            {
                StatModifier atkBuff = new StatModifier(
                    StatModifier.StatType.Atk,
                    mageST.BuffAmount,
                    mageST.BuffTurns,      // aantal turns
                    mageST.IsBuffPercent    // procentueel
                );

                _character.ApplyBuff(atkBuff);

                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * mageST.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Inferno voor {dmg} schade.");
                }
            });
    }
}
