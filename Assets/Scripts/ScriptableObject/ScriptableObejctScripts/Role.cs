using UnityEngine;

[CreateAssetMenu(fileName = "Role", menuName = "Game/Role", order = 1)]
public class Role : ScriptableObject
{
    [SerializeField] private float _taunt = 0.5f;

    public float Taunt { get => _taunt; set => _taunt = value; }
}