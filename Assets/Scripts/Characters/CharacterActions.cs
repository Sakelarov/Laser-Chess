using System.Collections.Generic;
using Grid;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    // Define the diagonal directions
    private static readonly int[] dr = { 1, -1, 1, -1 };
    private static readonly int[] dc = { 1, 1, -1, -1 };
    
    // Define the orthogonal directions
    private static readonly int[] or = { 0, 0, 1, -1 };
    private static readonly int[] oc = { 1, -1, 0, 0 };
    
    // Define all possible directions
    private static readonly int[] ar = { 1, -1, 0, 0, 1, -1, 1, -1 };
    private static readonly int[] ac = { 0, 0, 1, -1, 1, 1, -1, -1 };

    private static int[] dirR;
    private static int[] dirC;

    private static readonly string flashName = "muzzleFlash";

    public enum DirectionType
    {
        Orthogonal,
        Diagonal,
        AllDirections
    }
    

    public static List<Cell> GetAvailableTargets<T>(Cell location, DirectionType dirType, List<Cell> directionCells = null)
    {
        var psn = location.Coordinates;
        var results = new List<Cell>();

        GetDirections(dirType);

        for (int i = 0; i < dirR.Length; i++)
        {
            int r = psn.x;
            int c = psn.y;

            while (true)
            {
                r += dirR[i];
                c += dirC[i];

                var cell = BoardManager.TryGetCell(r, c);
                if (cell != null)
                {
                    if (cell.IsOccupied)
                    {
                        if (cell.Character is T)
                        {
                            results.Add(cell);
                            if (directionCells != null) GetDirectionToTarget(psn, directionCells, i, cell);
                        }

                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return results;
    }
    
    private static void GetDirections(DirectionType dirType)
    {
        if (dirType == DirectionType.Orthogonal)
        {
            dirR = or;
            dirC = oc;
        }
        else if (dirType == DirectionType.Diagonal)
        {
            dirR = dr;
            dirC = dc;
        }
        else if (dirType == DirectionType.AllDirections)
        {
            dirR = ar;
            dirC = ac;
        }
        else
        {
            Debug.LogError("Wrong direction type");
        }
    }

    private static void GetDirectionToTarget(Vector2Int psn, List<Cell> directionCells, int i, Cell target)
    {
        int psnX = psn.x;
        int psnY = psn.y;
        while (true)
        {
            psnX += dirR[i];
            psnY += dirC[i];
            var directionCell = BoardManager.TryGetCell(psnX, psnY);
            if (directionCell != null && directionCell != target)
                directionCells.Add(directionCell);
            else break;
        }

        directionCells.Add(target);
    }

    public static List<Cell> GetCellsInManhattanDistance(Cell cell, int manhattanDistance)
    {
        var psn = cell.Coordinates;
        List<Cell> result = new List<Cell>();

        for (int dx = -manhattanDistance; dx <= manhattanDistance; dx++)
        {
            for (int dy = -manhattanDistance; dy <= manhattanDistance; dy++)
            {
                var currentCell = BoardManager.TryGetCell(psn.x + dx, psn.y + dy);
                if (currentCell != null && currentCell != cell)
                {
                    result.Add(currentCell);
                }
            }
        }

        return result;
    }
    
    private static List<Cell> GetCellsAtManhattanDistance(Cell cell, int manhattanDistance)
    {
        var psn = cell.Coordinates;
        List<Cell> result = new List<Cell>();

        for (int dx = -manhattanDistance; dx <= manhattanDistance; dx++)
        {
            for (int dy = -manhattanDistance; dy <= manhattanDistance; dy++)
            {
                var currentCell = BoardManager.TryGetCell(psn.x + dx, psn.y + dy);
                if (currentCell != null && GetManhattanDistance(currentCell, cell) == manhattanDistance)
                {
                    result.Add(currentCell);
                }
            }
        }

        return result;
    }

    public static int GetManhattanDistance(Cell cell1, Cell cell2)
    {
        var c1Psn = cell1.Coordinates;
        var c2Psn = cell2.Coordinates;
        int xDistance = Mathf.Abs(c2Psn.x - c1Psn.x);
        int yDistance = Mathf.Abs(c2Psn.y - c1Psn.y);
        return Mathf.Max(xDistance, yDistance);
    }
    
    public static void ShootLaser(Transform chTransform, GameObject shotPrefab, Transform shotPosition, Cell target)
    {
        var shot = Instantiate(shotPrefab);
        shot.transform.SetPositionAndRotation(shotPosition.position, shotPosition.rotation);
        
        var sb = shot.GetComponent<ShotBehavior>();
        sb.Setup(Vector3.Distance(chTransform.position, target.Position));
        Destroy(shot, 3f);
        
        var flash = Instantiate(Resources.Load(flashName, typeof(GameObject))) as GameObject;
        flash.transform.SetPositionAndRotation(shotPosition.position, shotPosition.rotation);
    }
}