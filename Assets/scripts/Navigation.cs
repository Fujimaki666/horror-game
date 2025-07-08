using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵の巡回システム
/// 設定された巡回地点間をランダムに移動する
/// </summary>
public class Navigation : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("巡回する地点の配列")]
    public Transform[] goals;

    [Header("Movement Settings")]
    [Tooltip("目的地到達の判定距離")]
    public float arrivalDistance = 0.5f;

    [Header("Debug")]
    [Tooltip("デバッグログを表示するか")]
    public bool showDebugLogs = true;

    // プライベート変数
    private int currentDestinationIndex = 0;     // 現在の目的地番号
    private NavMeshAgent agent;                  // NavMeshAgentコンポーネント

    #region Unity Lifecycle

    void Start()
    {
        InitializeNavigation();
    }

    void Update()
    {
        CheckArrival();
    }

    /// <summary>
    /// 巡回中かどうかを確認
    /// </summary>
    public bool IsPatrolling()
    {
        return enabled && agent != null && !agent.isStopped && goals != null && goals.Length > 0;
    }

    /// <summary>
    /// 巡回システムが有効かどうか
    /// </summary>
    public bool IsActive()
    {
        return enabled && agent != null;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// 巡回システムの初期化
    /// </summary>
    private void InitializeNavigation()
    {
        // NavMeshAgentコンポーネントを取得
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"{gameObject.name}: NavMeshAgent component not found!");
            return;
        }

        // 巡回地点の検証
        if (!ValidateGoals())
        {
            return;
        }

        // 最初の目的地を設定
        MoveToNextGoal();
    }

    /// <summary>
    /// 巡回地点の設定が正しいかチェック
    /// </summary>
    private bool ValidateGoals()
    {
        if (goals == null || goals.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: No patrol goals set! Please assign patrol points.");
            return false;
        }

        // null地点をチェック
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i] == null)
            {
                Debug.LogWarning($"{gameObject.name}: Patrol goal at index {i} is null!");
            }
        }

        return true;
    }

    #endregion

    #region Movement Logic

    /// <summary>
    /// 目的地への到達をチェック
    /// </summary>
    private void CheckArrival()
    {
        if (agent == null || goals.Length == 0) return;

        // 目的地に近づき、移動が完了している場合
        bool nearDestination = agent.remainingDistance < arrivalDistance;
        bool pathComplete = !agent.pathPending;
        bool hasStoppedMoving = agent.velocity.sqrMagnitude < 0.1f;

        if (nearDestination && pathComplete && hasStoppedMoving)
        {
            MoveToNextGoal();
        }
    }

    /// <summary>
    /// 次の巡回地点に移動
    /// </summary>
    private void MoveToNextGoal()
    {
        if (goals.Length == 0) return;

        // ランダムに次の地点を選択（現在地点以外）
        int nextIndex = SelectNextGoal();
        currentDestinationIndex = nextIndex;

        // 選択された地点が有効かチェック
        if (goals[currentDestinationIndex] == null)
        {
            Debug.LogWarning($"{gameObject.name}: Selected patrol goal {currentDestinationIndex} is null!");
            return;
        }

        // 目的地を設定
        Vector3 destination = goals[currentDestinationIndex].position;
        agent.SetDestination(destination);

        // デバッグログ
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Moving to patrol goal {currentDestinationIndex} " +
                     $"({goals[currentDestinationIndex].name}) at {destination}");
        }
    }

    /// <summary>
    /// 次の巡回地点をランダムに選択
    /// 現在の地点と同じ地点は避ける
    /// </summary>
    private int SelectNextGoal()
    {
        if (goals.Length <= 1) return 0;

        int nextIndex;
        do
        {
            nextIndex = Random.Range(0, goals.Length);
        }
        while (nextIndex == currentDestinationIndex);

        return nextIndex;
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// 巡回システムを一時停止
    /// 追跡モード時に呼ばれる
    /// </summary>
    public void PausePatrol()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }

        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Patrol paused");
        }
    }

    /// <summary>
    /// 巡回システムを再開
    /// 追跡終了時に呼ばれる
    /// </summary>
    public void ResumePatrol()
    {
        if (agent != null)
        {
            agent.isStopped = false;
            // 現在の目的地に向かって再開
            if (goals.Length > 0 && goals[currentDestinationIndex] != null)
            {
                agent.SetDestination(goals[currentDestinationIndex].position);
            }
        }

        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Patrol resumed");
        }
    }

    /// <summary>
    /// 現在の目的地を取得
    /// </summary>
    public Transform GetCurrentGoal()
    {
        if (goals.Length > 0 && currentDestinationIndex < goals.Length)
        {
            return goals[currentDestinationIndex];
        }
        return null;
    }

   

    #endregion

    #region Debug Visualization

    /// <summary>
    /// デバッグ用：巡回地点と経路を可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (goals == null || goals.Length == 0) return;

        // 巡回地点を描画
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i] == null) continue;

            // 現在の目的地は赤、その他は青で表示
            Gizmos.color = (i == currentDestinationIndex) ? Color.red : Color.blue;
            Gizmos.DrawWireSphere(goals[i].position, 1f);

            // 地点番号を表示（エディタでのみ）
#if UNITY_EDITOR
            UnityEditor.Handles.Label(goals[i].position + Vector3.up * 2f, $"Goal {i}");
#endif
        }

        // 現在のパスを描画
        if (Application.isPlaying && agent != null && agent.hasPath)
        {
            Gizmos.color = Color.yellow;
            Vector3[] pathCorners = agent.path.corners;
            for (int i = 0; i < pathCorners.Length - 1; i++)
            {
                Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
            }
        }
    }

    #endregion
}