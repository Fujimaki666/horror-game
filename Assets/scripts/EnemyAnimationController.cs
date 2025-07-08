using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    [Header("Animation Settings")]
    public float speedMultiplier = 1f;
    public float smoothTime = 0.1f;

    private float currentSpeed;
    private float targetSpeed;

    void Start()
    {

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            Debug.LogWarning("Animator component not found!");
        }

        if (agent == null)
        {
            Debug.LogWarning("NavMeshAgent component not found!");
        }
    }

    void Update()
    {
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        if (animator == null || agent == null) return;

        targetSpeed = agent.velocity.magnitude * speedMultiplier;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / smoothTime);

        // 一時的に：常にWalkアニメーションを使用し、速度で制御
        animator.SetFloat("Speed", currentSpeed);

        // isWalkingは使わず、Speedのみで制御
        // animator.SetBool("isWalking", currentSpeed > 0.1f); // コメントアウト

        Debug.Log($"Agent Velocity: {agent.velocity.magnitude:F2}, Current Speed: {currentSpeed:F2}");
    }

    public void SetIdleState()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", 0f);
        animator.SetBool("isWalking", false);
    }

    public void SetWalkingState()
    {
        if (animator == null) return;

        animator.SetBool("isWalking", true);
    }

    public void PauseAnimation()
    {
        if (animator == null) return;

        animator.speed = 0f;
    }

    public void ResumeAnimation()
    {
        if (animator == null) return;

        animator.speed = 1f;
    }

    // 外部から直接アニメーション制御
    public void SetAnimationSpeed(float speed)
    {
        if (animator == null) return;

        animator.SetFloat("Speed", speed);
        animator.SetBool("isWalking", speed > 0.1f);
    }
}