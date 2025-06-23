using System.Collections.Generic;
using System;

public class CharacterSkill
{
    public string Name => Skill.Name;
    public TargetingType TargetingType => BattleManager.Instance.TargetingUtils.ConvertToTargetingType(Skill.TargetType);
    public Action<List<Entity>> Action { get; }
    public Skill Skill { get; }

    public CharacterSkill(Skill skill, Action<List<Entity>> action)
    {
        Skill = skill;
        Action = action;
    }

    public Skill ToSkill() => Skill;
}