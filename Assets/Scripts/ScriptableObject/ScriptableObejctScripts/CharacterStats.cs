using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Game/CharacterStats", order = 1)]
public class CharacterStats : ScriptableObject
{
    [SerializeField] private float _hp = 1f;
    [SerializeField] private float _mp = 1f;
    [SerializeField] private float _atk = 1f;
    [SerializeField] private float _def = 1f;
    [SerializeField] private float _speed = 1f;

    public float Hp { get => _hp; set => _hp = value; }
    public float Mp { get => _mp; set => _mp = value; }
    public float Atk { get => _atk; set => _atk = value; }
    public float Def { get => _def; set => _def = value; }
    public float Speed { get => _speed; set => _speed = value; }
}
