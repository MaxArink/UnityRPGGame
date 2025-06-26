using System;
using UnityEngine;

public interface ICharacter
{
    //Role Role { get; }
    //void CharacterInfo();
    void InitializeSkills();
    SkillHandler BasicSkill();
    SkillHandler SkillOne();
    SkillHandler SkillTwo();
}
