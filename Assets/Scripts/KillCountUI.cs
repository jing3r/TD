using UnityEngine;
using UnityEngine.UI;

public class KillCountUI : MonoBehaviour
{
    public Text killCountText;

    void Update()
    {
        killCountText.text = "Kills: " + PlayerStats.EnemiesKilled.ToString();
    }
}