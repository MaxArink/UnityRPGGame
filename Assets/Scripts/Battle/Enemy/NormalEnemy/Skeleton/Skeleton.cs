using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();

    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    public void InitializeSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            BoneSlash().ToSkill(),
            CrackedStrike().ToSkill()
        };

        Enemy.SetSkills(skills);
    }

    public SkillHandler BoneSlash()
    {
        Skill s = new Skill
        {
            Name = "Bone Slash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.8f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Character target)
                {
                    float dmg = _atk * s.Power;
                    target.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} snijdt {target.name} met zijn botzwaard voor {dmg} schade.");
                }
            });
    }

    public SkillHandler CrackedStrike()
    {
        Skill s = new Skill
        {
            Name = "Cracked Strike",
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
                    Debug.Log($"{Enemy.name} ramt {target.name} met een zware bottenknal voor {dmg} schade.");
                }
            });
    }
}



