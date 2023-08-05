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
using UnityEngine.EventSystems;

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
    private EventSystem eventSys;
    private RaycastHit hit;
    private Cell selectedCell;
    private PlayerCharacter currentPlayer;
    private List<PlayerCharacter> playerUnits;
    private List<Drone> drones;
    private List<Dreadnought> dreadnoughts;
    private List<CommandUnit> commandUnits;
    private static readonly int boardSize = 8;
    private static readonly float constantDelay = 1;

    public enum LevelType { None, Level1, Level2, Level3, }

    public static Board board;
    public bool IsPlayerTurn { get; private set; }
    [HideInInspector] public UnityEvent<PlayerCharacter> charSelected;

    private void Start()
    {
        cam = Camera.main;
        eventSys = EventSystem.current;
        board = GetComponentInChildren<Board>();
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

    public void StartLevel(LevelType level)
    {
        GameUIController.Instance.SetupGameView();
        
        switch (level)
        {
            case LevelType.Level1 : StartCoroutine(SetupLevel1()); break;
            //case LevelType.Level2 : StartCoroutine(SetupLevel1()); break;
            //case LevelType.Level3 : StartCoroutine(SetupLevel1()); break;
        }
    }

    private static Character SpawnPiece(int row, int col, GameObject prefab)
    {
        var spawnCell = board[row, col];
        var piece = Instantiate(prefab);
        
        var character = piece.GetComponent<Character>();
        character.Setup(spawnCell);
        spawnCell.SetCharacter(character);
        
        GameUIController.Instance.SpawnCharacter(character);
        
        return character;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) TrySelectCell(Input.mousePosition);
        
        if (IsPlayerTurn && AllActionsPerformed()) EndPlayerTurn();
        
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        // }

        // if (Input.GetMouseButton(0))
        // {
        //     PointerEventData pointer = new PointerEventData(EventSystem.current);
        //     pointer.position = Input.mousePosition;
        //
        //     List<RaycastResult> raycastResults = new List<RaycastResult>();
        //     EventSystem.current.RaycastAll(pointer, raycastResults);
        //
        //     if (raycastResults.Count > 0)
        //     {
        //         foreach (var go in raycastResults)
        //         {
        //             Debug.Log(go.gameObject.name, go.gameObject);
        //         }
        //     }
        // }
    }
    
    private void TrySelectCell(Vector3 position)
    {
        if (!IsPlayerTurn) return;
        
        Physics.Raycast(cam.ScreenPointToRay(position), out hit, Mathf.Infinity);
        if (hit.collider != null && hit.collider.TryGetComponent( out Cell cell))
        {
            UnselectAllCells();

            if (cell.SelectCell() == CellState.Select) SelectCharacter((PlayerCharacter)cell.Character);
            else selectedCell = null;
        }
    }

    public void UnselectAllCells()
    {
        ForEachCell(c => c.UnSelectCell());
    }

    private bool AllActionsPerformed()
    {
        return playerUnits.All(unit => unit.HasAttacked);
    }

    public void SelectCharacter(PlayerCharacter character)
    {
        selectedCell = character.Location;
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

    public void EnemyWin()
    {
        Debug.Log("Enemy Wins");
    }

    public void CheckPlayerWin()
    {
        bool hasCommandUnits = false;

        foreach (var unit in commandUnits)
        {
            if (unit != null)
            {
                hasCommandUnits = true;
                break;
            }
        }
        
        if(!hasCommandUnits) Debug.Log("Player Wins");
    }
    
    #region Levels
    
    private IEnumerator SetupLevel1()
    {
        playerUnits = new List<PlayerCharacter>();
        yield return new WaitForSeconds(0.2f);
        playerUnits.Add((PlayerCharacter)SpawnPiece(1, 4, gruntPrefab));
        yield return new WaitForSeconds(0.2f);
        playerUnits.Add((PlayerCharacter)SpawnPiece(1, 2, jumpshipPrefab));
        yield return new WaitForSeconds(0.2f);
        playerUnits.Add((PlayerCharacter)SpawnPiece(4, 4, tankPrefab));
        yield return new WaitForSeconds(1.0f);

        drones = new List<Drone>();
        drones.Add((Drone)SpawnPiece(4, 1, dronePrefab));
        yield return new WaitForSeconds(0.2f);
        drones.Add((Drone)SpawnPiece(3, 2, dronePrefab));
        yield return new WaitForSeconds(0.2f);

        dreadnoughts = new List<Dreadnought>();
        dreadnoughts.Add((Dreadnought)SpawnPiece(5, 1, dreadnoughtPrefab));
        yield return new WaitForSeconds(0.2f);

        commandUnits = new List<CommandUnit>() { (CommandUnit)SpawnPiece(7, 3, commandUnitPrefab) };

        IsPlayerTurn = true;
    }
    
    #endregion
}
