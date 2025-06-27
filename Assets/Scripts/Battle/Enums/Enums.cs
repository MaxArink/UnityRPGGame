using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// wordt tijdelijk niet gebruikt, maar kan handig zijn voor later, kost mij nu te veel tijd om dit te refactoren dat die deze goed gebruikt
// Bevint zig nu in de StatModifier class, maar kan handig zijn om hier te hebben voor later
public enum StatType { Hp, Atk, Def, Speed }
public enum SkillType { Attack, Heal, Buff, Debuff }
public enum TargetType { SingleEnemy, SplashEnemy, AOEEnemy, SingleAlly, SplashAlly, AOEAlly, Self }
public enum TargetingType { SingleTarget, Splash, AOE, Self }

public class Enums
{
    // This class is intentionally left empty.
    // It serves as a container for the enums defined above.
}