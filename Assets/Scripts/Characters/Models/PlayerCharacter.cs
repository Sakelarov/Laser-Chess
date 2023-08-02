using Characters.Models;
using Grid;
using UnityEngine;
using Utils;

public abstract class PlayerCharacter : Character
{
    protected bool IsSelected;

    public override void Setup(Cell cell)
    {
        base.Setup(cell);
        
        Bm.ForEachCell(c =>
        {
            c.moveToCell.AddListener(Move);
            c.attackCell.AddListener(Attack);
        });
        Bm.charSelected.AddListener(CheckSelectedCharacter);
    }

    protected virtual void SelectCharacter()
    {
        IsSelected = true;
        
        if (!HasMoved) ShowMoveLocations();
        else if (!HasAttacked) ShowAttackLocations();
    }

    protected virtual void UnSelectCharacter()
    {
        IsSelected = false;
        HideLocations();
    }

    protected void CheckSelectedCharacter(PlayerCharacter character)
    {
        if (IsSelected && this != character) UnSelectCharacter();
        else if (!IsSelected && this == character) this.InvokeAfterFrames(1,SelectCharacter);
    }

    protected abstract void ShowMoveLocations();
    
    protected abstract void ShowAttackLocations();

    protected abstract void HideLocations();

    protected abstract void Move(Cell cell);
    
    protected abstract void Attack(Cell cell);
}
