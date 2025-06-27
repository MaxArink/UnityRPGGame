using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkyPriest : MonoBehaviour, ICharacter
{
    private Character _character => GetComponent<Character>();
    private float _atk => _character.Stats.GetStatValue(StatModifier.StatType.Atk);

    // Initialiseer de skills en wijs ze toe aan het character
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

    // AOE aanval gebaseerd op ATK – richt schade aan alle vijanden
    public SkillHandler BasicSkill()
    {
        Skill skyBS = new Skill
        {
            Name = "Sky Blast",
            SkillType = SkillType.Attack,
            TargetType = TargetType.AOEEnemy,
            Power = 0.5f
        };

        return new SkillHandler(
            skyBS,
            targets =>
            {
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * skyBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Sky Blast voor {dmg} schade.");
                }
            });
    }

    // Verhoogt de Speed van alle allies voor meerdere beurten
    public SkillHandler SkillOne()
    {
        Skill skySO = new Skill
        {
            Name = "Wind Grace",
            SkillType = SkillType.Buff,
            TargetType = TargetType.AOEAlly,
            BuffType = StatModifier.StatType.Speed,
            BuffAmount = 15f, // 15% Speed boost
            BuffTurns = 3,
            IsBuffPercent = true
        };

        return new SkillHandler(
            skySO,
            targets =>
            {
                foreach (Character ally in targets.OfType<Character>())
                {
                    // Maak een speed buff aan en pas deze toe
                    StatModifier speedBuff = new StatModifier(
                        skySO.BuffType.Value,
                        skySO.BuffAmount,
                        skySO.BuffTurns,
                        skySO.IsBuffPercent
                    );

                    ally.ApplyBuff(speedBuff);
                    Debug.Log($"{_character.name} verhoogt {ally.name}'s Speed met {speedBuff.Value}% voor {speedBuff.Turns} beurten.");
                }
            });
    }

    // Boost de Speed van één ally voor 1 beurt met een vaste waarde
    public SkillHandler SkillTwo()
    {
        Skill skyST = new Skill
        {
            Name = "Haste Blessing",
            SkillType = SkillType.Buff,
            TargetType = TargetType.SingleAlly,
            BuffType = StatModifier.StatType.Speed,
            BuffAmount = 30f, // +30 Speed flat
            BuffTurns = 1,
            IsBuffPercent = false // geen percentage, maar vaste waarde
        };

        return new SkillHandler(
            skyST,
            targets =>
            {
                foreach (Character ally in targets.OfType<Character>())
                {
                    StatModifier speedBuff = new StatModifier(
                        skyST.BuffType.Value,
                        skyST.BuffAmount,
                        skyST.BuffTurns,
                        skyST.IsBuffPercent
                    );

                    ally.ApplyBuff(speedBuff);
                    Debug.Log($"{_character.name} geeft {ally.name} +{speedBuff.Value}% Speed voor {speedBuff.Turns} beurten.");
                }
            });
    }
}
