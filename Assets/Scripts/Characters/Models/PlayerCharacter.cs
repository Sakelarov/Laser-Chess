using Characters.Models;
using Grid;
using Utils;

public abstract class PlayerCharacter : Character
{
    protected bool IsSelected;

    protected bool moveLocationsDisplayed;
    protected bool attackLocationsDisplayed;

    public bool CanMove { get; set; }
    public bool CanAttack { get; set; }
    public bool HasActions => (!HasMoved && CanMove) || (!HasAttacked && CanAttack);

    public override void Setup(Cell cell)
    {
        base.Setup(cell);
        
        Bm.ForEachCell(c =>
        {
            c.moveToCell.AddListener(Move);
            c.attackCell.AddListener(Attack);
        });
        Bm.charSelected.AddListener(CheckSelectedCharacter);
        
        Bm.allEnemiesSpawned.AddListener(StartTurn);
    }

    protected virtual void StartTurn()
    {
        GetMovementCells();
        GetAttackTargets();
        portrait.UpdateActions();
    }

    protected virtual void SelectCharacter()
    {
        IsSelected = true;

        if (CanMove)
        {
            ShowMoveLocations();
            moveLocationsDisplayed = true;
            portrait.BlinkMove();
        }
        else if (CanAttack)
        {
            ShowAttackLocations();
            attackLocationsDisplayed = true;
            portrait.BlinkAttack();
        }
    }

    public void UnSelectCharacter()
    {
        IsSelected = false;
        HideLocations();
    }

    protected void CheckSelectedCharacter(PlayerCharacter character)
    {
        if (IsSelected)
        {
            if (this != character) UnSelectCharacter();
            else
            {
                if (moveLocationsDisplayed && CanAttack)
                {
                    Bm.ForEachCell(c => c.DisableHighlight());
                    moveLocationsDisplayed = false;
                    attackLocationsDisplayed = true;
                    portrait.BlinkAttack();
                    ShowAttackLocations();
                }
                else if (attackLocationsDisplayed && CanMove)
                {
                    Bm.ForEachCell(c => c.DisableHighlight());
                    attackLocationsDisplayed = false;
                    moveLocationsDisplayed = true;
                    portrait.BlinkMove();
                    ShowMoveLocations();
                }
            }
        }
        else if (!IsSelected && this == character) this.InvokeAfterFrames(1,SelectCharacter);
    }

    public abstract void GetMovementCells();
    
    public abstract void ShowMoveLocations();

    public abstract void GetAttackTargets();
    
    public abstract void ShowAttackLocations();

    protected abstract void HideLocations();

    protected abstract void Move(Cell cell);
    
    protected abstract void Attack(Cell cell);
    
    public override void ResetTurn()
    {
        moveLocationsDisplayed = false;
        attackLocationsDisplayed = false;
        HasMoved = false;
        HasAttacked = false;
        StartTurn();
    }

    protected virtual void RegisterAttack()
    {
        HasMoved = true; // player cant move after attack
        CanMove = false;
        HasAttacked = true;
        CanAttack = false;
        
        HideLocations();
        Bm.HideSelectOverlay();
        GameUIController.Instance.UpdatePlayerDisplay(this);
    }

    protected virtual void RegisterMove(Cell cell)
    {
        HasMoved = true;
        CanMove = false;
        
        HideLocations();
        Bm.HideSelectOverlay();

        Location.SetCharacter(null);
        Location = cell;
        cell.SetCharacter(this);
        GetMovementCells();
        Bm.CheckActions();
        portrait.UpdatePosition();
    }

    public override void Die()
    {
        base.Die();
        GameUIController.Instance.DeactivePlayer(this);
        Bm.CheckForEnemyWin();
    }
}
