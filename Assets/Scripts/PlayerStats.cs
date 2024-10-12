using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int Money;
    public static int Lives;
    public static int Rounds;
    public static int EnemiesKilled;

    [SerializeField]
    private int startMoney = 100;
    [SerializeField]
    private int startLives = 20;

    void Start()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        Money = startMoney;
        Lives = startLives;
        Rounds = 0;
        EnemiesKilled = 0;
    }
}