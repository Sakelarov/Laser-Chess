using Characters.Models;
using Grid;
using UnityEngine;

public abstract class PlayerCharacter : Character
{
    protected bool IsSelected;

    public override void Setup(Cell cell)
    {
        base.Setup(cell);
        
        Bm.ForEachCell(c => c.moveToCell.AddListener(Move));
    }

    public virtual void SelectCharacter()
    {
        if (!IsSelected)
        {
            IsSelected = true;
            ShowMoveLocations();
        }
    }

    protected abstract void ShowMoveLocations();

    protected abstract void HideMoveLocations();

    protected abstract void Move(Cell cell);
}
