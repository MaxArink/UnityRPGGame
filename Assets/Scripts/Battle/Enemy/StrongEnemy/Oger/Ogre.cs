using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ogre enemy met drie verschillende aanvallen — richt zich puur op brute kracht.
/// </summary>
public class Ogre : MonoBehaviour, IEnemy
{
    // Referentie naar het bijbehorende Enemy-component
    public Enemy Enemy => GetComponent<Enemy>();

    // Verkrijg de ATK-waarde van de ogre uit zijn stats
    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    // Initialiseert alle vaardigheden van de ogre
    public void InitializeSkills()
    {
        List<Skill> ogreSkills = new List<Skill>
        {
            ClubSmash().ToSkill(),
            GroundStomp().ToSkill(),
            BoulderThrow().ToSkill()
        };

        Enemy.SetSkills(ogreSkills);
    }

    /// <summary>
    /// Club Smash – zware enkele aanval op één vijand.
    /// </summary>
    public SkillHandler ClubSmash()
    {
        Skill s = new Skill
        {
            Name = "Club Smash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.9f // Hoge power, gericht op 1 doelwit
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Character target)
                {
                    float dmg = _atk * s.Power;
                    target.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} slaat {target.name} met zijn knots voor {dmg} schade.");
                }
            });
    }

    /// <summary>
    /// Ground Stomp – raakt alle vijanden, gemiddelde kracht.
    /// </summary>
    public SkillHandler GroundStomp()
    {
        Skill s = new Skill
        {
            Name = "Ground Stomp",
            SkillType = SkillType.Attack,
            TargetType = TargetType.AOEEnemy,
            Power = 0.5f // Minder krachtig, maar raakt meerdere doelen
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
                        target.TakeDamage(dmg);
                        Debug.Log($"{Enemy.name} stampt {target.name} voor {dmg} schade.");
                    }
                }
            });
    }

    /// <summary>
    /// Boulder Throw – sterke aanval met rots, maar enkel doelwit.
    /// </summary>
    public SkillHandler BoulderThrow()
    {
        Skill s = new Skill
        {
            Name = "Boulder Throw",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 1.2f // Meest krachtige aanval van de ogre
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Character target)
                {
                    float dmg = _atk * s.Power;
                    target.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} werpt een rots op {target.name} voor {dmg} schade.");
                }
            });
    }
}
