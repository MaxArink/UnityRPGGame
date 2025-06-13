using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance {  get; private set; }
    
    public event Action<Entity> OnTurnStarted;

    [SerializeField] private List<Transform> _characterSpawnPoints;
    [SerializeField] private List<Transform> _enemySpawnPoints;

    // Moet een list ophalen van jou chosen characters
    private List<Character> _playersTeam = new List<Character>();
    // Moet een list ophalen van de overworld enemy's enemy list
    private List<Enemy> _enemyTeam = new List<Enemy>();

    private List<Entity> _turnOrder = new();
    private int _currentTurnIndex = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        //Weet niet zeker of het makelijker is om een nieuwe aan te maken als een nieuwe gevecht begint
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChallengeData challengeData = GameManager.instance.PendingBattleData;

        if (challengeData != null)
        {
            StartBattle(challengeData);
        }
        else
        {
            Debug.LogWarning("No challenge data set!");
        }

        //InitializeBattle();
    }

    public void StartBattle(ChallengeData challengeData)
    {
        //_enemyTeam.Clear();
        //_playersTeam.Clear(); // Of: Load vanuit GameManager

        SpawnPlayerTeam();
        SpawnEnemyTeam(challengeData.EnemyLineup);

        InitializeBattle();
    }

    //public void SetupBattle(List<Character> players, List<Enemy> enemies)
    //{
    //    _playersTeam.Clear();
    //    _playersTeam.AddRange(players);

    //    _enemyTeam.Clear();
    //    _enemyTeam.AddRange(enemies);

    //    InitializeBattle();
    //}

    private void InitializeBattle()
    {
        _turnOrder.Clear();
        
        _turnOrder.AddRange(_playersTeam);
        _turnOrder.AddRange(_enemyTeam);

        _turnOrder = _turnOrder.OrderByDescending(e => e.Stats.GetStatValue(StatModifier.StatType.Speed)).ToList();

        _currentTurnIndex = 0;
        StartNextTurn();
    }

    private void SpawnPlayerTeam()
    {
        List<EntityData> teamLinup = CleanUpTeamLineup(GameManager.instance.TeamLineup);

        for (int i = 0; i < teamLinup.Count; i++)
        {
            EntityData characterData = teamLinup[i];
            GameObject prefab = characterData.Prefab;

            // Pak het spawnpunt op basis van de index, of val terug op de laatste als je er te weinig hebt
            Transform spawnPoint = (i < _characterSpawnPoints.Count) ? _characterSpawnPoints[i] : _characterSpawnPoints[_characterSpawnPoints.Count - 1];

            GameObject go = Instantiate(prefab, spawnPoint.position, Quaternion.identity, _characterSpawnPoints[i]);
            Character character = go.GetComponent<Character>();
            character.Initialize(characterData);
            _playersTeam.Add(character);
        }
    }

    private List<EntityData> CleanUpTeamLineup(List<EntityData> team)
    {
        List<EntityData> cleanedList = new List<EntityData>();

        foreach (EntityData character in team)
        {
            if (character != null)
                cleanedList.Add(character);
        }

        return cleanedList;
    }

    private void SpawnEnemyTeam(List<EntityData> enemyLineup)
    {
        List<EntityData> cleanedList = CleanUpEnemyLineup(enemyLineup);

        for (int i = 0; i < cleanedList.Count; i++)
        {
            EntityData enemyData = cleanedList[i];
            GameObject prefab = enemyData.Prefab;

            Transform spawnPoint = (i < _enemySpawnPoints.Count) ? _enemySpawnPoints[i] : _enemySpawnPoints[_enemySpawnPoints.Count - 1];

            GameObject go = Instantiate(prefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            Enemy enemy = go.GetComponent<Enemy>();
            enemy.Initialize(enemyData);
            _enemyTeam.Add(enemy);
        }
    }

    private List<EntityData> CleanUpEnemyLineup(List<EntityData> enemies)
    {
        List<EntityData> cleanedList = new List<EntityData>();

        foreach (EntityData enemy in enemies)
        {
            if (enemy != null)
                cleanedList.Add(enemy);
        }

        return cleanedList;
    }

    private void StartNextTurn()
    {
        if (CheckBattleOver()) 
            return;

        if (_currentTurnIndex >= _turnOrder.Count)
            _currentTurnIndex = 0;

        Entity current = _turnOrder[_currentTurnIndex];

        Debug.Log($"Turn: {current.name}");

        OnTurnStarted?.Invoke(current);

        if (current is Enemy enemy)
        {
            enemy.PerformAction(); // Enemy AI
            EndTurn();
        }
    }

    public void EndTurn()
    {
        _currentTurnIndex++;
        StartNextTurn();
    }

    private bool CheckBattleOver()
    {
        bool playersAlive = _playersTeam.Any(p => !p.IsDown);
        bool enemiesAlive = _enemyTeam.Any(e => !e.IsDead);

        if (!enemiesAlive)
        {
            Debug.Log("PLAYERS WIN");
            return true;
        }
        
        if (!playersAlive)
        {
            Debug.Log("ENEMIES WIN");
            return true;
        }

        return false;
    }

    public void RegisterCharacter(Character c) => _playersTeam.Add(c);
    public void RegisterEnemy(Enemy e) => _enemyTeam.Add(e);
}