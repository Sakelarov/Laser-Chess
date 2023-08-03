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

    private Camera cam;
    private RaycastHit hit;
    private Cell selectedCell;
    private PlayerCharacter currentPlayer;
    private List<PlayerCharacter> playerUnits;
    private List<Drone> drones;
    private List<Dreadnought> dreadnoughts;
    private List<CommandUnit> commandUnits;
    private static readonly int boardSize = 8;
    
    public static Board board;
    public bool IsPlayerTurn { get; private set; }
    public UnityEvent<PlayerCharacter> charSelected;

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
        StartCoroutine(PlayEnemyTurn());
    }

    private IEnumerator PlayEnemyTurn()
    {
        yield return new WaitForSeconds(2);
        foreach (var drone in drones)
        {
            if(drone != null)
            {
                if (drone.TryMove()) yield return new WaitForSeconds(2);
                if (drone.TryAttack()) yield return new WaitForSeconds(2);
            }
        }
        
        foreach (var dreadnought in dreadnoughts)
        {
            if(dreadnought != null)
            {
                if (dreadnought.TryMove()) yield return new WaitForSeconds(2);
                if (dreadnought.TryAttack()) yield return new WaitForSeconds(2);
            }
        }
    }
}
