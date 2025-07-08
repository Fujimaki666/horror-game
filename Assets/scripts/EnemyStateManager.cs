using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵の状態管理システム
/// 各コンポーネント間の連携を行い、全体の動作を制御
/// 巡回 → 追跡 → 一時停止 → 巡回 の状態遷移を管理
/// </summary>
public class EnemyStateManager : MonoBehaviour
{
    /// <summary>
    /// 敵の状態を定義する列挙型
    /// </summary>
    public enum EnemyState
    {
        Patrolling,     // 巡回中
        Chasing,        // 追跡中
        Paused          // 一時停止中
    }

    [Header("Behavior Settings")]
    public float pauseTimeAfterChase = 2f;  // 追跡終了後の一時停止時間

    private EnemyState currentState = EnemyState.Patrolling;   // 現在の状態

    // 各機能コンポーネントの参照
    private PlayerDetection playerDetection;       // プレイヤー検知システム
    private PatrolBehavior patrolBehavior;         // 巡回システム
    private ChaseBehavior chaseBehavior;           // 追跡システム
    private EnemyAnimationController animationController;  // アニメーション制御

    /// <summary>
    /// 初期化：各コンポーネントを取得してイベントを登録
    /// </summary>
    void Start()
    {
        Debug.Log("=== EnemyStateManager Start() ===");

        // まず同じオブジェクトで検索
        //playerDetection = GetComponent<PlayerDetection>();

        // 見つからない場合は子オブジェクトでも検索
        
            Debug.Log("PlayerDetection not found on this object, searching children...");
            playerDetection = GetComponentInChildren<PlayerDetection>();
        

        // 他のコンポーネントを取得
        patrolBehavior = GetComponent<PatrolBehavior>();
        chaseBehavior = GetComponent<ChaseBehavior>();
        animationController = GetComponent<EnemyAnimationController>();

        Debug.Log($"PlayerDetection found: {playerDetection != null}");
        if (playerDetection != null)
        {
            Debug.Log($"PlayerDetection found on: {playerDetection.gameObject.name}");
        }

        // イベント登録
        if (playerDetection != null)
        {
            Debug.Log("Registering events");
            playerDetection.OnPlayerDetected += StartChasing;
            playerDetection.OnPlayerLost += OnPlayerLost;
        }
        else
        {
            Debug.LogError("PlayerDetection component not found anywhere!");
        }

        if (chaseBehavior != null)
        {
            chaseBehavior.OnChaseEnded += OnChaseEnded;
        }

        SetState(EnemyState.Patrolling);
    }

    /// <summary>
    /// 毎フレーム呼ばれる：追跡中の特別な処理
    /// </summary>
    void Update()
    {
        if (currentState == EnemyState.Chasing)
        {
            HandleChasingUpdate();
        }
    }
    /// <summary>
    /// 追跡中の更新処理
    /// プレイヤーの可視状態を監視し、見失い時間を管理
    /// </summary>
    void HandleChasingUpdate()
    {
        if (chaseBehavior == null || playerDetection == null) return;

        if (playerDetection.CanSeePlayer())
        {
            // プレイヤーが見えている間は最後に見た時間を更新
            chaseBehavior.UpdateLastSeenTime();
        }
        else if (chaseBehavior.ShouldLoseTarget())
        {
            // 一定時間見失ったら追跡を終了
            chaseBehavior.StopChasing();
        }
    }

    /// <summary>
    /// 追跡を開始
    /// プレイヤー検知システムから呼び出される
    /// </summary>
    void StartChasing()
    {
        if (currentState == EnemyState.Chasing) return;

        Transform player = playerDetection.GetPlayer();
        if (player != null)
        {
            SetState(EnemyState.Chasing);
            chaseBehavior?.StartChasing(player);
        }
    }

    /// <summary>
    /// プレイヤーを見失った時の処理
    /// 実際の見失い判定は HandleChasingUpdate で行う
    /// </summary>
    void OnPlayerLost()
    {
        Debug.Log("Player lost from sight");
        // 見失い処理は HandleChasingUpdate で時間管理される
    }

