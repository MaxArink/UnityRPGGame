using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public TargetingService TargetingService { get; private set; }
    public TargetingUtils TargetingUtils { get; private set; }
    public AdjacencyService AdjacencyService { get; private set; }

    public event Action<Entity> OnTurnStarted;
    public event Action<bool> OnBattleOver;
    public event Action<bool> OnBattleEnded; // true = winst, false = verlies

    [SerializeField] private List<Transform> _characterSpawnPoints;
    [SerializeField] private List<Transform> _enemySpawnPoints;

    private List<Character> _characterTeam = new List<Character>();
    private List<Enemy> _enemyTeam = new List<Enemy>();

    public List<Entity> Enemies => _enemyTeam.Cast<Entity>().ToList();
    public List<Entity> Characters => _characterTeam.Cast<Entity>().ToList();

    private List<Entity> _turnOrder = new List<Entity>();
    private int _currentTurnIndex = 0;

    private bool _battleStarted = false;
    private bool _battleOver = false;

    private bool _isTurnAdvancing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LateAwake();
    }

    private void LateAwake()
    {
        TargetingService = new TargetingService(this);
        TargetingUtils = new TargetingUtils();
        AdjacencyService = new AdjacencyService();
    }

    private void Start()
    {
        GameManager.Instance.RegisterBattleManager(this);
        StartCoroutine(WaitForChallengeDataAndStart());
    }

    private IEnumerator WaitForChallengeDataAndStart()
    {
        while (GameManager.Instance.PendingBattleData == null)
        {
            yield return null; // wacht 1 frame
        }

        StartBattle(GameManager.Instance.PendingBattleData);
    }

    public void StartBattle(ChallengeData pChallengeData)
    {
        if (_battleStarted)
        {
            Debug.LogWarning("Battle is already running!");
            return;
        }

        _battleStarted = true;
        _battleOver = false;

        ClearExistingUnits();
        SpawnCharacterTeam();
        SpawnEnemyTeam(pChallengeData.EnemyLineup);
        InitializeBattle();
    }

    private void ClearExistingUnits()
    {
        foreach (Character c in _characterTeam)
        {
            if (c != null) Destroy(c.gameObject);
        }
        foreach (Enemy e in _enemyTeam)
        {
            if (e != null) Destroy(e.gameObject);
        }
        _characterTeam.Clear();
        _enemyTeam.Clear();
        _turnOrder.Clear();
        _currentTurnIndex = 0;
    }

    private void SpawnCharacterTeam()
    {
        List<EntityData> teamLineup = CleanUpCharacterLineup(GameManager.Instance.TeamLineup);
        for (int i = 0; i < teamLineup.Count; i++)
        {
            EntityData data = teamLineup[i];
            Transform spawnPoint = i < _characterSpawnPoints.Count ? _characterSpawnPoints[i] : _characterSpawnPoints.Last();

            GameObject go = Instantiate(data.Prefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            Character character = go.GetComponent<Character>();
            character.Initialize(data);

            if (go.TryGetComponent<ICharacter>(out var logic))
            {
                logic.InitializeSkills();
            }

            character.OnDie += HandleEntityDeath;

            _characterTeam.Add(character);
        }
    }

    private List<EntityData> CleanUpCharacterLineup(List<EntityData> pTeam)
    {
        return pTeam.Where(c => c != null).ToList();
    }

    private void SpawnEnemyTeam(List<EntityData> pEnemyLineup)
    {
        List<EntityData> cleanedList = CleanUpEnemyLineup(pEnemyLineup);
        for (int i = 0; i < cleanedList.Count; i++)
        {
            EntityData data = cleanedList[i];
            Transform spawnPoint = i < _enemySpawnPoints.Count ? _enemySpawnPoints[i] : _enemySpawnPoints.Last();

            GameObject go = Instantiate(data.Prefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            Enemy enemy = go.GetComponent<Enemy>();
            enemy.Initialize(data);

            if (go.TryGetComponent<IEnemy>(out var logic))
            {
                logic.InitializeSkills();
            }

            enemy.OnDie += HandleEntityDeath;

            _enemyTeam.Add(enemy);
        }
    }

    private List<EntityData> CleanUpEnemyLineup(List<EntityData> pEnemies)
    {
        return pEnemies.Where(e => e != null).ToList();
    }

    private void InitializeBattle()
    {
        _turnOrder.Clear();
        _turnOrder.AddRange(_characterTeam.Cast<Entity>());
        _turnOrder.AddRange(_enemyTeam.Cast<Entity>());

        // Sorteer beurtvolgorde op snelheid (hoog naar laag)
        _turnOrder = _turnOrder.OrderByDescending(e => e.Stats.GetStatValue(StatModifier.StatType.Speed)).ToList();
        _currentTurnIndex = 0;

        // Optioneel: kan gebruikt worden om nabijheid relaties te bepalen
        // AdjacencyService.AssignAdjacency(_characterTeam.Cast<Entity>().ToList());
        // AdjacencyService.AssignAdjacency(_enemyTeam.Cast<Entity>().ToList());

        StartNextTurn();
    }

    private void StartNextTurn()
    {
        if (_battleOver || CheckBattleOver())
            return;

        if (_currentTurnIndex >= _turnOrder.Count)
            _currentTurnIndex = 0;

        // Skip dode entiteiten in beurtvolgorde
        while (_turnOrder[_currentTurnIndex].IsDead)
        {
            _currentTurnIndex++;
            if (_currentTurnIndex >= _turnOrder.Count)
                _currentTurnIndex = 0;

            // Voorkom oneindige loop bij fouten: als iedereen dood is, markeer battle over
            if (_turnOrder.All(e => e.IsDead))
            {
                _battleOver = true;
                return;
            }
        }

        Entity current = _turnOrder[_currentTurnIndex];

        Debug.Log($"Turn: {current.name}");

        OnTurnStarted?.Invoke(current);

        if (current is Enemy enemy)
        {
            enemy.PerformAction();
            EndTurn();
        }
        else if (current is Character character)
        {
            character.PerformAction();
            // EndTurn() wordt binnen PerformAction aangeroepen voor Characters
        }
    }

    public void EndTurn()
    {
        if (_battleOver || _isTurnAdvancing) return;

        _isTurnAdvancing = true;
        _currentTurnIndex++;
        StartCoroutine(DelayNextTurn());
    }

    private IEnumerator DelayNextTurn()
    {
        yield return new WaitForSeconds(0.5f);
        _isTurnAdvancing = false;
        StartNextTurn();
    }

    /// <summary>
    /// Controleert of de battle voorbij is.
    /// Vuur het event OnBattleOver en unload de battle scene als het over is.
    /// </summary>
    /// <returns>True als battle over is, anders false.</returns>
    private bool CheckBattleOver()
    {
        if (_battleOver) return true;

        bool charactersAlive = _characterTeam.Any(p => !p.IsDead);
        bool enemiesAlive = _enemyTeam.Any(e => !e.IsDead);

        if (!enemiesAlive)
        {
            Debug.Log("PLAYERS WIN");
            _battleOver = true;
            OnBattleOver?.Invoke(true);

            // Registratie en unload kunnen hier worden afgehandeld in GameManager
            return true;
        }

        if (!charactersAlive)
        {
            Debug.Log("ENEMIES WIN");
            _battleOver = true;
            OnBattleOver?.Invoke(false);

            return true;
        }

        return false;
    }

    private void HandleEntityDeath(Entity pDeadEntity)
    {
        Debug.Log($"BattleManager noticed {pDeadEntity.name} died");

        // Verwijder uit beurtvolgorde
        _turnOrder.Remove(pDeadEntity);

        // Update index als nodig
        if (_currentTurnIndex >= _turnOrder.Count)
            _currentTurnIndex = 0;

        // Eventueel UI of andere systemen informeren (commented out)
        // TurnOrderUI.Instance?.RemoveEntity(deadEntity);

        // Check opnieuw of battle over is
        CheckBattleOver();
    }

    // ** TARGETING METHODS met taunt-gewogen selectie **

    public Entity GetRandomAliveEnemy()
    {
        List<Entity> aliveEnemies = _enemyTeam.Where(e => !e.IsDead).Cast<Entity>().ToList();
        return GetWeightedRandomByTaunt(aliveEnemies);
    }

    public Entity GetRandomAliveCharacter()
    {
        List<Entity> aliveCharacters = _characterTeam.Where(c => !c.IsDead).Cast<Entity>().ToList();
        return GetWeightedRandomByTaunt(aliveCharacters);
    }

    /// <summary>
    /// Selecteert een random entity uit een lijst, gewogen op basis van taunt.
    /// </summary>
    /// <param name="pEntities">Lijst van entities om uit te kiezen.</param>
    /// <returns>Gekozen entity op basis van taunt gewichten.</returns>
    public Entity GetWeightedRandomByTaunt(List<Entity> pEntities)
    {
        if (pEntities.Count == 0) return null;

        float totalTaunt = pEntities.Sum(e => e.Role.Taunt);

        if (totalTaunt <= 0)
            totalTaunt = pEntities.Count;

        float rand = UnityEngine.Random.Range(0f, totalTaunt);
        float cumulative = 0f;

        foreach (Entity entity in pEntities)
        {
            float weight = entity.Role.Taunt > 0 ? entity.Role.Taunt : 1f;
            cumulative += weight;

            if (rand <= cumulative)
                return entity;
        }

        // Fallback: eerste entity kiezen als er geen selectie werd gemaakt
        return pEntities[0];
    }

    public Entity GetFirstAliveEnemy()
    {
        return _enemyTeam.FirstOrDefault(e => !e.IsDead);
    }

    public Entity GetFirstAliveAlly()
    {
        return _characterTeam.FirstOrDefault(c => !c.IsDead);
    }

    public List<Entity> GetAliveEnemies()
    {
        return _enemyTeam.Where(e => !e.IsDead).Cast<Entity>().ToList();
    }

    public List<Entity> GetAliveCharacters()
    {
        return _characterTeam.Where(c => !c.IsDead).Cast<Entity>().ToList();
    }

    // Register methods, handig voor runtime aanmaken van units
    public void RegisterCharacter(Character c) => _characterTeam.Add(c);
    public void RegisterEnemy(Enemy e) => _enemyTeam.Add(e);
}
