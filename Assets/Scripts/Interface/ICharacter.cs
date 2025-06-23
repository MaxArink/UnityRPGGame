using System;
using UnityEngine;

public interface ICharacter
{
    //Role Role { get; }
    //void CharacterInfo();
    void InitializeSkills();
    CharacterSkill BasicSkill();
    CharacterSkill SkillOne();
    CharacterSkill SkillTwo();
}
