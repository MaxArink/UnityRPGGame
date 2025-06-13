using System.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "Game/EntityData", order = 1)]
public class EntityData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string _name;
    [SerializeField] private Sprite _portrait;
    [SerializeField] private GameObject _prefab;  // Hier wijs je het prefab toe met Character + ICharacter-component.

    [Header("Stats")]
    [SerializeField] private BaseStats _baseStats;
    [SerializeField] private EntityStats _entityStats;
    [SerializeField] private Role _role;

    public string Name => _name;
    public Sprite Portrait => _portrait;
    public GameObject Prefab => _prefab;
    public BaseStats BaseStats => _baseStats;
    public EntityStats EntityStats => _entityStats;
    public Role Role => _role;
}
