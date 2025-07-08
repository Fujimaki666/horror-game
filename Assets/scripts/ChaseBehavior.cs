using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChaseBehavior : MonoBehaviour
{
    [Header("Chase Settings")]
    public float chaseSpeed = 5f;
    public float loseTargetTime = 5f;

    private NavMeshAgent agent;
    private Transform target;
    private bool isChasing = false;
    private float lastSeenTime;

    public System.Action OnChaseStarted;
    public System.Action OnChaseEnded;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component required!");
        }
    }

    void Update()
    {
        if (isChasing)
        {
            HandleChasing();
        }
    }

    void HandleChasing()
    {
        if (target == null)
        {
            StopChasing();
            return;
        }

        agent.SetDestination(target.position);
    }

    public void StartChasing(Transform newTarget)
    {
        if (newTarget == null) return;

        target = newTarget;
        isChasing = true;
        lastSeenTime = Time.time;

        agent.speed = chaseSpeed;
        agent.isStopped = false;

        OnChaseStarted?.Invoke();
        Debug.Log("Chase started!");
    }

    public void StopChasing()
    {
        isChasing = false;
        target = null;
        agent.isStopped = true;

        OnChaseEnded?.Invoke();
        Debug.Log("Stopped chasing");
    }

    public void UpdateLastSeenTime()
    {
        lastSeenTime = Time.time;
    }

    public bool ShouldLoseTarget()
    {
        return Time.time - lastSeenTime > loseTargetTime;
    }

    public bool IsChasing()
    {
        return isChasing;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public float GetTimeSinceLastSeen()
    {
        return Time.time - lastSeenTime;
    }
}