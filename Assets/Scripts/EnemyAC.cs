using UnityEngine;
using System.Collections;

public class EnemyAC : MonoBehaviour
{
    private Animator animator;
    private Enemy enemy;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Enemy component not found on " + gameObject.name);
        }
    }

    public void StartWalking()
    {
        if (animator.HasParameter("isWalking")) animator.SetBool("isWalking", true);
    }

    public void Die()
    {
        if (animator.HasParameter("isWalking")) animator.SetBool("isWalking", false);
        
        if (animator.HasParameter("isDead")) animator.SetTrigger("isDead");

        if (enemy != null)
        {
            StartCoroutine(DestroyAfterAnimation());
        }
        else
        {
            Debug.LogError("Enemy component not found on " + gameObject.name);
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationTime = stateInfo.length;

        yield return new WaitForSeconds(animationTime);
        enemy.DestroyEnemy();
    }
}

public static class AnimatorExtensions
{
    public static bool HasParameter(this Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}