using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// Represents the Slime enemy type, implementing the IEnemy interface.  
/// Contains methods to initialize skills and define specific skill behaviors.  
/// </summary>  
public class Slime : MonoBehaviour, IEnemy
{
    /// <summary>  
    /// Gets the associated Enemy component.  
    /// </summary>  
    public Enemy Enemy => GetComponent<Enemy>();

    /// <summary>  
    /// Retrieves the attack stat value from the Enemy's stats.  
    /// </summary>  
    private float _atk => Enemy.Stats.GetStatValue(StatModifier.StatType.Atk);

    /// <summary>  
    /// Initializes the Slime's skills by creating and assigning them to the Enemy.  
    /// </summary>  
    public void InitializeSkills()
    {
        List<Skill> slimeSkills = new List<Skill>
        {
           SlimeBop().ToSkill(),
           SlimeDrip().ToSkill()
        };

        Enemy.SetSkills(slimeSkills);
    }

    /// <summary>  
    /// Defines the Slime Bop skill, which deals damage to a single enemy target.  
    /// </summary>  
    /// <returns>A SkillHandler for the Slime Bop skill.</returns>  
    public SkillHandler SlimeBop()
    {
        Skill s = new Skill
        {
            Name = "Slime Bop",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.6f
        };

        return new SkillHandler(
            s,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Character target)
                {
                    float dmg = _atk * s.Power;
                    target.TakeDamage(dmg);
                    Debug.Log($"{Enemy.name} botst tegen {target.name} aan voor {dmg} schade met Slime Bop.");
                }
            });
    }

    /// <summary>  
    /// Defines the Slime Drip skill, which deals damage to a single enemy target.  
    /// </summary>  
    /// <returns>A SkillHandler for the Slime Drip skill.</returns>  
    public SkillHandler SlimeDrip()
    {
        Skill s = new Skill
        {
            Name = "Slime Drip",
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
                    Debug.Log($"{Enemy.name} laat slijm op {target.name} druipen voor {dmg} schade met Slime Drip.");
                }
            });
    }
}

