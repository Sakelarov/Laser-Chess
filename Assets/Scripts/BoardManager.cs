using System;
using Characters;
using Grid;
using UnityEngine;

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

    private Camera cam;
    private RaycastHit hit;
    private Cell selectedCell;
    private PlayerCharacter currentPlayer;
    
    
    [HideInInspector] public Board board;

    private void Start()
    {
        cam = Camera.main;
        board = GetComponent<Board>();

        SetupLevel1();
    }

    public void ForEachCell(Action<Cell> action)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                action.Invoke(board[i,j]);
            }
        }
    }

    private void SetupLevel1()
    {
        var gruntCell = board[1, 4];
        var gruntEnemy = Instantiate(gruntPrefab);
        var grunt = gruntEnemy.GetComponent<Grunt>();
        grunt.Setup(gruntCell);
        gruntCell.SetCharacter(grunt);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) TrySelectCell(Input.mousePosition);
    }

    private void TrySelectCell(Vector3 position)
    {
        Physics.Raycast(cam.ScreenPointToRay(position), out hit, Mathf.Infinity);
        if (hit.collider.TryGetComponent( out Cell cell))
        {
            if (cell.SelectCell() == CellState.Select) selectedCell = cell;
        }
    }
}
