using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Aegis : MonoBehaviour, ICharacter
{
    // Shortcut naar de Character-component
    private Character _character => GetComponent<Character>();

    // Haal de Defense-stat op van de Character
    private float _def => _character.Stats.GetStatValue(StatModifier.StatType.Def);

    // Initialiseer de skills en wijs ze toe aan de Character
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

    /// <summary>
    /// Basisaanval: schade toebrengen gebaseerd op Defense
    /// </summary>
    /// <returns></returns>
    public SkillHandler BasicSkill()
    {
        Skill aegisBS = new Skill
        {
            Name = "Sanctified Slam",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.5f
        };

        return new SkillHandler(
            aegisBS,
            targets =>
            {
                // Controleer of er een geldig doel is en pas schade toe
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _def * aegisBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} met een heilige klap voor {dmg} schade.");
                }
            });
    }

    /// <summary>
    /// Buff voor meerdere bondgenoten: verhoogt defensie percentage voor een aantal beurten
    /// </summary>
    /// <returns></returns>
    public SkillHandler SkillOne()
    {
        Skill shieldAllies = new Skill
        {
            Name = "Guardian's Ward",
            SkillType = SkillType.Buff,
            TargetType = TargetType.AOEAlly,
            BuffType = StatModifier.StatType.Def,
            BuffAmount = 20f,
            BuffTurns = 2,
            IsBuffPercent = true
        };

        return new SkillHandler(
            shieldAllies,
            targets =>
            {
                // Voor elke bondgenoot, pas de defensie-buff toe
                foreach (Character ally in targets.OfType<Character>())
                {
                    StatModifier defBuff = new StatModifier(
                        shieldAllies.BuffType.Value,
                        shieldAllies.BuffAmount,
                        shieldAllies.BuffTurns,
                        shieldAllies.IsBuffPercent
                    );
                    ally.ApplyBuff(defBuff);
                    Debug.Log($"{_character.name} beschermt {ally.name} met +{defBuff.Value}% DEF voor {defBuff.Turns} beurten.");
                }
            });
    }

    /// <summary>
    /// Zelfbuff: verhoogt eigen defensie voor één beurt
    /// </summary>
    /// <returns></returns>
    public SkillHandler SkillTwo()
    {
        Skill selfBarrier = new Skill
        {
            Name = "Iron Wall",
            SkillType = SkillType.Buff,
            TargetType = TargetType.Self,
            BuffType = StatModifier.StatType.Def,
            BuffAmount = 60f, // Grote boost in Defense
            BuffTurns = 1,
            IsBuffPercent = true
        };

        return new SkillHandler(
            selfBarrier,
            targets =>
            {
                StatModifier defBoost = new StatModifier(
                    selfBarrier.BuffType.Value,
                    selfBarrier.BuffAmount,
                    selfBarrier.BuffTurns,
                    selfBarrier.IsBuffPercent
                );

                // Pas de buff toe op zichzelf
                _character.ApplyBuff(defBoost);
                Debug.Log($"{_character.name} wordt een ijzeren muur en krijgt +{defBoost.Value}% DEF voor {defBoost.Turns} beurt.");
            });
    }
}
