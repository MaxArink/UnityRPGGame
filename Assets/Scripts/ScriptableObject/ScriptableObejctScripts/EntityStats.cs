using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityStats", menuName = "Game/EntityStats", order = 2)]
public class EntityStats : ScriptableObject
{
    [SerializeField] private float _mp = 10f;
    [SerializeField] private float _critProcent = 20f;
    [SerializeField] private float _critDamage = 100f;

    public float Mp { get => _mp; set => _mp = value; }
    public float CritProcent { get => _critProcent; set => _critProcent = value; }
    public float CritDamage { get => _critDamage; set => _critDamage = value; }
}