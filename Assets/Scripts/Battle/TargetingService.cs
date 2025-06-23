using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetingService : ITargetingService
{
    private BattleManager _battleManager;

    public TargetingService(BattleManager pBattleManager)
    {
        _battleManager = pBattleManager;
    }

    public List<Entity> GetTargets(TargetingType pTargetingType, Entity pUser, bool pTargetsAllies)
    {
        // Bepaal bondgenoten en vijanden van de gebruiker
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

        var targetGroup = pTargetsAllies ? allies : enemies;

        switch (pTargetingType)
        {
            case TargetingType.Self:
                return new List<Entity> { pUser };

            case TargetingType.SingleTarget:
                return targetGroup.Take(1).ToList();

            case TargetingType.Splash:
                // Bijvoorbeeld eerste 3 vijanden/bondgenoten
                return targetGroup.Take(3).ToList();

            case TargetingType.AOE:
                return targetGroup;

            default:
                Debug.LogWarning("Unsupported targeting type: " + pTargetingType);
                return new List<Entity>();
        }
    }
}
