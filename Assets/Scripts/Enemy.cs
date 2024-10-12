using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float startSpeed = 10f;
    [HideInInspector]
    public float speed;
    public float startHealth = 100;
    [HideInInspector]
    public float health;

    public int worth = 50;
    public Image healthBar;
    public GameObject healthBarCanvas;
    private bool isDead = false;

    private Coroutine slowCoroutine;
    private EnemyAC animationController;
    private EnemyMovement enemyMovement;

    void Start()
    {
        speed = startSpeed;
        health = startHealth;
        animationController = GetComponent<EnemyAC>();
        enemyMovement = GetComponent<EnemyMovement>();
        if (animationController == null)
        {
            Debug.LogError("EnemyAC component not found on " + gameObject.name);
        }
        if (enemyMovement == null)
        {
            Debug.LogError("EnemyMovement component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        healthBarCanvas.transform.rotation = Quaternion.Euler(30, 0, 0);
    }


    public void TakeDamage(float amount)
    {
        health -= amount;
        // Debug.Log(gameObject.name + " took damage: " + amount + ", health: " + health);

        healthBar.fillAmount = health / startHealth;
        if (health <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }


void Die()
{
    //Debug.Log(gameObject.name + " is dying.");
    PlayerStats.Money += worth;
    PlayerStats.EnemiesKilled++;
    WaveSpawner.EnemiesAlive--;
    animationController.Die();

    if (enemyMovement != null)
    {
        enemyMovement.enabled = false;
    }
    gameObject.tag = "Untagged";
}


    public void Slow(float percentage)
    {
        speed = startSpeed * (1f - percentage);
    }

    public void ApplySlow(float percentage, float duration)
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = StartCoroutine(SlowCoroutine(percentage, duration));
    }

    private IEnumerator SlowCoroutine(float percentage, float duration)
    {
        Slow(percentage);
        yield return new WaitForSeconds(duration);
        ResetSpeed();
    }

    private void ResetSpeed()
    {
        speed = startSpeed;
        // Debug.Log("Speed reset to startSpeed");
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}