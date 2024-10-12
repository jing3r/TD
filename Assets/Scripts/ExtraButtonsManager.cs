using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExtraButtonsManager : MonoBehaviour
{
    public Button pauseButton;
    public Button increaseBulletDamageButton;
    public Button increaseBeamDamageButton;
    public Button reverseDirectionButton;

    public Text pauseButtonCostText;
    public Text increaseBulletDamageCostText;
    public Text increaseBeamDamageCostText;
    public Text reverseDirectionCostText;

    public Text bulletDamagePercentageText;
    public Text beamDamagePercentageText;

    public WaveSpawner waveSpawner;
    public float pauseDuration = 10f;
    public int increaseBulletDamageBaseCost = 100;
    public int increaseBeamDamageBaseCost = 100;
    public int reverseDirectionMultiplier = 25;

    private int increaseBulletDamageClickCount = 0;
    private int increaseBeamDamageClickCount = 0;
    private DamageValues damageValues;

    void Start()
    {
        pauseButton.onClick.AddListener(PauseTimer);
        increaseBulletDamageButton.onClick.AddListener(IncreaseBulletDamage);
        increaseBeamDamageButton.onClick.AddListener(IncreaseBeamDamage);
        reverseDirectionButton.onClick.AddListener(() => StartCoroutine(ReverseDirectionForEnemies(5f)));

        damageValues.bulletDamageMultiplier = 1.0f;
        damageValues.beamDamageMultiplier = 1.0f;

        bulletDamagePercentageText.text = "100%";
        beamDamagePercentageText.text = "100%";

        UpdateButtonCosts();
    }

    void Update()
    {
        HandleHotkeys();
    }

    public void UpdateButtonCosts()
    {
        int currentWave = waveSpawner.GetWaveNumber();
        int pauseCost = currentWave * Mathf.CeilToInt(pauseDuration);
        pauseButtonCostText.text = pauseCost.ToString();

        int bulletDamageCost = increaseBulletDamageBaseCost + (increaseBulletDamageClickCount * 25);
        increaseBulletDamageCostText.text = bulletDamageCost.ToString();

        int beamDamageCost = increaseBeamDamageBaseCost + (increaseBeamDamageClickCount * 25);
        increaseBeamDamageCostText.text = beamDamageCost.ToString();

        int reverseDirectionCost = (currentWave * 5) + reverseDirectionMultiplier;
        reverseDirectionCostText.text = reverseDirectionCost.ToString();
    }

    void PauseTimer()
    {
        int currentWave = waveSpawner.GetWaveNumber();
        int cost = currentWave * Mathf.CeilToInt(pauseDuration);
        if (PlayerStats.Money >= cost)
        {
            PlayerStats.Money -= cost;
            waveSpawner.PauseTimer(pauseDuration);
            UpdateButtonCosts();
        }
    }

    void IncreaseBulletDamage()
    {
        int cost = increaseBulletDamageBaseCost + (increaseBulletDamageClickCount * 25);
        if (PlayerStats.Money >= cost)
        {
            PlayerStats.Money -= cost;
            damageValues.bulletDamageMultiplier *= 1.1f;
            increaseBulletDamageClickCount++;

            Bullet[] allBullets = FindObjectsOfType<Bullet>();
            foreach (Bullet bullet in allBullets)
            {
                bullet.damage = Mathf.CeilToInt(bullet.baseDamage * damageValues.bulletDamageMultiplier);
            }

            bulletDamagePercentageText.text = Mathf.RoundToInt(damageValues.bulletDamageMultiplier * 100).ToString() + "%";
            UpdateButtonCosts();
        }
    }

    void IncreaseBeamDamage()
    {
        int cost = increaseBeamDamageBaseCost + (increaseBeamDamageClickCount * 25);
        if (PlayerStats.Money >= cost)
        {
            PlayerStats.Money -= cost;
            damageValues.beamDamageMultiplier *= 1.1f;
            increaseBeamDamageClickCount++;

            Tower[] allTowers = FindObjectsOfType<Tower>();
            foreach (Tower tower in allTowers)
            {
                if (tower.useBeam)
                {
                    tower.damageOverTime = Mathf.CeilToInt(tower.baseDamageOverTime * damageValues.beamDamageMultiplier);
                }
            }

            beamDamagePercentageText.text = Mathf.RoundToInt(damageValues.beamDamageMultiplier * 100).ToString() + "%";
            UpdateButtonCosts();
        }
    }

    private IEnumerator ReverseDirectionForEnemies(float duration)
    {
        int currentWave = waveSpawner.GetWaveNumber();
        int cost = (currentWave * 5) + reverseDirectionMultiplier;
        if (PlayerStats.Money >= cost)
        {
            PlayerStats.Money -= cost;
            EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
            foreach (EnemyMovement enemy in enemies)
            {
                enemy.ReverseDirection(true);
            }

            yield return new WaitForSeconds(duration);

            foreach (EnemyMovement enemy in enemies)
            {
                enemy.ReverseDirection(false);
            }

            UpdateButtonCosts();
        }
    }

    public float GetBulletDamageMultiplier()
    {
        return damageValues.bulletDamageMultiplier;
    }

    public float GetBeamDamageMultiplier()
    {
        return damageValues.beamDamageMultiplier;
    }

    void HandleHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PauseTimer();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            IncreaseBulletDamage();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(ReverseDirectionForEnemies(5f));
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            IncreaseBeamDamage();
        }
    }
}
