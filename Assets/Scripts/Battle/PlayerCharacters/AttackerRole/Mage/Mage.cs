using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mage : MonoBehaviour, ICharacter
{
    // Haalt de Character-component op van hetzelfde GameObject.
    private Character _character => GetComponent<Character>();

    // Haalt de aanvalskracht (Atk) statwaarde op uit de Character stats.
    private float _atk => _character.Stats.GetStatValue(StatModifier.StatType.Atk);

    public void InitializeSkills()
    {
        // Maakt een lijst met skills en zet deze in de Character.
        List<Skill> skills = new List<Skill>
        {
            BasicSkill().ToSkill(),     // Zet SkillHandler om naar Skill
            SkillOne().ToSkill(),
            SkillTwo().ToSkill()
        };
        _character.SetSkills(skills);
    }

    public SkillHandler BasicSkill()
    {
        Skill mageBS = new Skill
        {
            Name = "Magic Bullet",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SingleEnemy,
            Power = 0.6f    // Vermenigvuldigingsfactor voor schade
        };

        return new SkillHandler(
            mageBS,
            targets =>
            {
                // Controleert of er ten minste één target is en dat het een Enemy is
                if (targets.Count > 0 && targets[0] is Enemy enemy)
                {
                    // Bereken schade op basis van aanvalskracht en power van de skill
                    float dmg = _atk * mageBS.Power;
                    enemy.TakeDamage(dmg);  // Roept TakeDamage van Enemy aan
                    Debug.Log($"{_character.name} slaat {enemy.name} voor {dmg} schade.");
                }
            });
    }

    public SkillHandler SkillOne()
    {
        Skill mageSO = new Skill
        {
            Name = "Fire Ball",
            SkillType = SkillType.Attack,
            TargetType = TargetType.SplashEnemy,  // Raakt meerdere vijanden tegelijk
            Power = 0.8f
        };

        return new SkillHandler(
            mageSO,
            targets =>
            {
                // Loopt door alle targets die van het type Enemy zijn
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * mageSO.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Fire Ball voor {dmg} schade.");
                }
            });
    }

    public SkillHandler SkillTwo()
    {
        Skill mageST = new Skill
        {
            Name = "Inferno",
            SkillType = SkillType.Attack,
            TargetType = TargetType.AOEEnemy,  // Area of Effect, raakt alle vijanden in gebied
            Power = 1.2f,  // sterkere aanval dan de vorige skills

            // Buff eigenschappen die deze skill ook geeft aan de caster
            BuffType = StatModifier.StatType.Atk,
            BuffAmount = 10f,
            BuffTurns = 2,
            IsBuffPercent = true  // Buff is een percentage verhoging
        };

        return new SkillHandler(
            mageST,
            targets =>
            {
                // Maakt een nieuwe aanvalskracht buff aan op basis van de skill parameters
                StatModifier atkBuff = new StatModifier(
                    StatModifier.StatType.Atk,
                    mageST.BuffAmount,
                    mageST.BuffTurns,
                    mageST.IsBuffPercent
                );

                _character.ApplyBuff(atkBuff);  // Past de buff toe op de caster

                // Richt schade aan op alle vijanden in de targets
                foreach (Enemy enemy in targets.OfType<Enemy>())
                {
                    float dmg = _atk * mageST.Power;
                    enemy.TakeDamage(dmg);
                    Debug.Log($"{_character.name} raakt {enemy.name} met Inferno voor {dmg} schade.");
                }
            });
    }
}
