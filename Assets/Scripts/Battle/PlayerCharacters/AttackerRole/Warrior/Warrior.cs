using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Warrior : MonoBehaviour, ICharacter
{
    // Haalt de Character-component op van dit GameObject (lazy getter).
    private Character _character => GetComponent<Character>();

    // Haalt de actuele aanvalskracht van de character op, inclusief buffs/debuffs.
    private float _atk => _character.Stats.GetStatValue(StatModifier.StatType.Atk);

    public void InitializeSkills()
    {
        // Maakt een lijst met de skills van de Warrior en zet deze in de Character.
        List<Skill> skills = new List<Skill>
        {
            BasicSkill().ToSkill(),
            SkillOne().ToSkill(),
            SkillTwo().ToSkill()
        };
        _character.SetSkills(skills);
    }

    public SkillHandler BasicSkill()
    {
        // Basis aanval met power 1.0 (100% van de attack stat)
        Skill warriorBS = new Skill
        {
            Name = "Strike",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,  // Richt zich op één vijand
            Power = 1.0f
        };

        return new SkillHandler(
            warriorBS,
            targets =>
            {
                // Controleer of er minstens één target is en dat het een Enemy is
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    // Bereken schade en pas toe
                    float dmg = _atk * warriorBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    public SkillHandler SkillOne()
    {
        Skill warriorSO = new Skill
        {
            Name = "Slash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SplashEnemy,  // Meerdere vijanden raken
            Power = 1.0f
        };

        return new SkillHandler(
            warriorSO,
            targets =>
            {
                // Loop door alle vijanden en pas schade toe
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * warriorSO.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Slash voor {dmg}.");
                }
            });
    }

    public SkillHandler SkillTwo()
    {
        Skill warriorST = new Skill
        {
            Name = "Bigger Slash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SplashEnemy,  // Ook meerdere vijanden
            Power = 1.5f  // Sterkere aanval dan 'Slash'
        };

        return new SkillHandler(
            warriorST,
            targets =>
            {
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * warriorST.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Bigger Slash voor {dmg}.");
                }
            });
    }
}
