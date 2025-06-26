using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Archer : MonoBehaviour, ICharacter
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

    public SkillHandler BasicSkill()
    {
        Skill archerBS = new Skill
        {
            Name = "Shot",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 1.0f
        };

        return new SkillHandler(
            archerBS,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _atk * archerBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    public SkillHandler SkillOne()
    {
        Skill archerSO = new Skill
        {
            Name = "Charge Up",
            SkillType = SkillType.Buff,
            TargetType = TargetType.Self,
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = 40f,
            BuffTurns = 1,
            IsBuffPercent = true
        };

        return new SkillHandler(
            archerSO,
            targets =>
            {
                StatModifier atkBuff = new StatModifier(
                    StatModifier.StatType.Atk,
                    archerSO.BuffAmount,
                    archerSO.BuffTurns,
                    archerSO.IsBuffPercent
                );

                _character.ApplyBuff(atkBuff);

                //foreach (Entity target in targets)
                //{
                //    target.ApplyBuff(atkBuff);
                //    Debug.Log($"{_character.name} bufft {target.name} met +{archerSO.BuffAmount}% ATK voor 3 turns.");
                //}
            });
    }

    public SkillHandler SkillTwo()
    {
        Skill archerST = new Skill
        {
            Name = "Arrow Drill",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 2.0f
        };

        return new SkillHandler(
            archerST,
            targets =>
            {
                float atk = _character.Stats.GetStatValue(StatModifier.StatType.Atk);
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = atk * archerST.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Arrow Drill voor {dmg} schade.");
                }
            });
    }
}