    /// <summary>
    /// 追跡終了時の処理
    /// 一定時間待機してから巡回に戻る
    /// </summary>
    void OnChaseEnded()
    {
        StartCoroutine(ReturnToPatrolAfterDelay());
    }

    /// <summary>
    /// 一定時間待機してから巡回に戻るコルーチン
    /// </summary>
    /// <returns>コルーチン</returns>
    IEnumerator ReturnToPatrolAfterDelay()
    {
        SetState(EnemyState.Paused);    // まず一時停止状態に

        // 設定時間だけ待機
        yield return new WaitForSeconds(pauseTimeAfterChase);

        SetState(EnemyState.Patrolling); // 巡回状態に戻る
    }

    /// <summary>
    /// 状態を変更
    /// 前の状態の終了処理と新しい状態の開始処理を行う
    /// </summary>
    /// <param name="newState">新しい状態</param>
    void SetState(EnemyState newState)
    {
        Debug.Log($"=== SetState called: {currentState} -> {newState} ==="); // 追加

        if (currentState == newState)
        {
            Debug.Log("Same state, returning"); // 追加
            return;   // 同じ状態の場合は何もしない
        }

        // 前の状態を終了
        Debug.Log($"Exiting state: {currentState}"); // 追加
        ExitState(currentState);

        // 新しい状態に変更
        currentState = newState;

        // 新しい状態を開始
        Debug.Log($"Entering state: {currentState}"); // 追加
        EnterState(currentState);

        Debug.Log($"State changed to: {currentState}");
    }

    /// <summary>
    /// 新しい状態に入る時の処理
    /// </summary>
    /// <param name="state">入る状態</param>
    void EnterState(EnemyState state)
    {
        Debug.Log($"=== EnterState called with: {state} ==="); // 追加

        switch (state)
        {
            case EnemyState.Patrolling:
                Debug.Log("Starting patrol"); // 追加
                patrolBehavior?.StartPatrol();
                break;

            case EnemyState.Chasing:
                Debug.Log("Stopping patrol for chase"); // 追加
                patrolBehavior?.StopPatrol();
                break;

            case EnemyState.Paused:
                Debug.Log("Entering paused state"); // 追加
                patrolBehavior?.StopPatrol();
                animationController?.SetIdleState();
                break;
        }

        Debug.Log($"EnterState completed for: {state}"); // 追加
    }

    /// <summary>
    /// 状態から出る時の処理
    /// </summary>
    /// <param name="state">出る状態</param>
    void ExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrolling:
                // 巡回停止
                patrolBehavior?.StopPatrol();
                break;

            case EnemyState.Chasing:
                // 追跡終了処理は ChaseBehavior で行われる
                break;

            case EnemyState.Paused:
                // アニメーションを再開
                animationController?.ResumeAnimation();
                break;
        }
    }

    /// <summary>
    /// 外部から現在の状態を確認できるプロパティ群
    /// 他のスクリプトから敵の状態を確認する際に使用
    /// </summary>
    public EnemyState CurrentState => currentState;     // 現在の状態
    public bool IsChasing => currentState == EnemyState.Chasing;        // 追跡中か
    public bool IsPatrolling => currentState == EnemyState.Patrolling;  // 巡回中か
    public bool IsPaused => currentState == EnemyState.Paused;          // 一時停止中か

    /// <summary>
    /// オブジェクト破棄時の処理
    /// イベントの登録を解除してメモリリークを防ぐ
    /// </summary>
    void OnDestroy()
    {
        // プレイヤー検知システムのイベント登録解除
        if (playerDetection != null)
        {
            playerDetection.OnPlayerDetected -= StartChasing;
            playerDetection.OnPlayerLost -= OnPlayerLost;
        }

        // 追跡システムのイベント登録解除
        if (chaseBehavior != null)
        {
            chaseBehavior.OnChaseEnded -= OnChaseEnded;
        }
    }
}