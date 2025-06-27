using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Wraith enemy – gebruikt donkere energie om vijanden aan te vallen.
/// Geen buffs of debuffs, enkel magische schade.
/// </summary>
public class Wraith : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();
    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    public void InitializeSkills()
    {
        List<Skill> wraithSkills = new List<Skill>
        {
            SoulPierce().ToSkill(),
            ShadowBurst().ToSkill()
        };

        Enemy.SetSkills(wraithSkills);
    }

    /// <summary>
    /// Soul Pierce – een geconcentreerde aanval op één doelwit.
    /// </summary>
    public SkillHandler SoulPierce()
    {
        Skill s = new Skill
        {
            Name = "Soul Pierce",
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
                    Debug.Log($"{Enemy.name} doorboort de ziel van {target.name} voor {dmg} schade.");
                }
            });
    }

    /// <summary>
    /// Shadow Burst – explodeert in een ring van duisternis, raakt alle vijanden.
    /// </summary>
    public SkillHandler ShadowBurst()
    {
        Skill s = new Skill
        {
            Name = "Shadow Burst",
            SkillType = SkillType.Debuff, // Aangegeven als debuff vanwege effect
            TargetType = TargetType.AOEEnemy,
            Power = 0.7f,
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = -20f,     // Vermindert ATK met 20%
            BuffTurns = 2,
            IsBuffPercent = true
        };

        return new SkillHandler(
            s,
            targets =>
            {
                float dmg = _atk * s.Power;
                foreach (Character t in targets)
                {
                    if (t is Character target)
                    {
                        // Schade toebrengen
                        target.TakeDamage(dmg);

                        // Debuff toepassen
                        StatModifier atkDebuff = new StatModifier(
                            s.BuffType.Value,
                            s.BuffAmount,
                            s.BuffTurns,
                            s.IsBuffPercent
                        );
                        target.ApplyBuff(atkDebuff);

                        Debug.Log($"{Enemy.name} barst in schaduwen uit en raakt {target.name} voor {dmg} schade, ATK -{Mathf.Abs(s.BuffAmount)}% voor {s.BuffTurns} beurten.");
                    }
                }
            });
    }

}



