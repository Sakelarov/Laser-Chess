using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomLevelsPopup : Popup
{
    [SerializeField] private Button chessLevelBtn;
    [SerializeField] private Button gruntWarLevelBtn;
    [SerializeField] private Button threeManArmyBtn;
    [SerializeField] private Button hardcoreLevelBtn;

    [SerializeField] private Button[] reduceBtns;
    [SerializeField] private Button[] increaseBtns;
    [SerializeField] private TextMeshProUGUI[] amounts;
    
    [SerializeField] private Button generateLevelBtn;
    [SerializeField] private Button closeBtn;

    private void Start()
    {
        AddListeners();
    }

    private void AddListeners()
    {
        chessLevelBtn.onClick.AddListener(() => StartLevel(BoardManager.LevelType.Chess));
        gruntWarLevelBtn.onClick.AddListener(() => StartLevel(BoardManager.LevelType.GruntWar));
        threeManArmyBtn.onClick.AddListener(() => StartLevel(BoardManager.LevelType.ThreeManArmy));
        hardcoreLevelBtn.onClick.AddListener(() => StartLevel(BoardManager.LevelType.Hardcore));

        for (int i = 0; i < reduceBtns.Length; i++)
        {
            int y = i;
            reduceBtns[i].onClick.AddListener(() => ReduceHero(y));
            increaseBtns[i].onClick.AddListener(() => IncreaseHero(y));
        }
        
        generateLevelBtn.onClick.AddListener(GenerateLevel);
        closeBtn.onClick.AddListener(() => ClosePopup(null));
    }

    private void StartLevel(BoardManager.LevelType level)
    {
        ClosePopup(() => MainMenuController.Instance.CloseMainMenu(() => BoardManager.Instance.StartLevel(level)));
    }

    private void ReduceHero(int i)
    {
        int.TryParse(amounts[i].text, out int currentAmount);
        if (currentAmount == 0) return;

        if (i <= 2) // modifying player units
        {
            int.TryParse(amounts[0].text, out int grunts);
            int.TryParse(amounts[1].text, out int jumpships);
            int.TryParse(amounts[2].text, out int tanks);
            
            if (grunts + jumpships + tanks > 1)
            {
                currentAmount -= 1;
                amounts[i].text = currentAmount.ToString();
            }
        }
        else if (i <= 5) // modifying enemy units
        {
            int.TryParse(amounts[3].text, out int drones);
            int.TryParse(amounts[4].text, out int dreadnoughts);
            int.TryParse(amounts[5].text, out int commandUnits);
          
            if ((commandUnits > 1 && i == 5) || i != 5)
            {
                currentAmount -= 1;
                amounts[i].text = currentAmount.ToString();
            }
        }
    }
    
    private void IncreaseHero(int i)
    {
        int.TryParse(amounts[i].text, out int currentAmount);

        if (i <= 2) // modifying player units
        {
            int.TryParse(amounts[0].text, out int grunts);
            int.TryParse(amounts[1].text, out int jumpships);
            int.TryParse(amounts[2].text, out int tanks);
            
            if (grunts + jumpships + tanks < 16)
            {
                currentAmount += 1;
                amounts[i].text = currentAmount.ToString();
            }
        }
        else if (i <= 5) // modifying enemy units
        {
            int.TryParse(amounts[3].text, out int drones);
            int.TryParse(amounts[4].text, out int dreadnoughts);
            int.TryParse(amounts[5].text, out int commandUnits);
            
            if (drones + dreadnoughts + commandUnits < 32)
            {
                currentAmount += 1;
                amounts[i].text = currentAmount.ToString();
            }
        }
    }

    private void GenerateLevel()
    {
        int[] unitsQuantity = new int[amounts.Length];

        for (int i = 0; i < amounts.Length; i++)
        {
            int.TryParse(amounts[i].text, out unitsQuantity[i]);
        }
        
        ClosePopup(() => MainMenuController.Instance.CloseMainMenu(() => BoardManager.Instance.StartLevel(BoardManager.LevelType.Custom, true, unitsQuantity)));
    }
}
