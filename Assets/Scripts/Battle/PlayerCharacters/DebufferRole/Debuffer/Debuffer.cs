using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Debuffer : MonoBehaviour, ICharacter
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

    public CharacterSkill BasicSkill()
    {
        Skill debufferBS = new Skill
        {
            Name = "Bottle Throw",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.6f
        };

        return new CharacterSkill(
            debufferBS,
            targets =>
            {
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _atk * debufferBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    public CharacterSkill SkillOne()
    {
        Skill debuffSO = new Skill
        {
            Name = "Attack Down",
            SkillType = SkillType.Buff,
            TargetType = TargetType.AOEEnemy,
            Power = 0.6f,
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = -30f,
            BuffTurns = 3,
            IsBuffPercent = true,
        };

        return new CharacterSkill(
            debuffSO,
            targets =>
            {
                StatModifier atkDebuff = new StatModifier(
                    StatModifier.StatType.Atk,
                    debuffSO.BuffAmount,
                    debuffSO.BuffTurns,     // turns
                    debuffSO.IsBuffPercent   // procentueel
                );

                foreach (Entity target in targets)
                {
                    target.ApplyBuff(atkDebuff);
                    Debug.Log($"{_character.name} debuffed {target.name} met {debuffSO.BuffAmount}% Atk voor 3 turns.");
                }
            });
    }

    public CharacterSkill SkillTwo()
    {
        Skill debuffST = new Skill
        {
            Name = "Defense Down",
            SkillType = SkillType.Buff,
            TargetType = TargetType.AOEEnemy,
            Power = 0.6f,
            BuffType = StatModifier.StatType.Def,
            BuffAmount = -25f,
            BuffTurns = 2,
            IsBuffPercent = true
        };

        return new CharacterSkill(
            debuffST,
            targets =>
            {
                StatModifier defDebuff = new StatModifier(
                    StatModifier.StatType.Def,
                    debuffST.BuffAmount,
                    debuffST.BuffTurns,     // turns
                    debuffST.IsBuffPercent   // procentueel
                );

                foreach (Entity target in targets)
                {
                    target.ApplyBuff(defDebuff);
                    Debug.Log($"{_character.name} debuffed {target.name} met {defDebuff.Value}% Def voor 3 turns.");
                }
            });
    }
}
