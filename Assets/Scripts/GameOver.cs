using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public TMP_Text roundsText;
    public TMP_Text enemiesKilledText;

    void OnEnable()
    {
        roundsText.text = PlayerStats.Rounds.ToString();
        enemiesKilledText.text = PlayerStats.EnemiesKilled.ToString();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        FindObjectOfType<PlayerStats>().ResetStats();
    }
}