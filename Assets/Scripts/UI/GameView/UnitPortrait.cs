using System.Collections;
using System.Collections.Generic;
using Characters.Enemy;
using Characters.Models;
using DG.Tweening;
using Grid;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI location;
    [SerializeField] private Image canMoveIndicator;
    [SerializeField] private Image canAttackIndicator;
    [SerializeField] private GameObject selectedOverlay;
    [SerializeField] private Button selectBtn;
    
    private readonly Color greenTransp = new Color(0, 1, 0, 0);

    private Tweener moveTween;
    private Tweener attackTween;

    private Character _character;
    private UnitInfoDisplay _info;

    public void Setup(UnitInfoDisplay info, Character character, Sprite sprite)
    {
        _character = character;
        _info = info;
        
        if (selectBtn != null) selectBtn.onClick.AddListener(SelectCharacter);

        portrait.sprite = sprite;
        location.text = UnitInfoDisplay.GetPosition(character.Location.Coordinates);

        selectedOverlay.SetActive(false);
        
        Show();
    }

    // private void SetupIndicatiors()
    // {
    //     if (_character is PlayerCharacter ch)
    //     {
    //         canMoveIndicator.SetActive(ch.CanMove);
    //         canAttackIndicator.SetActive(ch.CanAttack);
    //     }
    //     else
    //     {
    //         canMoveIndicator.SetActive(!_character.HasMoved);
    //         canAttackIndicator.SetActive(!_character.HasAttacked && !(_character is CommandUnit));
    //     }
    // }

    private void SelectCharacter()
    {
        BoardManager.Instance.UnselectAllCells();


        if (_character is PlayerCharacter pl)
        {
            if (BoardManager.Instance.IsPlayerTurn && _character.Location.SelectCell() == CellState.Select)
            {
                BoardManager.Instance.SelectCharacter(pl);
            }
            else
            {
                _info.Setup(_character);
                selectedOverlay.SetActive(true);
            }
        }
        else if (_character is EnemyCharacter)
        {
            _info.Setup(_character);
            selectedOverlay.SetActive(true);
        }
    }

    public void Select()
    {
        if (selectedOverlay.activeSelf) return;
        
        _info.Setup(_character);
        selectedOverlay.SetActive(true);
        _character.Location.SelectCell();
    }

    public void BlinkMove()
    {
        StopBlinks();
        
        moveTween = DOVirtual.Color(Color.green, greenTransp, 0.65f, v => canMoveIndicator.color = v)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(canMoveIndicator.gameObject, LinkBehaviour.KillOnDisable);
    }

    public void BlinkAttack()
    {
        StopBlinks();
        
        attackTween = DOVirtual.Color(Color.green, greenTransp, 0.65f, v => canAttackIndicator.color = v)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(canMoveIndicator.gameObject, LinkBehaviour.KillOnDisable);
    }

    public void StopBlinks()
    {
        if (_character is PlayerCharacter ch)
        {
            if(attackTween.IsActive()) attackTween.Kill();
            if(moveTween.IsActive()) moveTween.Kill();

            if (ch.CanMove) canMoveIndicator.color = Color.green;
            if(ch.CanAttack) canAttackIndicator.color = Color.green;
        }
    }

    public void UnselectPortrait()
    {
        selectedOverlay.SetActive(false);
        StopBlinks();
    }

    public void DisableMoveIndicator()
    {
        if (moveTween.IsActive()) moveTween.Kill();
        canMoveIndicator.gameObject.SetActive(false);
    }

    public void DisableAttackIndicator()
    {
        if (attackTween.IsActive()) attackTween.Kill();
        canAttackIndicator.gameObject.SetActive(false);
    }

    public void UpdateActions()
    {
        if (gameObject == null) return;
        
        if (_character is PlayerCharacter ch)
        {
            canMoveIndicator.gameObject.SetActive(ch.CanMove);
            canAttackIndicator.gameObject.SetActive(ch.CanAttack);
        }
        else if (_character is EnemyCharacter enemy)
        {
            canMoveIndicator.gameObject.SetActive(!enemy.HasMoved);
            canAttackIndicator.gameObject.SetActive(!enemy.HasAttacked);
        }
    }

    public void UpdatePosition()
    {
        location.text = UnitInfoDisplay.GetPosition(_character.Location.Coordinates);
    }

    public void ResetTurn()
    {
        if(_character == null || _character.IsDead) return;
        
        canMoveIndicator.color = Color.green;
        canMoveIndicator.gameObject.SetActive(true);
        
        if (_character is CommandUnit) canAttackIndicator.gameObject.SetActive(false);
        else
        {
            canAttackIndicator.color = Color.green;
            canAttackIndicator.gameObject.SetActive(true);
        }
    }

    public void Show()
    {
        UIEffects.PanelOpenTransition(gameObject);
    }

    public void Hide()
    {
        UIEffects.PanelCloseTransition(gameObject);
    }
}
