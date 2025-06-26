using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShadowKnight : MonoBehaviour
{
    private Enemy Enemy => GetComponent<Enemy>();
    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    public void InitializeSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            StrongStrike().ToSkill(),
            WeakenSlash().ToSkill(),
            DarkJab().ToSkill(),
            ShadowNova().ToSkill()
        };

        Enemy.SetSkills(skills);
    }

    public SkillHandler StrongStrike()
    {
        Skill s = new Skill
        {
            Name = "Strong Strike",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 1.2f
        };

        return new SkillHandler(
            s, 
            targets =>
            {
                if (targets.Count > 0)
                {
                    float dmg = _atk * s.Power;
                    targets[0].TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} raakt {targets[0].name} met Strong Strike voor {dmg} schade.");
                }
            });
    }

    public SkillHandler WeakenSlash()
    {
        Skill s = new Skill
        {
            Name = "Weaken Slash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SplashEnemy,
            Power = 0.7f,

            BuffType = StatModifier.StatType.Atk,
            BuffAmount = -0.1f,       // 10% vermindering
            BuffTurns = 2,            // bijvoorbeeld 2 beurten
            IsBuffPercent = true
        };

        return new SkillHandler(
            s,
            targets =>
            {
                foreach (Character t in targets)
                {
                    float dmg = _atk * s.Power;
                    t.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} verzwakt {t.name} met Weaken Slash voor {dmg} schade.");

                    // Debuff toepassen
                    StatModifier debuff = new StatModifier(
                        s.BuffType.Value,
                        s.BuffAmount,
                        s.BuffTurns,
                        s.IsBuffPercent
                    );

                    t.ApplyBuff(debuff);
                    Debug.Log($"{t.name} krijgt -10% Atk voor {s.BuffTurns} beurten.");
                }
            });
    }

    public SkillHandler DarkJab()
    {
        Skill s = new Skill
        {
            Name = "Dark Jab",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 1.0f,

            BuffTurns = 2, // Duur voor beide debuffs
            IsBuffPercent = true
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Character target)
                {
                    // Verlaag Speed
                    StatModifier speedDebuff = new StatModifier(
                        StatModifier.StatType.Speed,
                        -0.1f, // -10% Speed
                        s.BuffTurns,
                        s.IsBuffPercent
                    );

                    // Verlaag Defense
                    StatModifier defDebuff = new StatModifier(
                        StatModifier.StatType.Def,
                        -0.1f, // -10% Defense
                        s.BuffTurns,
                        s.IsBuffPercent
                    );

                    target.ApplyBuff(speedDebuff);
                    target.ApplyBuff(defDebuff);

                    float dmg = _atk * s.Power;
                    target.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} steekt {target.name} met Dark Jab voor {dmg} schade.");
                    Debug.Log($"{target.name} krijgt -10% Speed en -10% Defense voor {s.BuffTurns} beurten.");
                }
            });
    }

    public SkillHandler ShadowNova()
    {
        Skill s = new Skill
        {
            Name = "Shadow Nova",
            SkillType = SkillType.Attack,
            TargetType = TargetType.AOEEnemy,
            Power = 0.9f
        };

        return new SkillHandler(
            s, 
            targets =>
            {
                foreach (Character character in targets.OfType<Character>())
                {
                    float dmg = _atk * s.Power;
                    character.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} raakt {character.name} met Shadow Nova voor {dmg} schade.");
                }
            });
    }
}
