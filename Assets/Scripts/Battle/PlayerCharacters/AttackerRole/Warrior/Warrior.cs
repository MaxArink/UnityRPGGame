using UnityEngine;

/// <summary>
/// Attacker is de kan ook de naam van de character zijn 
/// </summary>
public class Warrior : MonoBehaviour, ICharacter
{
    public void BasicSkill()
    {
        // single target attack that deals light damage based on ATK that does not consume mana
        Debug.Log("single target attack that deals light damage based on ATK that does not consume mana");
    }

    public void SkillOne()
    {
        
    }

    public void SkillTwo()
    {
        
    }
}
