using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 巡回システム
/// 設定された巡回地点間を移動する機能を提供
/// ランダム巡回または順番巡回を選択可能
/// </summary>
public class PatrolBehavior : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;        // 巡回地点の配列
    public float patrolSpeed = 2f;          // 巡回時の移動速度
    public bool randomPatrol = true;        // trueならランダム、falseなら順番巡回

    private NavMeshAgent agent;             // NavMeshAgentコンポーネント
    private int currentPatrolIndex = 0;     // 現在の巡回地点のインデックス
    private bool isPatrolling = false;      // 巡回中かどうかのフラグ

    /// <summary>
    /// 初期化：必要なコンポーネントを取得
    /// </summary>
    void Awake() // Start()をAwake()に変更
    {
        Debug.Log("PatrolBehavior Awake() called");

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component required!");
        }
        else
        {
            Debug.Log("NavMeshAgent successfully found in Awake()");
        }
    }

    /// <summary>
    /// 毎フレーム呼ばれる：巡回中の処理を実行
    /// </summary>
    void Update()
    {
        if (isPatrolling)
        {
            HandlePatrolling();
        }
    }

    /// <summary>
    /// 巡回中の処理：目的地に到着したら次の地点を設定
    /// </summary>
    void HandlePatrolling()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        // 目的地に近づき、かつ移動が完了している場合
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            // エージェントが実際に停止している場合（速度がほぼゼロ）
            if (agent.velocity.sqrMagnitude < 0.1f)
            {
                SetNextPatrolPoint(); // 次の巡回地点を設定
            }
        }
    }

    /// <summary>
    /// 巡回を開始する
    /// 外部スクリプトから呼び出される
    /// </summary>
    public void StartPatrol()
    {
        Debug.Log("StartPatrol() called"); // 追加

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points set!");
            return;
        }

        Debug.Log($"Starting patrol with {patrolPoints.Length} points"); // 追加
        isPatrolling = true;
        agent.speed = patrolSpeed;
        agent.isStopped = false;

        Debug.Log("Calling SetNextPatrolPoint()"); // 追加
        SetNextPatrolPoint();

        Debug.Log("StartPatrol() completed"); // 追加
    }

    /// <summary>
    /// 巡回を停止する
    /// 外部スクリプトから呼び出される
    /// </summary>
    public void StopPatrol()
    {
        isPatrolling = false;
        agent.isStopped = true;         // エージェントの移動を停止
        Debug.Log("Stopped patrolling");
    }

    /// <summary>
    /// 次の巡回地点を設定する
    /// randomPatrolの設定に応じてランダムまたは順番に選択
    /// </summary>
    void SetNextPatrolPoint()
    {
        Debug.Log("SetNextPatrolPoint() called"); // 追加

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points in SetNextPatrolPoint!");
            return;
        }

        if (randomPatrol)
        {
            int nextIndex;
            do
            {
                nextIndex = Random.Range(0, patrolPoints.Length);
            } while (nextIndex == currentPatrolIndex && patrolPoints.Length > 1);

            currentPatrolIndex = nextIndex;
        }
        else
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        Debug.Log($"Selected patrol index: {currentPatrolIndex}"); // 追加

        if (patrolPoints[currentPatrolIndex] != null)
        {
            Vector3 destination = patrolPoints[currentPatrolIndex].position;
            Debug.Log($"Setting destination to: {destination}"); // 追加

            agent.SetDestination(destination);

            Debug.Log($"Destination set. HasPath: {agent.hasPath}, PathPending: {agent.pathPending}"); // 追加
        }
        else
        {
            Debug.LogError($"Patrol point {currentPatrolIndex} is null!");
        }
    }

    /// <summary>
    /// 巡回中かどうかを確認
    /// </summary>
    /// <returns>巡回中の場合true</returns>
    public bool IsPatrolling()
    {
        return isPatrolling;
    }

    /// <summary>
    /// 現在の目標地点を取得
    /// </summary>
    /// <returns>現在の目標Transform</returns>
    public Transform GetCurrentTarget()
    {
        if (patrolPoints != null && currentPatrolIndex < patrolPoints.Length)
        {
            return patrolPoints[currentPatrolIndex];
        }
        return null;
    }
}