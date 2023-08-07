using System;
using System.Collections;
using System.Collections.Generic;
using Characters.Enemy;
using Characters.Models;
using Grid;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    private static BoardManager instance;

    public static BoardManager Instance
    {
        get
        {
            if (instance == null)
            {
                var inst = FindObjectOfType<BoardManager>();
                instance = inst;
            }

            return instance;
        }
    }

    [SerializeField] private GameObject damagePrefab;

    [SerializeField] private Transform selectTr;
    [SerializeField] private ParticleSystem selectParticle;

    private Camera cam;
    private RaycastHit hit;
    private PlayerCharacter currentPlayer;
    private PlayerCharacter currentlyHighlightedCharacter;
    private List<PlayerCharacter> playerUnits = new List<PlayerCharacter>();
    private List<EnemyCharacter> enemyUnits = new List<EnemyCharacter>();
    private List<Drone> drones = new List<Drone>();
    private List<Dreadnought> dreadnoughts = new List<Dreadnought>();
    private List<CommandUnit> commandUnits = new List<CommandUnit>();
    private IEnumerator levelSpawner;
    private static readonly int boardSize = 8;
    private static readonly float constantDelay = 1;
    private int[] unitsAmounts;

    public enum LevelType { None, Level1, Level2, Level3, Chess, GruntWar, ThreeManArmy, Hardcore, Custom}

    private LevelType currentLevel = LevelType.None;

    public static Board board;
    public bool IsPlayerTurn { get; set; }
    [HideInInspector] public UnityEvent<PlayerCharacter> charSelected;
    [HideInInspector] public UnityEvent allEnemiesSpawned;

    private void Start()
    {
        cam = Camera.main;
        board = GetComponentInChildren<Board>();
        LevelInfo.Init();
    }

    public void ForEachCell(Action<Cell> action)
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                action.Invoke(board[i,j]);
            }
        }
    }

    public void StartLevel(LevelType level, bool showGameView = true, int[] unitsQuantity = null)
    {
        if (showGameView) GameUIController.Instance.SetupGameView();

        currentLevel = level;

        playerUnits = new List<PlayerCharacter>();
        enemyUnits = new List<EnemyCharacter>();
        drones = new List<Drone>();
        dreadnoughts = new List<Dreadnought>();
        commandUnits = new List<CommandUnit>();
        
        switch (level)
        {
            case LevelType.Level1 : levelSpawner = LevelInfo.SetupLevel1(playerUnits, enemyUnits, drones, dreadnoughts, commandUnits, allEnemiesSpawned); break;
            case LevelType.Level2 : levelSpawner = LevelInfo.SetupLevel2(playerUnits, enemyUnits, drones, dreadnoughts, commandUnits, allEnemiesSpawned); break;
            case LevelType.Level3 : levelSpawner = LevelInfo.SetupLevel3(playerUnits, enemyUnits, drones, dreadnoughts, commandUnits, allEnemiesSpawned); break;
            case LevelType.Chess : levelSpawner = LevelInfo.ClassicalChess(playerUnits, enemyUnits, drones, dreadnoughts, commandUnits, allEnemiesSpawned); break;
            case LevelType.GruntWar : levelSpawner = LevelInfo.GruntsWar(playerUnits, enemyUnits, drones, dreadnoughts, commandUnits, allEnemiesSpawned); break;
            case LevelType.ThreeManArmy : levelSpawner = LevelInfo.ThreeManArmy(playerUnits, enemyUnits, drones, dreadnoughts, commandUnits, allEnemiesSpawned); break;
            case LevelType.Hardcore : levelSpawner = LevelInfo.HardcoreLevel(playerUnits, enemyUnits, drones, dreadnoughts, commandUnits, allEnemiesSpawned); break;
            case LevelType.Custom:
            {
                if (unitsQuantity != null)
                {
                    this.unitsAmounts = unitsQuantity;
                    levelSpawner = LevelInfo.Custom(playerUnits, enemyUnits, drones, dreadnoughts, commandUnits, allEnemiesSpawned, unitsQuantity);
                }
                else
                {
                    levelSpawner = LevelInfo.Custom(playerUnits, enemyUnits, drones, dreadnoughts, commandUnits, allEnemiesSpawned, this.unitsAmounts);
                }
                break;
            }
        }

        StartCoroutine(levelSpawner);
    }

    public void RestartLevel()
    {
        ResetAll();
        StartLevel(currentLevel, false);
    }

    public void ResetAll()
    {
        foreach (var unit in playerUnits)
        {
            if (unit != null) Destroy(unit.gameObject);
        }
        
        foreach (var unit in enemyUnits)
        {
            if (unit != null) Destroy(unit.gameObject);
        }

        if (levelSpawner != null) StopCoroutine(levelSpawner);
        HideSelectOverlay();
        ForEachCell((c) => c.ResetCell());
        GameUIController.Instance.ResetGameUI();
    }

    public static Character SpawnPiece(int row, int col, GameObject prefab)
    {
        var spawnCell = board[row, col];
        var piece = Instantiate(prefab);
        
        var character = piece.GetComponent<Character>();
        character.Setup(spawnCell);
        spawnCell.SetCharacter(character);
        
        GameUIController.Instance.SpawnCharacter(character);
        
        return character;
    }

    public Cell GetRandomPlayerSpawnCell()
    {
        while (true)
        {
            int randomX = Random.Range(0, 2);
            int randomY = Random.Range(0, 8);
            if (!board[randomX, randomY].IsOccupied) return board[randomX, randomY];
        }
    }
    
    public Cell GetRandomEnemySpawnCell()
    {
        while (true)
        {
            int randomX = Random.Range(4, 8);
            int randomY = Random.Range(0, 8);
            if (!board[randomX, randomY].IsOccupied) return board[randomX, randomY];
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0.1f) TrySelectCell(Input.mousePosition);
    }
    
    private void TrySelectCell(Vector3 position)
    {
        if (!IsPlayerTurn) return;
        
        Physics.Raycast(cam.ScreenPointToRay(position), out hit, Mathf.Infinity);
        if (hit.collider != null && hit.collider.TryGetComponent( out Cell cell))
        {
            UnselectAllCells();

            if (cell.SelectCell() == CellState.Select) SelectCharacter((PlayerCharacter)cell.Character);
        }
    }

    public void UnselectAllCells()
    {
        ForEachCell(c => c.UnSelectCell());
    }

    public void CheckActions()
    {
        bool hasActions = false;
        foreach (var unit in playerUnits)
        {
            if (unit != null)
            {
                if(!unit.HasMoved) unit.GetMovementCells();
                if(!unit.HasAttacked) unit.GetAttackTargets();
                if (unit.HasActions && !hasActions) hasActions = true;
                
                unit.portrait.UpdateActions();
                GameUIController.Instance.UpdatePlayerDisplay(unit);
            }
        }
        
        if(!hasActions) EndPlayerTurn();
    }

    public void SelectCharacter(PlayerCharacter character)
    {
        currentPlayer = character;
        currentPlayer.portrait.Select();
        charSelected.Invoke(character);
        
        selectTr.gameObject.SetActive(true);
        selectTr.position = character.transform.position;
        selectParticle.Play();
    }

    public void ShowSelectOverlay(PlayerCharacter character)
    {
        if (currentPlayer != character || !IsPlayerTurn) return;
        
        selectTr.gameObject.SetActive(true);
        selectTr.position = character.transform.position;
        selectParticle.Play();
    }

    public void HideSelectOverlay()
    {
        selectTr.gameObject.SetActive(false);
    }

    public bool IsCurrentlySelected(PlayerCharacter character)
    {
        return currentPlayer == character;
    }
    
    public static bool IsWithinBoard(int row, int col)
    {
        return row >= 0 && row < boardSize && col >= 0 && col < boardSize;
    }

    public static Cell TryGetCell(int row, int col)
    {
        if (IsWithinBoard(row, col)) return board[row, col];

        return null;
    }

    public void EndPlayerTurn()
    {
        if(!IsPlayerTurn) return;
        
        if (currentPlayer != null) currentPlayer.UnSelectCharacter();
        currentPlayer = null;
        IsPlayerTurn = false;
        
        foreach (var unit in playerUnits)
        {
            if (unit != null)
            {
                unit.CanMove = false;
                unit.CanAttack = false;
                unit.portrait.UpdateActions();
                GameUIController.Instance.UpdatePlayerDisplay(unit);
            }
        }
        
        foreach (var unit in enemyUnits)
        {
            if (unit != null)
            {
                unit.ResetTurn();
            }
        }

        GameUIController.Instance.ActivateEnemyTurn();
        
        HideSelectOverlay();
        StartCoroutine(PlayEnemyTurn());
    }

    private void EndAITurn()
    {
        foreach (var unit in enemyUnits)
        {
            if (unit != null)
            {
                unit.HasMoved = true;
                unit.HasAttacked = true;
                unit.portrait.UpdateActions();
            }
        }

        foreach (var unit in playerUnits)
        {
            unit.ResetTurn();
        }
        
        GameUIController.Instance.ActivatePlayerTurn();
        IsPlayerTurn = true;
    }

    private IEnumerator PlayEnemyTurn()
    {
        yield return new WaitForSeconds(2);
        float delay = 0;
        
        foreach (var drone in drones)
        {
            if(drone != null && !drone.IsDead)
            {
                if (drone.TryMove(ref delay)) yield return new WaitForSeconds(delay + constantDelay);
                if (drone.TryAttack(ref delay)) yield return new WaitForSeconds(delay+ constantDelay);
            }
        }
        
        foreach (var dreadnought in dreadnoughts)
        {
            if(dreadnought != null && !dreadnought.IsDead)
            {
                if (dreadnought.TryMove(ref delay)) yield return new WaitForSeconds(delay + constantDelay);
                if (dreadnought.TryAttack(ref delay)) yield return new WaitForSeconds(delay + constantDelay);
            }
        }

        foreach (var cUnit in commandUnits)
        {
            if (cUnit != null && !cUnit.IsDead)
            {
                if (cUnit.TryMove(ref delay)) yield return new WaitForSeconds(delay + constantDelay);
            }
        }

        yield return new WaitForSeconds(constantDelay);
        EndAITurn();
    }

    public void DisplayDamage(Cell cell, int amount)
    {
        var dmg = Instantiate(damagePrefab);
        dmg.transform.position = cell.Position;
        var dmgController = dmg.GetComponent<DamageReduction>();
        dmgController.Setup(amount);
    }

    public void CheckForEnemyWin()
    {
        bool alivePlayer = false;
        foreach (var unit in playerUnits)
        {
            if (unit != null && !unit.IsDead)
            {
                alivePlayer = true;
                break;
            }
        }
        
        if(!alivePlayer) EnemyWin();
    }

    public void EnemyWin()
    {
        GameUIController.Instance.ShowEndGame(false);
    }

    public void CheckPlayerWin()
    {
        bool hasCommandUnits = false;

        foreach (var unit in commandUnits)
        {
            if (unit != null && !unit.IsDead)
            {
                hasCommandUnits = true;
                break;
            }
        }
        
        if(!hasCommandUnits) GameUIController.Instance.ShowEndGame(true);
    }
}
