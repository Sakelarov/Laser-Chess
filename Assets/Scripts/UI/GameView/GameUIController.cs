using Characters.Enemy;
using Characters.Models;
using Characters.Player;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    private static GameUIController instance;

    public static GameUIController Instance
    {
        get
        {
            if (instance == null)
            {
                var inst = FindObjectOfType<GameUIController>();
                instance = inst;
            }

            return instance;
        }
    }
    
    [SerializeField] private Transform playerUnitsGrid;
    [SerializeField] private Transform enemyUnitsGrid;

    [SerializeField] private UnitInfoDisplay playerDisplay;
    [SerializeField] private UnitInfoDisplay enemyDisplay;

    [SerializeField] private GameObject playerUnitPrefab;
    [SerializeField] private GameObject enemyUnitPrefab;

    [SerializeField] private Sprite[] unitPortraits;

    [SerializeField] private GameObject topMenu;
    [SerializeField] private CanvasGroup pauseMenuOverlay;
    [SerializeField] private GameObject pauseMenuBG;
    [SerializeField] private Button endTurnBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button mainMenuBtn;

    private RectTransform settingsBtnRect;
    private readonly float pauseBtnOffset = 110;

    private bool isInitialized;

    public void SetupGameView()
    {
        playerDisplay.Show();
        enemyDisplay.Show();

        if(!isInitialized) Init();
        
        UIEffects.PanelOpenTransition(topMenu);
    }

    private void Init()
    {
        settingsBtnRect = settingsBtn.GetComponent<RectTransform>();
        
        endTurnBtn.onClick.AddListener(EndTurn);
        settingsBtn.onClick.AddListener(OpenPauseMenu);
        continueBtn.onClick.AddListener(ClosePauseMenu);
        restartBtn.onClick.AddListener(RestartLevel);
        mainMenuBtn.onClick.AddListener(BackToMainMenu);
    }

    private void EndTurn()
    {
        BoardManager.Instance.EndPlayerTurn();
        DisableButtons();
        Invoke(nameof(EnableBtns), 0.6f);
    }

    private void OpenPauseMenu()
    {
        DisableButtons();
        Time.timeScale = 0;
        pauseMenuOverlay.gameObject.SetActive(true);
        DOVirtual.Float(0, 1, 0.2f, (value) => pauseMenuOverlay.alpha = value).SetUpdate(true).OnComplete(EnableBtns);
        UIEffects.PanelOpenTransition(pauseMenuBG,1, 0, null, null, true);
    }

    private void ClosePauseMenu()
    {
        DisableButtons();
        DOVirtual.Float(1, 0, 0.2f, (value) => pauseMenuOverlay.alpha = value).SetUpdate(true)
            .OnComplete(() =>
            {
                EnableBtns();
                Time.timeScale = 1;
                pauseMenuOverlay.gameObject.SetActive(false);
            });
        UIEffects.PanelCloseTransition(pauseMenuBG, 1, 0, null, null, true);
    }
    
    private void RestartLevel()
    {
        ClosePauseMenu();
        BoardManager.Instance.RestartLevel();
    }

    private void BackToMainMenu()
    {
        ClosePauseMenu();
        
        playerDisplay.Hide();
        enemyDisplay.Hide();
        UIEffects.PanelCloseTransition(topMenu, 1, 0, () =>
        {
            settingsBtnRect.offsetMax = new Vector2(-pauseBtnOffset, 0);
            settingsBtnRect.offsetMin = new Vector2(-pauseBtnOffset, 0);
            endTurnBtn.gameObject.SetActive(false);
        }, null, true);
        
        BoardManager.Instance.ResetAll();
        MainMenuController.Instance.OpenMainMenu();
    }

    private void DisableButtons()
    {
        endTurnBtn.interactable = false;
        settingsBtn.interactable = false;
        continueBtn.interactable = false;
        restartBtn.interactable = false;
        mainMenuBtn.interactable = false;
    }

    private void EnableBtns()
    {
        endTurnBtn.interactable = true;
        settingsBtn.interactable = true;
        continueBtn.interactable = true;
        restartBtn.interactable = true;
        mainMenuBtn.interactable = true;
    }

    public void SpawnCharacter(Character character)
    {
        if (character is PlayerCharacter)
        {
            var player = Instantiate(playerUnitPrefab, playerUnitsGrid);
            var unitPortrait = player.GetComponent<UnitPortrait>();
            unitPortrait.Setup(playerDisplay, character, GetPlayerSprite(character));
            character.portrait = unitPortrait;
        }
        else if (character is EnemyCharacter)
        {
            var enemy = Instantiate(enemyUnitPrefab, enemyUnitsGrid);
            var unitPortrait = enemy.GetComponent<UnitPortrait>();
            unitPortrait.Setup(enemyDisplay, character, GetEnemySprite(character));
            character.portrait = unitPortrait;
        }
    }

    private Sprite GetPlayerSprite(Character character)
    {
        if (character is Grunt) return unitPortraits[0];
        if (character is Jumpship) return unitPortraits[1];
        if (character is Tank) return unitPortraits[2];

        return null;
    }
    
    private Sprite GetEnemySprite(Character character)
    {
        if (character is Drone) return unitPortraits[3];
        if (character is Dreadnought) return unitPortraits[4];
        if (character is CommandUnit) return unitPortraits[5];

        return null;
    }

    public void UpdatePlayerDisplay(Character character) { playerDisplay.UpdateInfo(character); }
    
    public void UpdateEnemyDisplay(Character character) { enemyDisplay.UpdateInfo(character); }

    public void DeactivePlayer(Character character) { playerDisplay.DeactivateCharacter(character); }
    
    public void DeactiveEnemy(Character character) { enemyDisplay.DeactivateCharacter(character); }

    public void ClearPlayerDisplay() { playerDisplay.Clear(); }
    
    public void ClearEnemyDisplay() { enemyDisplay.Clear(); }

    public void ActivatePlayerTurn()
    {
        playerDisplay.ActivateTurnIndicator();
        enemyDisplay.DeactivateTurnIndicator();
        ClearPlayerDisplay();

        UIEffects.PanelOpenTransition(endTurnBtn.gameObject);
        DOVirtual.Float(pauseBtnOffset, 0, 0.25f, value =>
            {
                settingsBtnRect.offsetMax = new Vector2(-value, 0);
                settingsBtnRect.offsetMin = new Vector2(-value, 0);
            }).SetEase(Ease.Linear);
    }

    public void ActivateEnemyTurn()
    {
        playerDisplay.DeactivateTurnIndicator();
        enemyDisplay.ActivateTurnIndicator();
        
        DOVirtual.Float(0, pauseBtnOffset, 0.25f, value =>
        {
            settingsBtnRect.offsetMax = new Vector2(-value, 0);
            settingsBtnRect.offsetMin = new Vector2(-value, 0);
        }).SetEase(Ease.Linear);
        UIEffects.PanelCloseTransition(endTurnBtn.gameObject);
    }

    public void ResetGameUI()
    {
        ClearPlayerDisplay();
        foreach (Transform unit in playerUnitsGrid) Destroy(unit.gameObject);

        ClearEnemyDisplay();
        foreach (Transform unit in enemyUnitsGrid) Destroy(unit.gameObject);
    }
}
