using UnityEngine;

public interface ICharacter
{
    Role Role { get; }
    void CharacterInfo();
    void BasicSkill();
    void SkillOne();
    void SkillTwo();
}
