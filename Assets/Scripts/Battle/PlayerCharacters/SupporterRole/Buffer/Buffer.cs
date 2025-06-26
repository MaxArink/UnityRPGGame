using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Buffer : MonoBehaviour, ICharacter
{
    private Character _character => GetComponent<Character>();

    private float _atk => _character.Stats.GetStatValue(StatModifier.StatType.Atk);

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

    public SkillHandler BasicSkill()
    {
        Skill bufferBS = new Skill
        {
            Name = "Wack",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.6f
        };

        return new SkillHandler(
            bufferBS,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _atk * bufferBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    public SkillHandler SkillOne()
    {
        Skill bufferSO = new Skill
        {
            Name = "DEF Buff",
            SkillType = SkillType.Buff,
            TargetType = TargetType.AOEAlly,
            BuffType = StatModifier.StatType.Def,
            BuffAmount = 20f, // 20% DEF
            BuffTurns = 3,
            IsBuffPercent = true
        };

        return new SkillHandler(
            bufferSO,
            targets =>
            {
                foreach (Character ally in targets.OfType<Character>())
                {
                    StatModifier defBuff = new StatModifier(
                        StatModifier.StatType.Def,
                        bufferSO.BuffAmount,
                        bufferSO.BuffTurns, // turns
                        bufferSO.IsBuffPercent // procentueel
                    );

                    ally.ApplyBuff(defBuff);
                    Debug.Log($"{_character.name} geeft {ally.name} +{defBuff.Value} DEF voor {defBuff.Turns} beurten.");
                }
            });
    }

    public SkillHandler SkillTwo()
    {
        Skill bufferST = new Skill
        {
            Name = "ATK Buff",
            SkillType = SkillType.Buff,
            TargetType = TargetType.SingleAlly,
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = 25f, // 25% ATK
            BuffTurns = 2,
            IsBuffPercent = true
        };

        return new SkillHandler(
            bufferST,
            targets =>
            {
                foreach (Character ally in targets.OfType<Character>())
                {
                    StatModifier atkBuff = new StatModifier(
                        StatModifier.StatType.Atk,
                        bufferST.BuffAmount,
                        bufferST.BuffTurns,
                        bufferST.IsBuffPercent // procentueel
                    );
                    ally.ApplyBuff(atkBuff);
                    Debug.Log($"{_character.name} bufft {ally.name} met +{atkBuff.Value}% ATK voor {atkBuff.Turns} beurten.");
                }
            });
    }

    
}
