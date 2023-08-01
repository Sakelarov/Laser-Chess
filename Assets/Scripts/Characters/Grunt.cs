using Grid;
using DG.Tweening;
using UnityEngine;

namespace Characters
{
    public class Grunt : PlayerCharacter
    {
        private Cell _topCell;
        private Cell _bottomCell;
        private Cell _leftCell;
        private Cell _rightCell;

        private Animator anim;
        private int paramSpeed;
        private int paramReload;
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            GetNewCells();

            anim = GetComponentInChildren<Animator>();
            paramSpeed = Animator.StringToHash("Speed");
        }

        private void GetNewCells()
        {
            var psn = Location.Coordinates;
           
            _topCell = BoardManager.TryGetCell(psn.x + 1, psn.y);
            _bottomCell = BoardManager.TryGetCell(psn.x - 1, psn.y);
            _leftCell = BoardManager.TryGetCell(psn.x, psn.y - 1);
            _rightCell = BoardManager.TryGetCell(psn.x, psn.y + 1);
        }

        protected override void ShowMoveLocations()
        {
            if (_topCell != null && !_topCell.IsOccupied)
                _topCell.GreenHighlight();
            if (_bottomCell != null &&!_bottomCell.IsOccupied)
                _bottomCell.GreenHighlight();
            if (_leftCell != null &&!_leftCell.IsOccupied)
                _leftCell.GreenHighlight();
            if (_rightCell != null &&!_rightCell.IsOccupied)
                _rightCell.GreenHighlight();
        }

        protected override void ShowAttackLocations()
        {
            throw new System.NotImplementedException();
        }

        protected override void HideLocations()
        {
            if (_topCell != null) 
                _topCell.DisableHighlight();
            if (_bottomCell != null)
                _bottomCell.DisableHighlight();
            if (_leftCell != null)
                _leftCell.DisableHighlight();
            if (_rightCell != null)
                _rightCell.DisableHighlight();
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
            DOVirtual.Vector3(pos, cell.Position, 1, value => transform.position = value);
            DOVirtual.Float(0, 1, 0.5f, value => anim.SetFloat(paramSpeed, value))
                .SetEase(Ease.InOutCubic)
                .SetLoops(2, LoopType.Yoyo);
        }
    }
}
