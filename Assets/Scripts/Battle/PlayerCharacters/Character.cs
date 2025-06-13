using UnityEngine;

public class Character : Entity
{
    private ICharacter _character;

    private bool _isDown = false;
    
    /// <summary>
    /// kan een event maken als down denk ik hou het tijdelijk wel zo
    /// </summary>
    public bool IsDown { get => _isDown; private set => _isDown = value; }

    protected override void Awake()
    {
        base.Awake();
        _character = GetComponent<ICharacter>();
    }

    private void Start()
    {
        _character.BasicSkill();
    }

    public override void TakeDamage(float pRawDamage)
    {
        // Bijvoorbeeld: speler kan shield of buffs hebben die schade verder reduceren
        float modifiedDamage = pRawDamage; // Je kan hier eigen logica toevoegen

        // Eigen logica van mij
        

        base.TakeDamage(modifiedDamage);

        // Extra character-specifieke logica...
    }
}
