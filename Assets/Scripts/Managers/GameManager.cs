using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<EntityData> _teamLineup;

    [Header("Test Be Gone")]
    [SerializeField] private ChallengeData _challenge = null;

    // ** temp code wie ik hier niet wil laten staan **
    private int _enemiesDefeated = 0;
    [SerializeField] private int _totalEnemiesToDefeat = 4;
    [SerializeField] private string _winSceneName = "WinScene";

    private ChallengeTrigger _pendingTrigger;
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

    private void Start()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.OnBattleOver += HandleBattleEnded;
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

    public void SetPendingBattleData(ChallengeData pData, ChallengeTrigger pTrigger)
    {
        _pendingBattleData = pData;
        _pendingTrigger = pTrigger;
    }

    public void RegisterBattleManager(BattleManager battleManager)
    {
        battleManager.OnBattleOver += HandleBattleEnded;
    }

    // ** Temporary code: This section handles enemy defeat tracking and win screen loading. **
    // ** This code is intended for testing purposes and should be refactored or removed in production. **

    private void HandleBattleEnded(bool pPlayerWon)
    {
        Debug.Log($"Battle ended. Player won: {pPlayerWon}");
        if (_pendingTrigger == null)
        {
            Debug.LogWarning("No pending trigger set when battle ended.");
            return;
        }

        if (pPlayerWon)
        {
            Debug.Log("Calling OnBattleWon");
            _pendingTrigger.OnBattleWon();
            RegisterEnemyDefeat();
        }
        else
        {
            Debug.Log("Calling OnBattleLost");
            _pendingTrigger.OnBattleLost();
        }

        _pendingBattleData = null;
        _pendingTrigger = null;

        Debug.Log("Unloading BattleScene...");
        StartCoroutine(DelayedUnload());
    }

    private IEnumerator DelayedUnload()
    {
        yield return null; // wacht 1 frame zodat BattleManager events kunnen afhandelen
        SceneLoader.Instance.UnloadScene("BattleScene");
    }

    private void RegisterEnemyDefeat()
    {
        _enemiesDefeated++;
        Debug.Log($"Enemy defeated! Total: {_enemiesDefeated}");

        if (_enemiesDefeated >= _totalEnemiesToDefeat)
        {
            Debug.Log("All enemies defeated. You win!");
            LoadWinScreen();
        }
    }

    private void LoadWinScreen()
    {
        SceneLoader.Instance.LoadSceneSingle(_winSceneName); // Laadt de win scene
    }

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.OnBattleOver += HandleBattleEnded;
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.OnBattleOver -= HandleBattleEnded;
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.OnBattleOver -= HandleBattleEnded;
    }
}