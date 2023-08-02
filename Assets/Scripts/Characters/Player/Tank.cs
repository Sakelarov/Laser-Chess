using System.Collections.Generic;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Characters.Player
{
    public class Tank : PlayerCharacter
    {
        private List<Cell> availablePsns = new List<Cell>();
        private (int, int)[] positionIndices = new (int, int)[8];
        
        // Define the directions the queen can move (horizontally, vertically, and diagonally)
        private int[] dr = { 0, 0, 1, -1, 1, -1, 1, -1 };
        private int[] dc = { 1, -1, 0, 0, 1, 1, -1, -1 };
        
        private Animator anim;
        private int paramRun;
        private int paramShoot;
        private int paramDie;
        private int paramReload;
        private readonly int maxMoveDistance = 3;
        
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            GetNewCells();

            anim = GetComponentInChildren<Animator>();
            paramShoot = Animator.StringToHash("shoot");
            paramRun = Animator.StringToHash("run");
            paramDie = Animator.StringToHash("die");
            paramReload = Animator.StringToHash("reload");
        }

        private void GetNewCells()
        {
            var psn = Location.Coordinates;

            availablePsns = new List<Cell>();

            for (int i = 0; i < dr.Length; i++)
            {
                int r = psn.x;
                int c = psn.y;

                for (int moveDistance = 1; moveDistance <= maxMoveDistance; moveDistance++)
                {
                    r += dr[i];
                    c += dc[i];

                    var cell = BoardManager.TryGetCell(r, c);
                    if (cell != null)
                    {
                        if (!cell.IsOccupied) availablePsns.Add(cell);
                        else
                        {
                            // The position is blocked by another piece, so we stop searching in this direction
                            break;
                        }
                    }
                    else
                    {
                        // The position is outside the chessboard, so we stop searching in this direction
                        break;
                    }
                }
            }
        }
        protected override void ShowMoveLocations()
        {
            GetNewCells();
            foreach (var cell in availablePsns)
            {
                if(cell != null && !cell.IsOccupied) cell.GreenHighlight();
            }
        }
        
        protected override void ShowAttackLocations()
        {
            
        }

        protected override void HideLocations()
        {
            foreach (var cell in availablePsns)
            {
                if(cell != null) cell.DisableHighlight();
            }
        }

        protected override void Move(Cell cell)
        {
            if (!Bm.IsCurrentlySelected(this)) return;
            
            transform.LookAt(cell.Position);
            
            HasMoved = true;
            HideLocations();
            
            Location.SetCharacter(null);
            Location = cell;
            cell.SetCharacter(this);
            GetNewCells();
            
            var pos = transform.position;
            anim.SetBool(paramRun, true);
            float speed = Vector3.Distance(transform.position, cell.Position) * 0.6f;
            DOVirtual.Vector3(pos, cell.Position, speed, value => transform.position = value)
                .SetEase(Ease.Linear)
                .OnComplete(() => anim.SetBool(paramRun, false));
        }
        
        protected override void Attack(Cell cell)
        {
            
        }
    }
}
