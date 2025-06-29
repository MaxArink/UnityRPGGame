using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour, ICharacter
{
    // Shortcut naar Character-component
    private Character _character => GetComponent<Character>();

    // Haal de Defense-stat op van de Character
    private float _def => _character.Stats.GetStatValue(StatModifier.StatType.Def);

    // Initialiseer en wijs de skills toe aan de Character
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

    // Basisaanval: schade gebaseerd op defense aan ��n vijand
    public SkillHandler BasicSkill()
    {
        Skill tankBS = new Skill
        {
            Name = "Shield Bash",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.6f
        };

        return new SkillHandler(
            tankBS,
            targets =>
            {
                // Check of er een doel is en breng schade toe
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _def * tankBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} ramt {enemy.name} met een schild voor {dmg} schade.");
                }
            });
    }

    // Buff skill: verhoogt eigen defense voor enkele beurten
    public SkillHandler SkillOne()
    {
        Skill tankSO = new Skill
        {
            Name = "Fortify",
            SkillType = SkillType.Buff,
            TargetType = TargetType.Self,
            BuffType = StatModifier.StatType.Def,
            BuffAmount = 30f,
            BuffTurns = 2,
            IsBuffPercent = true
        };

        return new SkillHandler(
            tankSO,
            targets =>
            {
                StatModifier buff = new StatModifier(
                    tankSO.BuffType.Value,
                    tankSO.BuffAmount,
                    tankSO.BuffTurns,
                    tankSO.IsBuffPercent
                );

                // Pas de buff toe op zichzelf
                _character.ApplyBuff(buff);
                Debug.Log($"{_character.name} versterkt zichzelf met +{tankSO.BuffAmount} DEF voor {tankSO.BuffTurns} beurten.");
            });
    }

    // AOE aanval: schade gebaseerd op defense aan alle vijanden in gebied
    public SkillHandler SkillTwo()
    {
        Skill tankST = new Skill
        {
            Name = "Shockwave",
            SkillType = SkillType.Attack,
            TargetType = TargetType.AOEEnemy,
            Power = 0.4f
        };

        return new SkillHandler(
            tankST,
            targets =>
            {
                float dmg = _def * tankST.Power;
                // Breng schade toe aan alle doelen in de lijst
                foreach (Entity enemy in targets)
                {
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met een schokgolf voor {dmg} schade.");
                }
            });
    }
}
