using System.Data;
using UnityEngine;

public class Enemy : Entity
{
    private IEnemy _enemy;

    private bool _isDead = false;
    /// <summary>
    /// Kan denk ik wel een event zijn voor vernoetigen van zelf zodat hij ook uit de list met enemy's gehaalt wordt
    /// </summary>
    public bool IsDead { get => _isDead; private set => _isDead = value; }

    protected override void Awake()
    {
        base.Awake();
        _enemy = GetComponent<IEnemy>();
    }

    

    public void PerformAction()
    {
        Debug.Log($"{_enemy} Doet iets");
    }

    public override void TakeDamage(float rawDamage)
    {
        // Bijvoorbeeld: vijanden hebben 10% minder schade van kritieke hits - nee enemy gaat waschijnlijk gewoon veel meer hp hebben dan characters
        float modifiedDamage = rawDamage * 0.9f;
        base.TakeDamage(modifiedDamage);

        // Extra enemy-specifieke logica...
    }
}
