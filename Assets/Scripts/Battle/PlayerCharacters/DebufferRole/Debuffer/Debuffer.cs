using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Debuffer : MonoBehaviour, ICharacter
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

    // Basisaanval: schade aan één vijand berekenen en toepassen
    public SkillHandler BasicSkill()
    {
        Skill debufferBS = new Skill
        {
            Name = "Bottle Throw",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.6f
        };

        return new SkillHandler(
            debufferBS,
            targets =>
            {
                // Check of er een geldig doel is en cast naar Enemy
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    float dmg = _atk * debufferBS.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    // Eerste vaardigheid: verlaagt aanvalskracht van meerdere vijanden (AOE debuff)
    public SkillHandler SkillOne()
    {
        Skill debuffSO = new Skill
        {
            Name = "Attack Down",
            SkillType = SkillType.Debuff,
            TargetType = TargetType.AOEEnemy,
            Power = 0.6f,
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = -30f,
            BuffTurns = 3,
            IsBuffPercent = true,
        };

        return new SkillHandler(
            debuffSO,
            targets =>
            {
                // Maak een aanval debuff aan met specifieke duur en waarde
                StatModifier atkDebuff = new StatModifier(
                    StatModifier.StatType.Atk,
                    debuffSO.BuffAmount,
                    debuffSO.BuffTurns,
                    debuffSO.IsBuffPercent
                );

                // Pas de debuff toe op elk doelwit en log dit
                foreach (Entity target in targets)
                {
                    target.ApplyBuff(atkDebuff);
                    Debug.Log($"{_character.name} debuffed {target.name} met {debuffSO.BuffAmount}% Atk voor 3 turns.");
                }
            });
    }

    // Tweede vaardigheid: verlaagt verdediging van meerdere vijanden (AOE debuff)
    public SkillHandler SkillTwo()
    {
        Skill debuffST = new Skill
        {
            Name = "Defense Down",
            SkillType = SkillType.Debuff,
            TargetType = TargetType.AOEEnemy,
            Power = 0.6f,
            BuffType = StatModifier.StatType.Def,
            BuffAmount = -25f,
            BuffTurns = 2,
            IsBuffPercent = true
        };

        return new SkillHandler(
            debuffST,
            targets =>
            {
                // Maak een verdediging debuff aan met specifieke duur en waarde
                StatModifier defDebuff = new StatModifier(
                    StatModifier.StatType.Def,
                    debuffST.BuffAmount,
                    debuffST.BuffTurns,
                    debuffST.IsBuffPercent
                );

                // Pas de debuff toe op elk doelwit en log dit
                foreach (Entity target in targets)
                {
                    target.ApplyBuff(defDebuff);
                    Debug.Log($"{_character.name} debuffed {target.name} met {defDebuff.Value}% Def voor 3 turns.");
                }
            });
    }
}
