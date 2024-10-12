using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour
{
    private Transform target;
    private Enemy targetEnemy;

    [Header("General")]
    public float range = 30f;

    [Header("Projectiles")]
    public float fireRate = 1f;
    private float fireCountdown = 0.25f;
    public GameObject bulletPrefab;

    [Header("Beams")]
    public bool useBeam = false;
    public int baseDamageOverTime;
    [HideInInspector]
    public int damageOverTime;
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;

    [Header("Multishot")]
    public bool useMultishot = false;
    public float multishotRadius = 30f;
    public int maxMultishotTargets = 1;

    [Header("Slow Settings")]
    public bool useSlow = false;
    [Range(0f, 1f)]
    public float slowChance = 0.2f;
    [Range(0f, 1f)]
    public float slowAmount = 0.5f;
    public float slowDuration = 2f;

    [Header("Critical Hit Settings")]
    public bool useCriticalHit = false;
    [Range(0f, 1f)]
    public float critChance = 0.1f;
    [Range(1f, 3f)]
    public float critMultiplier = 2f;
    public float critCooldown = 1f;
    [Range(0f, 1f)]
    public float additionalCritDamagePercent = 0f;

    private float lastCritTime = 0f;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public Transform partToRotate;
    public float turnSpeed = 10f;

    public Transform firePoint;
    public ExtraButtonsManager extraButtonsManager;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 1f);

        extraButtonsManager = FindObjectOfType<ExtraButtonsManager>();
        if (extraButtonsManager != null && useBeam)
        {
            InitializeDamageOverTime();
        }
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
        }
        else
        {
            target = null;
            targetEnemy = null;
        }
    }

    void Update()
    {
        if (target == null || targetEnemy == null || targetEnemy.health <= 0)
        {
            if (useBeam)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                }
            }
            UpdateTarget();
            return;
        }

        LockOnTarget();

        if (useBeam)
        {
            Beam();
        }
        else if (useMultishot)
        {
            if (fireCountdown <= 0f)
            {
                Multishot();
                fireCountdown = 1f / fireRate;
            }
        }
        else
        {
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
        }

        fireCountdown -= Time.deltaTime;
    }

    void LockOnTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Beam()
    {
        float damage = damageOverTime * Time.deltaTime;
        float critDamage = 0f;

        if (useCriticalHit && Time.time - lastCritTime >= critCooldown && Random.value < critChance)
        {
            critDamage = (damage * critMultiplier) + (targetEnemy.startHealth * additionalCritDamagePercent);
            lastCritTime = Time.time;
        }

        targetEnemy.TakeDamage(damage + critDamage);

        if (useSlow && Random.value < slowChance)
        {
            targetEnemy.ApplySlow(slowAmount, slowDuration);
        }

        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;
        impactEffect.transform.position = target.position + dir.normalized * 1.5f;
        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    }

    void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);
            bullet.SetCriticalHitSettings(useCriticalHit, critChance, critMultiplier, critCooldown, additionalCritDamagePercent, lastCritTime);
            if (useSlow && Random.value < slowChance)
            {
                targetEnemy.ApplySlow(slowAmount, slowDuration);
            }
        }
    }

    void Multishot()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, multishotRadius);
        int targetsHit = 0;
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Bullet bullet = bulletGO.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.Seek(collider.transform);
                    bullet.SetCriticalHitSettings(useCriticalHit, critChance, critMultiplier, critCooldown, additionalCritDamagePercent, lastCritTime);
                    Enemy enemy = collider.GetComponent<Enemy>();
                    if (useSlow && enemy != null && Random.value < slowChance)
                    {
                        enemy.ApplySlow(slowAmount, slowDuration);
                    }
                }
                targetsHit++;
                if (targetsHit >= maxMultishotTargets)
                    break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
        if (useMultishot)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, multishotRadius);
        }
    }

    public void InitializeDamageOverTime()
    {
        if (extraButtonsManager != null)
        {
            damageOverTime = Mathf.CeilToInt(baseDamageOverTime * extraButtonsManager.GetBeamDamageMultiplier());
        }
        else
        {
            damageOverTime = baseDamageOverTime;
        }
    }
}
