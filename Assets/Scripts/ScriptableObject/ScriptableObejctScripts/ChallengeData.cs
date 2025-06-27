using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChallengeData", menuName = "Game/ChallengeData", order = 3)]
public class ChallengeData : ScriptableObject
{
    [SerializeField] private List<EntityData> _enemyLineup;

    public List<EntityData> EnemyLineup => _enemyLineup;
}
