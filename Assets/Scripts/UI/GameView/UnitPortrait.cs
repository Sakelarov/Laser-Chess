using System.Collections;
using System.Collections.Generic;
using Characters.Enemy;
using Characters.Models;
using Grid;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI location;
    [SerializeField] private GameObject canMoveIndicator;
    [SerializeField] private GameObject canAttackIndicator;
    [SerializeField] private GameObject selectedOverlay;
    [SerializeField] private Button selectBtn;

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

    private void SetupIndicatiors()
    {
        if (_character is PlayerCharacter ch)
        {
            canMoveIndicator.SetActive(ch.CanMove);
            canAttackIndicator.SetActive(ch.CanAttack);
        }
        else
        {
            canMoveIndicator.SetActive(!_character.HasMoved);
            canAttackIndicator.SetActive(!_character.HasAttacked && !(_character is CommandUnit));
        }
    }

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
                selectedOverlay.SetActive(true);
                _info.Setup(_character);
            }
        }
        else if (_character is EnemyCharacter)
        {
            selectedOverlay.SetActive(true);
            _info.Setup(_character);
        }
    }

    public void Select()
    {
        if (selectedOverlay.activeSelf) return;
        
        selectedOverlay.SetActive(true);
        _info.Setup(_character);
        _character.Location.SelectCell();
    }

    public void UnselectPortrait()
    {
        selectedOverlay.SetActive(false);
    }

    public void DisableMoveIndicator()
    {
        canMoveIndicator.SetActive(false);
    }

    public void DisableAttackIndicator()
    {
        canAttackIndicator.SetActive(false);
    }

    public void UpdateActions()
    {
        if (_character is PlayerCharacter ch)
        {
            canMoveIndicator.SetActive(ch.CanMove);
            canAttackIndicator.SetActive(ch.CanAttack);
        }
        else if (_character is EnemyCharacter enemy)
        {
            canMoveIndicator.SetActive(!enemy.HasMoved);
            canAttackIndicator.SetActive(!enemy.HasAttacked);
        }
    }

    public void UpdatePosition()
    {
        location.text = UnitInfoDisplay.GetPosition(_character.Location.Coordinates);
    }

    public void ResetTurn()
    {
        if(_character == null || _character.IsDead) return;
        
        canMoveIndicator.SetActive(true);
        
        if (_character is CommandUnit) canAttackIndicator.SetActive(false);
        else canAttackIndicator.SetActive(true);
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
