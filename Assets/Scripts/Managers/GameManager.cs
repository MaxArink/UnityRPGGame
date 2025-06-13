using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private List<EntityData> _teamLineup;

    [Header("Test Be Gone")]
    [SerializeField] private ChallengeData _challenge = null;


    private ChallengeData _pendingBattleData;
    
    public ChallengeData PendingBattleData => _pendingBattleData;
    public List<EntityData> TeamLineup { get => _teamLineup; private set => _teamLineup = value; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (_pendingBattleData == null)
            _pendingBattleData = _challenge;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            BattleManager.instance.StartBattle(_challenge);
    }

    public void SetPendingBattleData(ChallengeData data)
    {
        _pendingBattleData = data;
    }
}
