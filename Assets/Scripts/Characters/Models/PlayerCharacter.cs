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

    protected abstract void GetMovementCells();
    public abstract void ShowMoveLocations();
    
    public abstract void ShowAttackLocations();

    protected abstract void HideLocations();

    protected abstract void Move(Cell cell);
    
    protected abstract void Attack(Cell cell);

    protected virtual void RegisterAttack()
    {
        HasMoved = true; // player cant move after attack
        HasAttacked = true;
        portrait.DisableAttackIndicator();
        portrait.DisableMoveIndicator();
        HideLocations();
        Bm.HideSelectOverlay();
        GameUIController.Instance.UpdatePlayerInfo(this);
    }

    protected virtual void RegisterMove(Cell cell)
    {
        HasMoved = true;
        portrait.DisableMoveIndicator();
        HideLocations();
        Bm.HideSelectOverlay();
        GameUIController.Instance.UpdatePlayerInfo(this);
            
        Location.SetCharacter(null);
        Location = cell;
        cell.SetCharacter(this);
        GetMovementCells();
    }

    public override void Die()
    {
        base.Die();
        GameUIController.Instance.DeactivePlayer(this);
    }
}
