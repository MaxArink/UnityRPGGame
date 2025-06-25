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

    public event Action<Entity> OnTurnStarted;
    public event Action<bool> OnBattleOver;

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
    }

    private void Start()
    {
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

        _turnOrder = _turnOrder.OrderByDescending(e => e.Stats.GetStatValue(StatModifier.StatType.Speed)).ToList();
        _currentTurnIndex = 0;
        StartNextTurn();
    }

    private void StartNextTurn()
    {
        if (_battleOver || CheckBattleOver())
            return;

        if (_currentTurnIndex >= _turnOrder.Count)
            _currentTurnIndex = 0;

        while (_turnOrder[_currentTurnIndex].IsDead)
        {
            _currentTurnIndex++;
            if (_currentTurnIndex >= _turnOrder.Count)
                _currentTurnIndex = 0;

            // Voorkom infinite loop bij een fout
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
            // EndTurn() wordt binnen PerformAction aangeroepen
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

            SceneLoader.Instance.UnloadScene("BattleScene");

            return true;
        }

        if (!charactersAlive)
        {
            Debug.Log("ENEMIES WIN");
            _battleOver = true;
            OnBattleOver?.Invoke(false);

            SceneLoader.Instance.UnloadScene("BattleScene");

            return true;
        }

        return false;
    }

    private void HandleEntityDeath(Entity deadEntity)
    {
        Debug.Log($"BattleManager noticed {deadEntity.name} died");

        // Verwijder uit turn order
        _turnOrder.Remove(deadEntity);

        // Update turnIndex als nodig
        if (_currentTurnIndex >= _turnOrder.Count)
            _currentTurnIndex = 0;

        // Informeer UI of andere systemen (indien nodig)
        //TurnOrderUI.Instance?.RemoveEntity(deadEntity);

        // Check of battle afgelopen is
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
    /// Selects a random entity from the provided list, weighted by their taunt value.  
    /// </summary>  
    /// <param name="pEntities">List of entities to select from.</param>  
    /// <returns>A randomly selected entity, weighted by taunt.</returns>  
    private Entity GetWeightedRandomByTaunt(List<Entity> pEntities)
    {
        // Return null als er geen entities zijn om uit te kiezen  
        if (pEntities.Count == 0) return null;

        // Bereken de totale taunt waarde van alle entities  
        float totalTaunt = pEntities.Sum(e => e.Role.Taunt);

        // Als de totale taunt waarde nul of minder is, gebruik het aantal entities als fallback  
        if (totalTaunt <= 0)
            totalTaunt = pEntities.Count;

        // Genereer een random waarde tussen 0 en de totale taunt waarde  
        float rand = UnityEngine.Random.Range(0f, totalTaunt);
        float cumulative = 0f;

        // Itereer door de entities en selecteer er één op basis van gewogen taunt  
        foreach (Entity entity in pEntities)
        {
            // Gebruik de taunt waarde van de entity als gewicht, standaard naar 1 als taunt nul is  
            float weight = entity.Role.Taunt > 0 ? entity.Role.Taunt : 1f;
            cumulative += weight;

            // Return de entity als de random waarde binnen zijn cumulatieve gewicht valt  
            if (rand <= cumulative)
                return entity;
        }

        // Fallback naar de eerste entity in de lijst als er geen selectie is gemaakt  
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

    // Register methods als je ze nodig hebt (bijv. voor runtime aanmaken)
    public void RegisterCharacter(Character c) => _characterTeam.Add(c);
    public void RegisterEnemy(Enemy e) => _enemyTeam.Add(e);
}
