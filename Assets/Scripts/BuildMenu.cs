using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenu : MonoBehaviour
{
    public TowerBlueprint[] ctTowers;
    public TowerBlueprint[] gtTowers;
    public TowerBlueprint[] ktTowers;
    public TowerBlueprint[] rtTowers;
    public TowerBlueprint[] stTowers;
    public TowerBlueprint[] wtTowers;

    private TowerBlueprint[] currentFactionTowers;
    public TowerBlueprint[][] allTowersByLevel;

    public Tooltip tooltip;

    public Button[] towerButtons;
    public TMP_Text[] buttonCostLabels;
    public Image[] buttonIcons;

    BuildManager buildManager;
    private bool isMergeMode = false;

    void Update()
    {
        HandleHotkeys();
    }

    void Start()
    {
        buildManager = BuildManager.instance;
        if (buildManager == null)
        {
            Debug.LogError("BuildManager instance is missing!");
            return;
        }

        if (ctTowers == null || gtTowers == null || ktTowers == null || rtTowers == null || stTowers == null || wtTowers == null)
        {
            Debug.LogError("One or more tower arrays are missing!");
            return;
        }

        if (ctTowers.Length != 6 || gtTowers.Length != 6 || ktTowers.Length != 6 || rtTowers.Length != 6 || stTowers.Length != 6 || wtTowers.Length != 6)
        {
            Debug.LogError("One or more tower arrays do not have the correct length!");
            return;
        }

        allTowersByLevel = new TowerBlueprint[6][];
        for (int i = 0; i < 6; i++)
        {
            if (ctTowers[i] == null || gtTowers[i] == null || ktTowers[i] == null || rtTowers[i] == null || stTowers[i] == null || wtTowers[i] == null)
            {
                Debug.LogError($"One or more tower arrays contain null elements at index {i}!");
                return;
            }

            allTowersByLevel[i] = new TowerBlueprint[]
            {
                ctTowers[i], gtTowers[i], ktTowers[i], rtTowers[i], stTowers[i], wtTowers[i]
            };
        }

        Debug.Log("All towers by level initialized successfully.");

        InitializeMergeModeButtons();
    }

    private void InitializeMergeModeButtons()
    {
        for (int i = 0; i < towerButtons.Length; i++)
        {
            if (i == 0 || i == 2)
            {
                towerButtons[i].gameObject.SetActive(true);
                buttonCostLabels[i].text = (i == 0) ? "25" : "125";
                int level = (i == 0) ? 0 : 2;
                int randomIndex = Random.Range(0, allTowersByLevel[level].Length);
                buttonIcons[i].sprite = allTowersByLevel[level][randomIndex].icon;
            }
            else
            {
                towerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectTower(int towerIndex)
    {
        if (isMergeMode)
        {
            if (allTowersByLevel == null || allTowersByLevel.Length == 0 || allTowersByLevel[towerIndex] == null || allTowersByLevel[towerIndex].Length == 0)
            {
                Debug.LogError("AllTowersByLevel array is not initialized properly!");
                return;
            }

            int randomIndex = Random.Range(0, allTowersByLevel[towerIndex].Length);
            buildManager.SelectTowerToBuild(allTowersByLevel[towerIndex][randomIndex]);
            return;
        }

        if (currentFactionTowers == null || towerIndex < 0 || towerIndex >= currentFactionTowers.Length)
            return;

        buildManager.SelectTowerToBuild(currentFactionTowers[towerIndex]);
    }

    public void SetFactionTowers(string faction)
    {
        isMergeMode = false;
        BuildManager.instance.IsMergeModeActive = false;
        switch (faction)
        {
            case "CT":
                currentFactionTowers = ctTowers;
                break;
            case "GT":
                currentFactionTowers = gtTowers;
                break;
            case "KT":
                currentFactionTowers = ktTowers;
                break;
            case "RT":
                currentFactionTowers = rtTowers;
                break;
            case "ST":
                currentFactionTowers = stTowers;
                break;
            case "WT":
                currentFactionTowers = wtTowers;
                break;
            default:
                currentFactionTowers = null;
                break;
        }

        UpdateTowerButtons();
    }

    public void SetMastermindTowers()
    {
        isMergeMode = false;
        BuildManager.instance.IsMergeModeActive = false;

        currentFactionTowers = new TowerBlueprint[6];

        for (int i = 0; i < 6; i++)
        {
            int randomIndex = Random.Range(0, allTowersByLevel[i].Length);
            currentFactionTowers[i] = allTowersByLevel[i][randomIndex];
        }

        UpdateTowerButtons();
    }

    public void SetMergeMode()
    {
        isMergeMode = true; 
        BuildManager.instance.IsMergeModeActive = true;


        if (allTowersByLevel == null || allTowersByLevel.Length == 0 || allTowersByLevel[0] == null || allTowersByLevel[0].Length == 0)
        {
            Debug.LogError("AllTowersByLevel array is not initialized properly!");
            return;
        }


        buildManager.allTowersByLevel = allTowersByLevel;


        InitializeMergeModeButtons();
    }

    void UpdateTowerButtons()
    {
        if (currentFactionTowers == null)
            return;

        for (int i = 0; i < towerButtons.Length; i++)
        {
            if (i < currentFactionTowers.Length)
            {
                towerButtons[i].gameObject.SetActive(true);
                buttonCostLabels[i].text = currentFactionTowers[i].cost.ToString();
                buttonIcons[i].sprite = currentFactionTowers[i].icon;
            }
            else
            {
                towerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowTooltip(int towerIndex)
    {
        if (BuildManager.instance.IsMergeModeActive)
            return;

        if (currentFactionTowers == null || towerIndex < 0 || towerIndex >= currentFactionTowers.Length)
            return;

        string tooltipText = currentFactionTowers[towerIndex].description.Replace("\\n", "\n");
        tooltip.ShowTooltip(tooltipText);
    }

    public void HideTooltip()
    {
        tooltip.HideTooltip();
    }

    void HandleHotkeys()
    {
        if (!isMergeMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectTower(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectTower(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectTower(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectTower(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SelectTower(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SelectTower(5);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectTower(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectTower(2);
            }
        }
    }
}
