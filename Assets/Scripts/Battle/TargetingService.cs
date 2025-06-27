using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetingService : ITargetingService
{
    private BattleManager _battleManager;

    // Constructor ontvangt de BattleManager om toegang te hebben tot de karakters en vijanden
    public TargetingService(BattleManager pBattleManager)
    {
        _battleManager = pBattleManager;
    }

    /// <summary>
    /// Geeft een lijst van doelwitten terug op basis van targeting type en wie de gebruiker is.
    /// </summary>
    /// <param name="pTargetingType">Type targeting (bv Self, SingleTarget, AOE, etc.)</param>
    /// <param name="pUser">De entity die de actie uitvoert</param>
    /// <param name="pTargetsAllies">Of de targets bondgenoten zijn (true) of vijanden (false)</param>
    /// <returns>Lijst van doelwitten</returns>
    public List<Entity> GetTargets(TargetingType pTargetingType, Entity pUser, bool pTargetsAllies = false)
    {
        // Bepaal bondgenoten en vijanden van de gebruiker, filter direct doden eruit
        List<Entity> allies;
        List<Entity> enemies;

        if (pUser is Character)
        {
            allies = _battleManager.Characters.Cast<Entity>().Where(e => !e.IsDead).ToList();
            enemies = _battleManager.Enemies.Cast<Entity>().Where(e => !e.IsDead).ToList();
        }
        else
        {
            allies = _battleManager.Enemies.Cast<Entity>().Where(e => !e.IsDead).ToList();
            enemies = _battleManager.Characters.Cast<Entity>().Where(e => !e.IsDead).ToList();
        }

        // Kies welke groep wordt getarget, bondgenoten of vijanden
        List<Entity> targetGroup = pTargetsAllies ? allies : enemies;

        // Selecteer targets afhankelijk van het targeting type
        switch (pTargetingType)
        {
            case TargetingType.Self:
                // Alleen de gebruiker zelf
                return new List<Entity> { pUser };

            case TargetingType.SingleTarget:
                // Kies 1 target via gewicht op basis van 'taunt' (als dat bestaat)
                Entity chosen = _battleManager.GetWeightedRandomByTaunt(targetGroup);
                return chosen != null ? new List<Entity> { chosen } : new List<Entity>();

            case TargetingType.Splash:
                // Bijvoorbeeld de eerste 3 targets uit de groep
                return targetGroup.Take(3).ToList();

            case TargetingType.AOE:
                // Alle targets in de groep
                return targetGroup;

            default:
                Debug.LogWarning("Unsupported targeting type: " + pTargetingType);
                return new List<Entity>();
        }
    }
}
