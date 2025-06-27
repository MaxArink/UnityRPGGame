using System.Collections.Generic;
using UnityEngine;

public class Necromancer : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();

    // Haalt de aanvalskracht (ATK) van de necromancer op
    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    // Initialiseert de vaardigheden van de necromancer
    public void InitializeSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            DarkBolt().ToSkill(),
            SoulEmpower().ToSkill(),
            ShadowWave().ToSkill()
        };

        Enemy.SetSkills(skills);
    }

    // Enkele aanval met hoge kracht
    public SkillHandler DarkBolt()
    {
        Skill s = new Skill
        {
            Name = "Dark Bolt",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.9f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Character target)
                {
                    float dmg = _atk * s.Power;
                    target.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} schiet een donkere kogel op {target.name} voor {dmg} schade.");
                }
            });
    }

    // Verhoogt zijn eigen ATK tijdelijk
    public SkillHandler SoulEmpower()
    {
        Skill s = new Skill
        {
            Name = "Soul Empower",
            SkillType = SkillType.Buff,
            TargetType = TargetType.Self,
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = 20f, // 20% meer ATK
            BuffTurns = 3,
            IsBuffPercent = true
        };

        return new SkillHandler(
            s,
            targets =>
            {
                Enemy.ApplyBuff(new StatModifier(
                    s.BuffType.Value,
                    s.BuffAmount,
                    s.BuffTurns,
                    s.IsBuffPercent));
                Debug.Log($"{Enemy.name} versterkt zijn ziel, verhoogt aanval met {s.BuffAmount}% voor {s.BuffTurns} beurten.");
            });
    }

    // AOE aanval die meerdere vijanden raakt
    public SkillHandler ShadowWave()
    {
        Skill s = new Skill
        {
            Name = "Shadow Wave",
            SkillType = SkillType.Attack,
            TargetType = TargetType.AOEEnemy,
            Power = 0.5f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                float dmg = _atk * s.Power;

                foreach (Character enemy in targets)
                {
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} raakt {enemy.name} met een golf van schaduwen voor {dmg} schade.");
                }
            });
    }
}
