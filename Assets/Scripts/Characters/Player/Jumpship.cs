using DG.Tweening;
using Grid;
using UnityEngine;

namespace Characters.Player
{
    public class Jumpship : PlayerCharacter
    {
        private Cell[] availablePsns = new Cell[8];

        private Animator anim;
        private int paramShoot;
        private int paramJump;
        private int paramDie;
        private int paramReload;
    
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            GetNewCells();

            anim = GetComponentInChildren<Animator>();
            paramShoot = Animator.StringToHash("shoot");
            paramJump = Animator.StringToHash("jump");
            paramDie = Animator.StringToHash("die");
            paramReload = Animator.StringToHash("reload");
        }
    
        private void GetNewCells()
        {
            var psn = Location.Coordinates;
            
            availablePsns[0] = BoardManager.TryGetCell(psn.x + 2, psn.y + 1);
            availablePsns[1] = BoardManager.TryGetCell(psn.x + 2, psn.y - 1);
            availablePsns[2] = BoardManager.TryGetCell(psn.x + 1, psn.y + 2);
            availablePsns[3] = BoardManager.TryGetCell(psn.x + 1, psn.y - 2);
            availablePsns[4] = BoardManager.TryGetCell(psn.x - 1, psn.y + 2);
            availablePsns[5] = BoardManager.TryGetCell(psn.x - 1, psn.y - 2);
            availablePsns[6] = BoardManager.TryGetCell(psn.x - 2, psn.y + 1);
            availablePsns[7] = BoardManager.TryGetCell(psn.x - 2, psn.y - 1);
        }
    
        protected override void ShowMoveLocations()
        {
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

            anim.SetTrigger(paramJump);
            var pos = transform.position;
            var target = cell.Position;
            var midPoint = (pos + target + new Vector3(0, 4, 0)) / 2;
            DOVirtual.Vector3(pos, midPoint, 0.65f, value => transform.position = value)
                .SetEase(Ease.Linear)
                .SetDelay(0.3f)
                .OnComplete(() =>
                {
                    DOVirtual.Vector3(midPoint, target, 0.65f, value => transform.position = value)
                        .SetEase(Ease.InSine);
                });
        }
        
        protected override void Attack(Cell cell)
        {
            
        }
    }
}
