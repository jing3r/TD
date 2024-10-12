using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerBlueprint
{
    public GameObject prefab;
    public int cost;
    public GameObject upgradedPrefab;
    public int upgradeCost;
    public string description;
    public string upgradedDescription;
    public Sprite icon;
    public int level;

    public int GetRefund(bool isUpgraded)
    {
        if (isUpgraded)
        {
            return (cost + upgradeCost) / 2;
        }
        return cost / 2;
    }
}