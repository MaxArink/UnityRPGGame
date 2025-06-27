using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Archer : MonoBehaviour, ICharacter
{
    // Verkrijg referentie naar de Character component op hetzelfde GameObject
    private Character _character => GetComponent<Character>();

    // Verkrijg de actuele Attack waarde van de character
    private float _atk => _character.Stats.GetStatValue(StatModifier.StatType.Atk);

    // Initialiseer en wijs een lijst van skills toe aan de character
    public void InitializeSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            BasicSkill().ToSkill(),
            SkillOne().ToSkill(),
            SkillTwo().ToSkill()
        };
        _character.SetSkills(skills);
    }

    // Basis aanval: een enkele vijand raakt met normale schade
    public SkillHandler BasicSkill()
    {
        Skill archerBS = new Skill
        {
            Name = "Shot",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 1.0f
        };

        // Definieer de actie die uitgevoerd wordt bij gebruik van deze skill
        return new SkillHandler(
            archerBS,
            targets =>
            {
                // Check of er een target is en dat het een Enemy is
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _atk * archerBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    // Skill die de eigen Attack verhoogt (buff) voor een aantal beurten
    public SkillHandler SkillOne()
    {
        Skill archerSO = new Skill
        {
            Name = "Charge Up",
            SkillType = SkillType.Buff,
            TargetType = TargetType.Self,
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = 40f,
            BuffTurns = 1,
            IsBuffPercent = true
        };

        return new SkillHandler(
            archerSO,
            targets =>
            {
                // Maak een buff aan die de attack met 40% verhoogt voor 1 beurt
                StatModifier atkBuff = new StatModifier(
                    StatModifier.StatType.Atk,
                    archerSO.BuffAmount,
                    archerSO.BuffTurns,
                    archerSO.IsBuffPercent
                );

                // Pas de buff toe op de eigen character
                _character.ApplyBuff(atkBuff);
            });
    }

    // Skill die meerdere vijanden raakt met verhoogde schade
    public SkillHandler SkillTwo()
    {
        Skill archerST = new Skill
        {
            Name = "Arrow Drill",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 2.0f
        };

        return new SkillHandler(
            archerST,
            targets =>
            {
                // Loop door alle vijanden in de targetlijst en richt schade toe
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * archerST.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Arrow Drill voor {dmg} schade.");
                }
            });
    }
}
