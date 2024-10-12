using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeUI : MonoBehaviour
{
    private Node target;
    public GameObject ui;
    public TMP_Text upgradeCost;
    public Button upgradeButton;
    public TMP_Text refund;
    public Button mergeButton;
    public Button sellButton;
    public Tooltip tooltip;

    private bool isMergeMode = false;

    void Update()
    {
        HandleHotkeys();
    }

    public void SetTarget(Node _target)
    {
        target = _target;
        transform.position = target.GetBuildPosition();

        if (target.IsMaxLevel())
        {
            mergeButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            mergeButton.gameObject.SetActive(isMergeMode);
            upgradeButton.gameObject.SetActive(!isMergeMode);
        }

        if (!target.isUpgraded)
        {
            upgradeCost.text = " " + target.towerBlueprint.upgradeCost;
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeCost.text = "DONE";
            upgradeButton.interactable = false;
        }

        refund.text = " " + target.towerBlueprint.GetRefund(target.isUpgraded);
        ui.SetActive(true);

        if (BuildManager.instance.IsMergeModeActive)
        {
            ShowMergeTooltip();
        }
    }

    public void Hide()
    {
        ui.SetActive(false);
        HideMergeTooltip();
    }

    public void Upgrade()
    {
        target.UpgradeTower();
        BuildManager.instance.DeselectNode();
        HideUpgradeTooltip();
    }

    public void Merge()
    {
        target.MergeTower();
        BuildManager.instance.DeselectNode();
    }

    public void Sell()
    {
        target.isUpgraded = false;
        target.SellTower();
        BuildManager.instance.DeselectNode();
    }

    public void ShowUpgradeTooltip()
    {
        if (target != null && target.towerBlueprint != null)
        {
            string tooltipText = target.towerBlueprint.upgradedDescription.Replace("\\n", "\n");
            tooltip.ShowTooltip(tooltipText);
        }
    }

    public void HideUpgradeTooltip()
    {
        tooltip.HideTooltip();
    }

    public void ShowMergeTooltip()
    {
        if (target != null && target.towerBlueprint != null)
        {
            string tooltipText = target.towerBlueprint.description.Replace("\\n", "\n");
            tooltip.ShowTooltip(tooltipText);
        }
    }

    public void HideMergeTooltip()
    {
        tooltip.HideTooltip();
    }

    public void SetClassicMode()
    {
        isMergeMode = false;
        mergeButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(true);
    }

    public void SetMastermindMode()
    {
        isMergeMode = false;
        mergeButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(true);
    }

    public void SetMergeMode()
    {
        isMergeMode = true;
        mergeButton.gameObject.SetActive(true);
        upgradeButton.gameObject.SetActive(false);
    }

    void HandleHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (upgradeButton.gameObject.activeSelf && upgradeButton.interactable)
            {
                Upgrade();
            }
            else if (mergeButton.gameObject.activeSelf && mergeButton.interactable)
            {
                Merge();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Sell();
        }
    }
}
