using UnityEngine;
using UnityEngine.UI;

public class WaveCountUI : MonoBehaviour
{
    public Text waveCountText;
    public bool endlessMode = false;

    void Update()
    {
        if (endlessMode)
        {
            waveCountText.text = "Wave: Final";
        }
        else
        {
            waveCountText.text = "Wave: " + PlayerStats.Rounds.ToString();
        }
    }

    public void SetEndlessMode(bool isEndless)
    {
        endlessMode = isEndless;
    }
}
