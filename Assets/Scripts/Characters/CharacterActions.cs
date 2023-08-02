using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Grid;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    // Define the diagonal directions
    private static readonly int[] dr = { 1, -1, 1, -1 };
    private static readonly int[] dc = { 1, 1, -1, -1 };

    private static readonly string flashName = "muzzleFlash";
    
    public static List<Cell> GetAvailableDiagonalTargets<T>(Cell location, List<Cell> directionCells = null)
    {
        var psn = location.Coordinates;
        var results = new List<Cell>();

        for (int i = 0; i < dr.Length; i++)
        {
            int r = psn.x;
            int c = psn.y;

            while (true)
            {
                r += dr[i];
                c += dc[i];

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

    private static void GetDirectionToTarget(Vector2Int psn, List<Cell> directionCells, int i, Cell target)
    {
        int dirR = psn.x;
        int dirC = psn.y;
        while (true)
        {
            dirR += dr[i];
            dirC += dc[i];
            var directionCell = BoardManager.TryGetCell(dirR, dirC);
            if (directionCell != null && directionCell != target)
                directionCells.Add(directionCell);
            else break;
        }

        directionCells.Add(target);
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