using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();
    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    public void InitializeSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            GoblinStab().ToSkill(),
            FuriousSlash().ToSkill()
        };

        Enemy.SetSkills(skills);
    }

    private SkillHandler GoblinStab()
    {
        Skill skill = new Skill()
        {
            Name = "Goblin Stab",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.6f
        };

        return new SkillHandler(
            skill, 
            targets =>
            {
                if (targets.Count > 0)
                {
                    float damage = _atk * skill.Power;
                    targets[0].TakeDamage(damage);
                    Debug.Log($"{Enemy.name} steekt {targets[0].name} met Goblin Stab voor {damage} schade.");
                }
            });
    }

    private SkillHandler FuriousSlash()
    {
        Skill skill = new Skill()
        {
            Name = "Furious Slash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SplashEnemy, // raakt meerdere vijanden
            Power = 0.5f
        };

        return new SkillHandler(
            skill, 
            targets =>
            {
                foreach (Entity target in targets)
                {
                    float damage = _atk * skill.Power;
                    target.TakeDamage(damage);
                    Debug.Log($"{Enemy.name} raakt {target.name} met Furious Slash voor {damage} schade.");
                }
            });
    }
}
