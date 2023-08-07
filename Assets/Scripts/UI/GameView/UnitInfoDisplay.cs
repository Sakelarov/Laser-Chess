using System;
using Characters.Enemy;
using Characters.Models;
using Characters.Player;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI unitPosition;
    [SerializeField] private GameObject[] hpBars;
    [SerializeField] private GameObject[] atkBars;
    [SerializeField] private Button moveBtn;
    [SerializeField] private Button attackBtn;
    [SerializeField] private GameObject parent;
    [SerializeField] private Image background;
    [SerializeField] private Image turnIndicator;

    [SerializeField] private bool isPlayerUI;

    private readonly Color turnIndicatorInactive = new Color(1, 1, 1, 0.5f);
    private readonly Color playerturnActive = new Color(1f, 0f, 1, 1f);
    private readonly Color enemyturnActive = new Color(0, 0.8f, 1f, 1f);

    private readonly Color playerUnselectedColor = new Color(1f, 0, 1, 0.25f);
    private readonly Color playerSelectedColor = new Color(1f, 0, 1, 0.5f);
    
    private readonly Color enemyUnselectedColor = new Color(1, 1, 1, 0.2f);
    private readonly Color enemySelectedColor = new Color(1, 1f, 1, 0.4f);

    private Character _character;
    private Tweener tween;

    public void Setup(Character character)
    {
        if (_character != null) _character.portrait.UnselectPortrait();
        _character = character;
        unitName.text = GetUnitName(character);
        unitPosition.text = GetPosition(character.Location.Coordinates);

        for (int i = 0; i < hpBars.Length; i++)
        {
            if(character.HealthPoints >= i + 1) hpBars[i].SetActive(true);
            else hpBars[i].SetActive(false);
            
            if(character.AttackPonints >= i + 1) atkBars[i].SetActive(true);
            else atkBars[i].SetActive(false);
        }

        SetupIndicators(character);
        
        Display();
    }

    private void SetupIndicators(Character character)
    {
        if (character is PlayerCharacter player)
        {
            if (player.CanMove)
            {
                moveBtn.image.color = Color.green;
                moveBtn.onClick.RemoveAllListeners();
                moveBtn.onClick.AddListener(player.ShowMoveLocations);
                moveBtn.enabled = true;
            }
            else
            {
                moveBtn.image.color = Color.black;
                moveBtn.enabled = false;
            }

            if (player.CanAttack)
            {
                attackBtn.image.color = Color.green;
                attackBtn.onClick.RemoveAllListeners();
                attackBtn.onClick.AddListener(player.ShowAttackLocations);
                attackBtn.enabled = true;
            }
            else
            {
                attackBtn.image.color = Color.black;
                attackBtn.enabled = false;
            }
        }
        else if (character is EnemyCharacter enemy)
        {
            if (enemy.HasMoved)
            {
                moveBtn.image.color = Color.black;
                moveBtn.enabled = false;
            }
            else
            {
                moveBtn.image.color = Color.green;
            }
        
            if (enemy.HasAttacked || enemy is CommandUnit)
            {
                attackBtn.image.color = Color.black;
                attackBtn.enabled = false;
            }
            else
            {
                attackBtn.image.color = Color.green;
            }
        }
    }

    public void UpdateInfo(Character character)
    {
        if(_character == character) Setup(character);
    }

    public static string GetPosition(Vector2Int coordinates)
    {
        var letter = "";
        switch (coordinates.y)
        {
            case 0: letter = "A"; break;
            case 1: letter = "B"; break;
            case 2: letter = "C"; break;
            case 3: letter = "D"; break;
            case 4: letter = "E"; break;
            case 5: letter = "F"; break;
            case 6: letter = "G"; break;
            case 7: letter = "H"; break;
        }

        return letter + (coordinates.x + 1);
    }

    public static string GetUnitName(Character character)
    {
        if (character is Grunt) return "Grunt";
        if (character is Jumpship) return "Jumpship";
        if (character is Tank) return "Tank";
        if (character is Drone) return "Drone";
        if (character is Dreadnought) return "Dreadnought";
        if (character is CommandUnit) return "Command Unit";

        return "";
    }

    private void Display()
    {
        if (isPlayerUI) background.color = playerSelectedColor;
        else background.color = enemySelectedColor;
        
        parent.SetActive(true);
    }
    
    public void Clear()
    {
        if (isPlayerUI) background.color = playerUnselectedColor;
        else background.color = enemyUnselectedColor;
        
        parent.SetActive(false);
        
        if (_character != null)
        {
            _character.portrait.UnselectPortrait();
            _character = null;
        }
    }

    public void DeactivateCharacter(Character character)
    {
        if (character == _character) Clear();
    }

    public void Show()
    {
        UIEffects.PanelOpenTransition(gameObject);
    }

    public void Hide()
    {
        UIEffects.PanelCloseTransition(gameObject);
        DeactivateTurnIndicator();
    }

    public void ActivateTurnIndicator()
    {
        if (isPlayerUI)
        {
            tween = DOVirtual.Color(turnIndicatorInactive, playerturnActive, 1, value => { turnIndicator.color = value; })
                .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }
        else
        {
            tween = DOVirtual.Color(turnIndicatorInactive, enemyturnActive, 1, value => { turnIndicator.color = value; })
                .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }
    }

    public void DeactivateTurnIndicator()
    {
        if (tween.IsActive()) tween.Kill();
        turnIndicator.color = turnIndicatorInactive;
    }
}
