using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Transform target;
    private int wavepointIndex = 0;
    private Enemy enemy;
    private bool isRotating = false;
    private bool isReversed = false;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        target = Waypoints.waypoints[0];
    }

    void Update()
    {
        if (isRotating)
            return;

        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * enemy.speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 0.5f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        if (isReversed)
        {
            if (wavepointIndex <= 0)
            {
                enemy.speed = 0;
                return;
            }

            wavepointIndex--;
        }
        else
        {
            if (wavepointIndex >= Waypoints.waypoints.Length - 1)
            {
                EndPath();
                return;
            }

            wavepointIndex++;
        }

        target = Waypoints.waypoints[wavepointIndex];
        

        if (wavepointIndex == 1)
        {
            isRotating = false;
        }
        else
        {
            StartCoroutine(RotateToNextWaypoint());
        }
    }

    IEnumerator RotateToNextWaypoint()
    {
        isRotating = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, transform.eulerAngles.y + (isReversed ? 90 : -90), 0);
        float rotateDuration = 0.5f;
        float rotateElapsed = 0f;

        while (rotateElapsed < rotateDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotateElapsed / rotateDuration);
            rotateElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
    }

    public void ReverseDirection(bool reverse)
    {
        if (reverse != isReversed)
        {
            if (isRotating)
            {
                StopCoroutine(RotateToNextWaypoint());
                isRotating = false;
            }

            isReversed = reverse;
            wavepointIndex = isReversed ? wavepointIndex - 1 : wavepointIndex + 1;

            if (wavepointIndex < 0) wavepointIndex = 0;
            if (wavepointIndex >= Waypoints.waypoints.Length) wavepointIndex = Waypoints.waypoints.Length - 1;

            target = Waypoints.waypoints[wavepointIndex];
            enemy.speed = enemy.startSpeed;

            StartCoroutine(RotateOnReverse(reverse));
        }
    }

    IEnumerator RotateOnReverse(bool reverse)
    {
        isRotating = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);
        float rotateDuration = 0.5f;
        float rotateElapsed = 0f;

        while (rotateElapsed < rotateDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotateElapsed / rotateDuration);
            rotateElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;

    }

    void EndPath()
    {
        PlayerStats.Lives--;
        WaveSpawner.EnemiesAlive--;
        Destroy(gameObject);
    }
}