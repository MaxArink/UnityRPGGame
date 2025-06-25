using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<EntityData> _teamLineup;

    [Header("Test Be Gone")]
    [SerializeField] private ChallengeData _challenge = null;

    
    private ChallengeData _pendingBattleData;

    public ChallengeData PendingBattleData { get => _pendingBattleData; set => _pendingBattleData = value; }

    public List<EntityData> TeamLineup { get => _teamLineup; private set => _teamLineup = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //private void Start()
    //{
    //    if (_pendingBattleData == null)
    //        _pendingBattleData = _challenge;
    //}

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //        BattleManager.Instance.StartBattle(_challenge);
    //}

    public void SetPendingBattleData(ChallengeData data)
    {
        _pendingBattleData = data;
    }
}
