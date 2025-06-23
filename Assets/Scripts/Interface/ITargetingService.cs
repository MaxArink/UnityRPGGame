using System.Collections.Generic;

public interface ITargetingService
{
    List<Entity> GetTargets(TargetingType pTargetingType, Entity pUser, bool pTargetsAllies);
}
