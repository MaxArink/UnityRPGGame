using UnityEngine;

public class Goblin : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();

    public void BaseAction()
    {
        Entity target = BattleManager.Instance.GetRandomAliveCharacter();

        if (target != null)
        {
            Debug.Log($"{Enemy.name} valt {target.name} aan!");
            // Voer hier aanval uit op target
            target.TakeDamage(Enemy.Stats.GetStatValue(StatModifier.StatType.Atk));
        }
        else
        {
            Debug.Log("Geen beschikbare targets voor Goblin");
        }
    }
}
