using System.Collections.Generic;
using System;

// Klasse die een Skill en bijbehorende actie koppelt voor eenvoudig gebruik
public class SkillHandler
{
    // Naam van de skill, doorgegeven via Skill object
    public string Name => Skill.Name;

    // Converteert TargetType van skill naar TargetingType voor de targeting logica
    public TargetingType TargetingType => BattleManager.Instance.TargetingUtils.ConvertToTargetingType(Skill.TargetType);

    // De actie die uitgevoerd wordt wanneer deze skill gebruikt wordt (met targets)
    public Action<List<Entity>> Action { get; }

    // Het Skill object zelf
    public Skill Skill { get; }

    // Constructor koppelt skill aan een bijbehorende actie
    public SkillHandler(Skill pSkill, Action<List<Entity>> pAction)
    {
        Skill = pSkill;
        Action = pAction;
    }

    // Retourneert het Skill object (voor eventuele directe toegang)
    public Skill ToSkill() => Skill;
}
