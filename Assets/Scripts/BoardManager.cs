using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Characters.Enemy;
using Characters.Models;
using Grid;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField] private GameObject gruntPrefab;
    [SerializeField] private GameObject jumpshipPrefab;
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private GameObject dreadnoughtPrefab;
    [SerializeField] private GameObject commandUnitPrefab;

    [SerializeField] private GameObject damagePrefab;

    [SerializeField] private Transform selectTr;
    [SerializeField] private ParticleSystem selectParticle;

    private Camera cam;
    private RaycastHit hit;
    private Cell selectedCell;
    private PlayerCharacter currentPlayer;
    private List<PlayerCharacter> playerUnits;
    private List<Drone> drones;
    private List<Dreadnought> dreadnoughts;
    private List<CommandUnit> commandUnits;
    private static readonly int boardSize = 8;
    private static readonly float constantDelay = 1;

    public static Board board;
    public bool IsPlayerTurn { get; private set; }
    [HideInInspector] public UnityEvent<PlayerCharacter> charSelected;

    private void Start()
    {
        cam = Camera.main;
        board = GetComponentInChildren<Board>();

        SetupLevel1();
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

    private void SetupLevel1()
    {
        IsPlayerTurn = true;
        
        playerUnits = new List<PlayerCharacter>
        {
            (PlayerCharacter)SpawnPiece(1, 4, gruntPrefab),
            (PlayerCharacter)SpawnPiece(1, 2, jumpshipPrefab),
            (PlayerCharacter)SpawnPiece(4, 4, tankPrefab)
        };

        drones = new List<Drone>()
        {
            (Drone)SpawnPiece(4, 1, dronePrefab),
            (Drone)SpawnPiece(4, 7, dronePrefab),
            (Drone)SpawnPiece(3, 2, dronePrefab)
        };

        dreadnoughts = new List<Dreadnought>()
        {
            (Dreadnought)SpawnPiece(5, 1, dreadnoughtPrefab),
        };

        commandUnits = new List<CommandUnit>()
        {
            (CommandUnit)SpawnPiece(7, 3, commandUnitPrefab),
        };
    }

    private static Character SpawnPiece(int row, int col, GameObject prefab)
    {
        var spawnCell = board[row, col];
        var piece = Instantiate(prefab);
        var character = piece.GetComponent<Character>();
        character.Setup(spawnCell);
        spawnCell.SetCharacter(character);
        return character;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) TrySelectCell(Input.mousePosition);
        
        if (IsPlayerTurn && AllActionsPerformed()) EndPlayerTurn();
    }
    
    private void TrySelectCell(Vector3 position)
    {
        if (!IsPlayerTurn) return;
        
        Physics.Raycast(cam.ScreenPointToRay(position), out hit, Mathf.Infinity);
        if (hit.collider.TryGetComponent( out Cell cell))
        {
            ForEachCell(c => c.UnSelectCell());
            
            if (cell.SelectCell() == CellState.Select) selectedCell = cell;
            else selectedCell = null;
        }
    }

    private bool AllActionsPerformed()
    {
        return playerUnits.All(unit => unit.HasAttacked);
    }

    public void SelectCharacter(PlayerCharacter character)
    {
        currentPlayer = character;
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
        IsPlayerTurn = false;
        HideSelectOverlay();
        StartCoroutine(PlayEnemyTurn());
    }

    private void EndAITurn()
    {
        foreach (var unit in playerUnits)
        {
            unit.ResetTurn();
        }
        IsPlayerTurn = true;
    }

    private IEnumerator PlayEnemyTurn()
    {
        yield return new WaitForSeconds(2);
        float delay = 0;
        foreach (var drone in drones)
        {
            if(drone != null)
            {
                if (drone.TryMove(ref delay)) yield return new WaitForSeconds(delay + constantDelay);
                if (drone.TryAttack(ref delay)) yield return new WaitForSeconds(delay+ constantDelay);
            }
        }
        
        foreach (var dreadnought in dreadnoughts)
        {
            if(dreadnought != null)
            {
                if (dreadnought.TryMove(ref delay)) yield return new WaitForSeconds(delay + constantDelay);
                if (dreadnought.TryAttack(ref delay)) yield return new WaitForSeconds(delay + constantDelay);
            }
        }

        foreach (var cUnit in commandUnits)
        {
            if (cUnit != null)
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
}
