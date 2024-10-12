using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    public float speed = 70f;
    public int baseDamage;
    [HideInInspector]
    public int damage;
    public float explosionRadius = 0f;
    public GameObject impactEffect;

    private bool useCriticalHit;
    private float critChance;
    private float critMultiplier;
    private float critCooldown;
    private float additionalCritDamagePercent;
    private float lastCritTime;

    public ExtraButtonsManager extraButtonsManager;

    void Start()
    {
        InitializeDamage();
    }

    public void InitializeDamage()
    {
        extraButtonsManager = FindObjectOfType<ExtraButtonsManager>();
        if (extraButtonsManager != null)
        {
            damage = Mathf.CeilToInt(baseDamage * extraButtonsManager.GetBulletDamageMultiplier());
        }
        else
        {
            damage = baseDamage;
        }
    }

    public void Seek(Transform _target)
    {
        target = _target;
    }

    public void SetCriticalHitSettings(bool useCrit, float chance, float multiplier, float cooldown, float additionalDamagePercent, float lastTime)
    {
        useCriticalHit = useCrit;
        critChance = chance;
        critMultiplier = multiplier;
        critCooldown = cooldown;
        additionalCritDamagePercent = additionalDamagePercent;
        lastCritTime = lastTime;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    void HitTarget()
    {
        if (explosionRadius > 0f)
        {
            Explode();
        }
        else
        {
            Damage(target);
        }

        Destroy(gameObject);
    }
    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Damage(collider.transform);
            }
        }

        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);
    }

    void Damage(Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            float actualDamage = damage;

            if (useCriticalHit && Time.time - lastCritTime >= critCooldown && Random.value < critChance)
            {
                actualDamage *= critMultiplier;
                actualDamage += e.startHealth * additionalCritDamagePercent;
                lastCritTime = Time.time;
            }

            e.TakeDamage(actualDamage);
        }

        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}