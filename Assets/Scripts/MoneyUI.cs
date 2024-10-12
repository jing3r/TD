using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class MoneyUI : MonoBehaviour
{
    public Text moneyText;
    void Update()
    {
        moneyText.text = "Money: " + "\n"+ PlayerStats.Money.ToString();
    }
}
