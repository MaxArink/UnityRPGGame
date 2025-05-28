using UnityEngine;

public class Defender : MonoBehaviour, ICharacter
{
    [SerializeField] private CharacterStats _stats = null;
    [SerializeField] private Role _role = null;

    public Role Role { get => _role; }

    public void CharacterInfo()
    {
        Debug.Log($"Character: {_stats}, Role: {_role},\n Stats: HP.{_stats.Hp}, MP.{_stats.Mp}, ATK.{_stats.Atk}, DEF.{_stats.Def}, Speed.{_stats.Speed}, Taunt.{_role.Taunt}");
    }

    public void BasicSkill()
    {
        //
    }

    public void SkillOne()
    {
        //
    }

    public void SkillTwo()
    {
        //
    }
}
