using Characters.Enemy;
using Characters.Models;
using Characters.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI unitPosition;
    [SerializeField] private GameObject[] hpBars;
    [SerializeField] private GameObject[] atkBars;
    // [SerializeField] private GameObject canMoveIndicator;
    // [SerializeField] private GameObject canAttackIndicator;
    // [SerializeField] private GameObject moveDisabledIndicator;
    // [SerializeField] private GameObject attackDisabledIndicator;
    [SerializeField] private Button moveBtn;
    [SerializeField] private Button attackBtn;
    [SerializeField] private GameObject parent;
    [SerializeField] private Image background;

    [SerializeField] private bool isPlayerUI;

    private readonly Color playerUnselectedColor = new Color(0.85f, 0.45f, 0, 0.3f);
    private readonly Color playerSelectedColor = new Color(0.85f, 0.45f, 0, 0.6f);
    
    private readonly Color enemyUnselectedColor = new Color(1, 1, 1, 0.1f);
    private readonly Color enemySelectedColor = new Color(1, 1f, 1, 0.2f);

    private Character _character;

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

        if (character.HasMoved)
        {
            moveBtn.image.color = Color.black;
            moveBtn.enabled = false;
        }
        else
        {
            moveBtn.image.color = Color.green;
            if (character is PlayerCharacter pl)
            {
                moveBtn.onClick.RemoveAllListeners();
                moveBtn.onClick.AddListener(pl.ShowMoveLocations);
                moveBtn.enabled = true;
            }
        }
        
        if (character.HasAttacked || character is CommandUnit)
        {
            attackBtn.image.color = Color.black;
            attackBtn.enabled = false;
        }
        else
        {
            attackBtn.image.color = Color.green;
            if (character is PlayerCharacter pl)
            {
                attackBtn.onClick.RemoveAllListeners();
                attackBtn.onClick.AddListener(pl.ShowAttackLocations);
                attackBtn.enabled = true;
            }
        }

        Display();
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
    
    public void Deactivate()
    {
        if (isPlayerUI) background.color = playerUnselectedColor;
        else background.color = enemyUnselectedColor;
        
        _character.portrait.UnselectPortrait();
        _character = null;
        parent.SetActive(false);
    }

    public void DeactivateCharacter(Character character)
    {
        if (character == _character) Deactivate();
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
