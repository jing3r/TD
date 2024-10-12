using UnityEngine;
using UnityEngine.UI;

public class IncomeManager : MonoBehaviour
{
    public float incomeInterval = 10f;
    private float countdown;
    public int passiveIncome = 10;
    public int upgradeCost = 10;
    public Text currentIncomeText;
    public Button upgradeIncomeButton;
    public Text upgradeCostText;

    void Start()
    {
        countdown = incomeInterval;
        UpdateUI();
        upgradeIncomeButton.onClick.AddListener(UpgradeIncome);
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            GrantPassiveIncome();
            countdown = incomeInterval;
        }
        UpdateUI();
        HandleHotkeys();
    }

    void GrantPassiveIncome()
    {
        PlayerStats.Money += passiveIncome;
        Debug.Log("Income: " + passiveIncome);
    }

    void UpgradeIncome()
    {
        if (PlayerStats.Money >= upgradeCost)
        {
            PlayerStats.Money -= upgradeCost;
            passiveIncome += 1;
            Debug.Log("Income increased to " + passiveIncome);
        }
        else
        {
            Debug.Log("Not enough money to upgrade passive income");
        }
        
        UpdateUI();
    }

    void UpdateUI()
    {
        upgradeIncomeButton.GetComponentInChildren<Text>().text = "Increase income";
        currentIncomeText.text = "Income: " + passiveIncome + " in " + Mathf.Ceil(countdown).ToString() + "s";
        upgradeCostText.text = "" + upgradeCost;
    }

    void HandleHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpgradeIncome();
        }
    }
}
