using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Color notEnoughMoneyColor;
    public Vector3 positionOffset;

    [HideInInspector]
    public GameObject tower;
    [HideInInspector]
    public TowerBlueprint towerBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;

    private Color startColor;
    private Renderer rend;
    BuildManager buildManager;
    public int maxLevel = 5;

    public bool IsMaxLevel()
    {
        return towerBlueprint.level >= maxLevel;
    }
    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        buildManager = BuildManager.instance;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (tower != null)
        {
            buildManager.SelectNode(this);
            return;
        }

        if (!buildManager.CanBuild)
            return;

        BuildTower(buildManager.GetTowerToBuild());
    }

    void BuildTower(TowerBlueprint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log("Not enough money");
            return;
        }

        PlayerStats.Money -= blueprint.cost;
        GameObject _tower = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        tower = _tower;
        towerBlueprint = blueprint;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
        buildManager.ClearTowerToBuild();
    }

    public void UpgradeTower()
    {
        if (PlayerStats.Money < towerBlueprint.upgradeCost)
        {
            Debug.Log("Not enough money to upgrade");
            return;
        }

        PlayerStats.Money -= towerBlueprint.upgradeCost;
        Destroy(tower);

        GameObject _tower = (GameObject)Instantiate(towerBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        tower = _tower;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        isUpgraded = true;
    }

    public void SellTower()
    {
        PlayerStats.Money += towerBlueprint.GetRefund(isUpgraded);

        GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Destroy(tower);
        towerBlueprint = null;
    }

    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!buildManager.CanBuild)
            return;

        if (buildManager.HasMoney)
        {
            rend.material.color = hoverColor;
        }
        else
        {
            rend.material.color = notEnoughMoneyColor;
        }
    }

    void OnMouseExit()
    {
        rend.material.color = startColor;
    }

    public void ShowTooltip()
    {
        if (towerBlueprint != null)
        {
            string tooltipText = towerBlueprint.description.Replace("\\n", "\n");
            buildManager.nodeUI.tooltip.ShowTooltip(tooltipText);
        }
    }

    public void HideTooltip()
    {
        buildManager.nodeUI.tooltip.HideTooltip();
    }

    public bool CanMerge()
    {
        return FindNearestSameTower() != null;
    }

    private Node FindNearestSameTower()
    {
        Node nearestNode = null;
        float minDistance = Mathf.Infinity;

        foreach (Node node in FindObjectsOfType<Node>())
        {
            if (node != this && node.tower != null && node.tower.name == tower.name)
            {
                float distance = Vector3.Distance(transform.position, node.transform.position);
                if (distance < minDistance)
                {
                    nearestNode = node;
                    minDistance = distance;
                }
            }
        }

        return nearestNode;
    }

    public void MergeTower()
    {
        Node nearestNode = FindNearestSameTower();
        if (nearestNode == null)
        {
            Debug.Log("No same tower to merge with");
            return;
        }

        Destroy(tower);
        Destroy(nearestNode.tower);

        TowerBlueprint nextLevelTower = GetNextLevelTower();
        if (nextLevelTower == null)
        {
            Debug.Log("No higher level tower available");
            return;
        }

        GameObject newTower = (GameObject)Instantiate(nextLevelTower.prefab, GetBuildPosition(), Quaternion.identity);
        tower = newTower;
        towerBlueprint = nextLevelTower;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        nearestNode.tower = null;
        nearestNode.towerBlueprint = null;

        isUpgraded = false;
    }

    public TowerBlueprint GetNextLevelTower()
    {
        if (buildManager == null)
        {
            Debug.LogError("BuildManager instance is missing!");
            return null;
        }

        int currentLevel = towerBlueprint.level;
        int nextLevel = currentLevel + 1;

        if (nextLevel >= buildManager.allTowersByLevel.Length)
        {
            return null;
        }

        if (buildManager.allTowersByLevel[nextLevel] == null || buildManager.allTowersByLevel[nextLevel].Length == 0)
        {
            Debug.LogError("Next level towers array is not initialized properly!");
            return null;
        }

        int randomIndex = Random.Range(0, buildManager.allTowersByLevel[nextLevel].Length);
        return buildManager.allTowersByLevel[nextLevel][randomIndex];
    }
}
