using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public static int EnemiesAlive = 0;
    public Wave[] waves;
    public GameObject[] bossPrefabs;
    public Transform spawnPoint;
    public float[] bossSpawnIntervals;
    public float timeBetweenWaves = 20f;
    public float timeIncreasePerWave = 1f;
    public float healthMultiplier = 1.5f;
    public float intervalDecreaseFactor = 0.9f;
    public float minSpawnInterval = 1f;
    private float countdown;
    public Text waveCountdownText;
    private int waveIndex = 0;
    private bool endlessMode = false;
    private int currentBossIndex = 0;

    public WaveCountUI waveCountUI;

    private int[] bossSpawnCounts;

    private bool isPaused = false;
    private float pauseTimer = 0f;
    private float originalCountdown;
    private ExtraButtonsManager extraButtonsManager;
    void Start()
    {
        bossSpawnCounts = new int[bossPrefabs.Length];
        countdown = timeBetweenWaves;
         extraButtonsManager = FindObjectOfType<ExtraButtonsManager>();
    }
    void Update()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            waveCountdownText.text = "Paused: " + Mathf.CeilToInt(pauseTimer) + "s";

            if (pauseTimer <= 0f)
            {
                isPaused = false;
                waveCountdownText.color = Color.white;
                countdown = originalCountdown;
            }
        }
        else
        {
            if (countdown <= 0f)
            {
                if (endlessMode)
                {
                    waveCountdownText.gameObject.SetActive(false);
                    StartCoroutine(SpawnEndlessWave());
                }
                else
                {
                    StartCoroutine(SpawnWave());
                }
                countdown = timeBetweenWaves;
            }

            countdown -= Time.deltaTime;
            waveCountdownText.text = string.Format("{0:00.0}", countdown);
        }
    }

    public void PauseTimer(float duration)
    {
        isPaused = true;
        pauseTimer = duration;
        originalCountdown = countdown;
        waveCountdownText.color = Color.gray;
    }

    public int GetWaveNumber()
    {
        return waveIndex + 1;
    }
   public IEnumerator SpawnWave()
{
    PlayerStats.Rounds++;
    Wave wave = waves[waveIndex];
    for (int i = 0; i < wave.count; i++)
    {
        SpawnEnemy(wave.enemy);
        yield return new WaitForSeconds(1f / wave.rate);
    }

    waveIndex++;
    if (waveIndex == waves.Length)
    {
        endlessMode = true;
        waveCountUI.SetEndlessMode(true);
    }
    else
    {
        timeBetweenWaves += timeIncreasePerWave;
    }

    if (extraButtonsManager != null)
    {
        extraButtonsManager.UpdateButtonCosts();
    }
}

    IEnumerator SpawnEndlessWave()
    {

    yield return StartCoroutine(SpawnBosses());


    while (true)
    {
        yield return StartCoroutine(SpawnBosses());
        yield return new WaitForSeconds(60f);
    }
    }

    IEnumerator SpawnBosses()
    {
        while (currentBossIndex < bossPrefabs.Length)
        {
            float interval = bossSpawnIntervals[currentBossIndex];
            StartCoroutine(SpawnBossType(bossPrefabs[currentBossIndex], interval, currentBossIndex));
            currentBossIndex++;
            yield return new WaitForSeconds(60f);
        }
    }

    IEnumerator SpawnBossType(GameObject bossPrefab, float interval, int bossIndex)
    {
        while (true)
        {
            SpawnEnemy(bossPrefab, bossIndex);
            yield return new WaitForSeconds(interval);
            interval = Mathf.Max(interval * intervalDecreaseFactor, minSpawnInterval);
        }
    }

    void SpawnEnemy(GameObject enemyPrefab, int bossIndex = -1)
    {
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        EnemiesAlive++;

        Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            if (endlessMode && bossIndex >= 0)
            {
                bossSpawnCounts[bossIndex]++;
                float newHealth = enemyScript.startHealth * Mathf.Pow(healthMultiplier, bossSpawnCounts[bossIndex]);
                enemyScript.startHealth = newHealth;
                enemyScript.health = newHealth;
                Debug.Log("New start health for " + enemyPrefab.name + ": " + newHealth);
            }
            else
            {
                enemyScript.health = enemyScript.startHealth;
            }
        }
    }
}
