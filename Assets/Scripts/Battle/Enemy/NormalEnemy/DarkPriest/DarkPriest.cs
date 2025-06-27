using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Represents the DarkPriest enemy, with one single-target attack and one single-target buff.
/// </summary>
public class DarkPriest : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();
    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    public void InitializeSkills()
    {
        List<Skill> darkPriestSkills = new List<Skill>
        {
            DarkStrike().ToSkill(),
            UnholyBlessing().ToSkill()
        };

        Enemy.SetSkills(darkPriestSkills);
    }

    /// <summary>
    /// Dark Strike: A strong single-target attack.
    /// </summary>
    public SkillHandler DarkStrike()
    {
        Skill s = new Skill
        {
            Name = "Dark Strike",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 1.0f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Character target)
                {
                    float dmg = _atk * s.Power;
                    target.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} valt {target.name} aan met Dark Strike voor {dmg} schade.");
                }
            });
    }

    /// <summary>
    /// Unholy Blessing: Buffs an ally's attack stat.
    /// </summary>
    public SkillHandler UnholyBlessing()
    {
        Skill s = new Skill
        {
            Name = "Unholy Blessing",
            SkillType = SkillType.Buff,
            TargetType = TargetType.SingleAlly,
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = 20f,         // +20% ATK
            BuffTurns = 2,
            IsBuffPercent = true
        };

        return new SkillHandler(
            s,
            targets =>
            {
                foreach (Enemy t in targets)
                {
                    if (t is Enemy ally)
                    {
                        StatModifier buff = new StatModifier(
                            s.BuffType.Value,
                            s.BuffAmount,
                            s.BuffTurns,
                            s.IsBuffPercent
                        );
                        ally.ApplyBuff(buff);
                        Debug.Log($"{Enemy.name} versterkt {ally.name} met Unholy Blessing (+{s.BuffAmount}% ATK).");
                    }
                }
            });
    }
}


