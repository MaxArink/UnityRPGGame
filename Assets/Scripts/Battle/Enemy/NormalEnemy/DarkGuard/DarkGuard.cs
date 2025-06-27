using System.Collections.Generic;
using UnityEngine;

public class DarkGuard : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();

    // Haalt de ATK waarde op uit de stat sheet van de enemy
    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    // Voegt de vaardigheden toe aan de enemy bij initialisatie
    public void InitializeSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            DarkSmash().ToSkill(),
            ShadowShield().ToSkill()
        };

        Enemy.SetSkills(skills);
    }

    // Aanval die schade doet aan één enkele vijand (speler)
    public SkillHandler DarkSmash()
    {
        Skill s = new Skill
        {
            Name = "Dark Smash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.7f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Character target)
                {
                    float dmg = _atk * s.Power;
                    target.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} slaat {target.name} met donkere kracht voor {dmg} schade.");
                }
            });
    }

    // Verhoogt eigen DEF tijdelijk met een percentage
    public SkillHandler ShadowShield()
    {
        Skill s = new Skill
        {
            Name = "Shadow Shield",
            SkillType = SkillType.Buff,
            TargetType = TargetType.Self,
            BuffType = StatModifier.StatType.Def,
            BuffAmount = 40f,  // 40% meer DEF
            BuffTurns = 3,
            IsBuffPercent = true
        };

        return new SkillHandler(
            s,
            targets =>
            {
                // Toepassen van de DEF buff op zichzelf
                Enemy.ApplyBuff(new StatModifier(
                    s.BuffType.Value,
                    s.BuffAmount,
                    s.BuffTurns,
                    s.IsBuffPercent));
                Debug.Log($"{Enemy.name} omhult zichzelf met een schaduwschild, +{s.BuffAmount}% DEF voor {s.BuffTurns} beurten.");
            });
    }
}
