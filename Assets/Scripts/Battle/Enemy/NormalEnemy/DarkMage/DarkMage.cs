using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the DarkMage enemy, with splash and AOE magic attacks.
/// </summary>
public class DarkMage : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();

    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    public void InitializeSkills()
    {
        List<Skill> darkMageSkills = new List<Skill>
        {
            ShadowBolt().ToSkill(),
            DarkRuin().ToSkill()
        };

        Enemy.SetSkills(darkMageSkills);
    }

    /// <summary>
    /// Shadow Bolt: Hits main target for full damage and adjacent targets for reduced damage.
    /// </summary>
    public SkillHandler ShadowBolt()
    {
        Skill s = new Skill
        {
            Name = "Shadow Bolt",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SplashEnemy,
            Power = 0.7f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count == 0) return;

                for (int i = 0; i < targets.Count; i++)
                {
                    if (targets[i] is Character c)
                    {
                        float dmg = i == 0 ? _atk * s.Power : _atk * s.Power * 0.4f;
                        c.TakeDamage(dmg);
                        Debug.Log($"{Enemy.name} raakt {c.name} met Shadow Bolt voor {dmg} schade.");
                    }
                }
            });
    }

    /// <summary>
    /// Dark Ruin: Damages all enemies equally.
    /// </summary>
    public SkillHandler DarkRuin()
    {
        Skill s = new Skill
        {
            Name = "Dark Ruin",
            SkillType = SkillType.Attack,
            TargetType = TargetType.AOEEnemy,
            Power = 0.5f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                foreach (Character t in targets)
                {
                    if (t is Character c)
                    {
                        float dmg = _atk * s.Power;
                        c.TakeDamage(dmg);
                        Debug.Log($"{Enemy.name} ontketent Dark Ruin tegen {c.name} voor {dmg} schade.");
                    }
                }
            });
    }
}

