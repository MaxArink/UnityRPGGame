using System.Collections.Generic;
using System;

public class SkillHandler
{
    public string Name => Skill.Name;
    public TargetingType TargetingType => BattleManager.Instance.TargetingUtils.ConvertToTargetingType(Skill.TargetType);
    public Action<List<Entity>> Action { get; }
    public Skill Skill { get; }

    public SkillHandler(Skill pSkill, Action<List<Entity>> pAction)
    {
        Skill = pSkill;
        Action = pAction;
    }

    public Skill ToSkill() => Skill;
}