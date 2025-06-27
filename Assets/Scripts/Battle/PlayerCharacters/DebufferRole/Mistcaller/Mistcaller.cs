using System.Collections.Generic;
using UnityEngine;

public class Mistcaller : MonoBehaviour, ICharacter
{
    // Shortcut naar de Character-component
    private Character _character => GetComponent<Character>();

    // Haal de Attack-stat op van de Character
    private float _atk => _character.Stats.GetStatValue(StatModifier.StatType.Atk);

    // Initialiseer de vaardighedenlijst met de gedefinieerde skills
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

    // Basisaanval: richt schade aan één vijand toe
    public SkillHandler BasicSkill()
    {
        Skill basic = new Skill
        {
            Name = "Mist Bolt",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.7f
        };

        return new SkillHandler(
            basic,
            targets =>
            {
                // Controleer of er ten minste één doel is en cast deze naar Enemy
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _atk * basic.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Mist Bolt voor {dmg} schade.");
                }
            });
    }

    // Eerste vaardigheid: verlaagd snelheid van meerdere vijanden (AOE debuff)
    public SkillHandler SkillOne()
    {
        Skill slowMist = new Skill
        {
            Name = "Slowing Mist",
            SkillType = SkillType.Debuff,
            TargetType = TargetType.AOEEnemy,
            BuffType = StatModifier.StatType.Speed,
            BuffAmount = -25f, // -25% snelheid
            BuffTurns = 3,
            IsBuffPercent = true
        };

        return new SkillHandler(
            slowMist,
            targets =>
            {
                // Maak een speed debuff aan
                StatModifier slow = new StatModifier(
                    slowMist.BuffType.Value,
                    slowMist.BuffAmount,
                    slowMist.BuffTurns,
                    slowMist.IsBuffPercent
                );

                // Pas de debuff toe op elk doelwit en log de actie
                foreach (Entity target in targets)
                {
                    target.ApplyBuff(slow);
                    Debug.Log($"{_character.name} verlaagt de Speed van {target.name} met {slow.Value}% voor {slow.Turns} beurten.");
                }
            });
    }

    // Tweede vaardigheid: verzwakt verdediging van meerdere vijanden (AOE debuff)
    public SkillHandler SkillTwo()
    {
        Skill fogWeaken = new Skill
        {
            Name = "Armor Melt",
            SkillType = SkillType.Debuff,
            TargetType = TargetType.AOEEnemy,
            BuffType = StatModifier.StatType.Def,
            BuffAmount = -30f, // -30% verdediging
            BuffTurns = 2,
            IsBuffPercent = true
        };

        return new SkillHandler(
            fogWeaken,
            targets =>
            {
                // Maak een verdediging debuff aan
                StatModifier defDown = new StatModifier(
                    fogWeaken.BuffType.Value,
                    fogWeaken.BuffAmount,
                    fogWeaken.BuffTurns,
                    fogWeaken.IsBuffPercent
                );

                // Pas de debuff toe op elk doelwit en log de actie
                foreach (Entity target in targets)
                {
                    target.ApplyBuff(defDown);
                    Debug.Log($"{_character.name} verzwakt {target.name}'s verdediging met {defDown.Value}% voor {defDown.Turns} beurten.");
                }
            });
    }
}
