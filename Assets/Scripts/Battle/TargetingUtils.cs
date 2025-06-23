public class TargetingUtils
{
    public TargetingType ConvertToTargetingType(TargetType type)
    {
        return type switch
        {
            TargetType.SingleEnemy => TargetingType.SingleTarget,
            TargetType.AOEEnemy => TargetingType.AOE,
            TargetType.SplashEnemy => TargetingType.Splash,
            TargetType.Self => TargetingType.Self,
            _ => TargetingType.SingleTarget // fallback
        };
    }

    public TargetType ConvertToTargetType(TargetingType type)
    {
        return type switch
        {
            TargetingType.SingleTarget => TargetType.SingleEnemy,
            TargetingType.AOE => TargetType.AOEEnemy,
            TargetingType.Splash => TargetType.SplashEnemy,
            TargetingType.Self => TargetType.Self,
            _ => TargetType.SingleEnemy // fallback
        };
    }
}