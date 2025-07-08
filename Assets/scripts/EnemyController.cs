using UnityEngine;
using System;

/// <summary>
/// 敵の軽量状態管理システム
/// 既存のNavigation、ChaseTarget、FunSearchと連携して全体の状態を管理
/// 各コンポーネントの独立性を保ちながら、統一された状態確認を提供
/// </summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// 敵の行動状態
    /// </summary>
    public enum EnemyState
    {
        Patrolling,    // 巡回中
        Chasing,       // 追跡中
        Searching,     // 探索中（プレイヤーを見失った直後）
        Idle           // 待機中
    }

    [Header("State Management")]
    [Tooltip("現在の敵の状態（読み取り専用）")]
    [SerializeField] private EnemyState currentState = EnemyState.Patrolling;

    [Header("State Transition Settings")]
    [Tooltip("追跡終了後の探索時間（秒）")]
    public float searchDuration = 3f;

    [Header("Debug")]
    [Tooltip("状態変化のログを表示するか")]
    public bool showStateChangeLogs = true;

    [Tooltip("現在の状態をUIに表示するか")]
    public bool showStatusUI = true;

    // コンポーネント参照
    private Navigation navigationSystem;
    private ChaseTarget chaseSystem;
    private FunSearch detectionSystem;

    // 状態管理
    private EnemyState previousState;
    private float stateChangeTime;
    private float searchTimer;

    // イベント（他のシステムが購読可能）
    public event Action<EnemyState, EnemyState> OnStateChanged; // (新状態, 旧状態)
    public event Action OnStartedPatrolling;
    public event Action OnStartedChasing;
    public event Action OnStartedSearching;
    public event Action OnBecameIdle;

    #region Unity Lifecycle

    void Start()
    {
        InitializeController();
        StartPatrolling();
    }

    void Update()
    {
        UpdateStateLogic();
        UpdateUI();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// コントローラーの初期化
    /// </summary>
    private void InitializeController()
    {
        // 各システムコンポーネントを取得
        navigationSystem = GetComponent<Navigation>();
        chaseSystem = GetComponent<ChaseTarget>();
        detectionSystem = GetComponentInChildren<FunSearch>();

        // コンポーネントの存在確認
        ValidateComponents();

        // 初期状態を設定
        previousState = currentState;
        stateChangeTime = Time.time;

        if (showStateChangeLogs)
        {
            Debug.Log($"{gameObject.name}: EnemyController initialized. Initial state: {currentState}");
        }
    }

    /// <summary>
    /// 必要なコンポーネントが存在するかチェック
    /// </summary>
    private void ValidateComponents()
    {
        if (navigationSystem == null)
        {
            Debug.LogWarning($"{gameObject.name}: Navigation component not found. Patrol functionality will be limited.");
        }

        if (chaseSystem == null)
        {
            Debug.LogWarning($"{gameObject.name}: ChaseTarget component not found. Chase functionality will be limited.");
        }

        if (detectionSystem == null)
        {
            Debug.LogWarning($"{gameObject.name}: FunSearch component not found. Player detection will be limited.");
        }
    }

    #endregion

    #region State Logic

    /// <summary>
    /// 状態ロジックの更新
    /// </summary>
    private void UpdateStateLogic()
    {
        // 各システムの状態を確認して、適切な状態を決定
        EnemyState newState = DetermineCurrentState();

        // 状態が変化した場合の処理
        if (newState != currentState)
        {
            ChangeState(newState);
        }

        // 探索状態のタイマー管理
        if (currentState == EnemyState.Searching)
        {
            UpdateSearchTimer();
        }
    }

    /// <summary>
    /// 現在の状態を各システムの状態から判定
    /// </summary>
    private EnemyState DetermineCurrentState()
    {
        // 追跡中の場合
        if (chaseSystem != null && chaseSystem.IsChasing)
        {
            return EnemyState.Chasing;
        }

        // 探索中の場合（探索タイマーが残っている）
        if (currentState == EnemyState.Searching && searchTimer > 0)
        {
            return EnemyState.Searching;
        }

        // 巡回可能な場合
        if (navigationSystem != null && navigationSystem.IsPatrolling())
        {
            return EnemyState.Patrolling;
        }

        // その他の場合は待機
        return EnemyState.Idle;
    }

    /// <summary>
    /// 状態を変更
    /// </summary>
    private void ChangeState(EnemyState newState)
    {
        EnemyState oldState = currentState;
        previousState = currentState;
        currentState = newState;
        stateChangeTime = Time.time;

        // 状態変更時の特別な処理
        HandleStateTransition(oldState, newState);

        // イベント発火
        OnStateChanged?.Invoke(newState, oldState);
        InvokeSpecificStateEvents(newState);

        if (showStateChangeLogs)
        {
            Debug.Log($"{gameObject.name}: State changed from {oldState} to {newState}");
        }
    }

    /// <summary>
    /// 状態遷移時の特別な処理
    /// </summary>
    private void HandleStateTransition(EnemyState fromState, EnemyState toState)
    {
        // 追跡から他の状態への遷移
        if (fromState == EnemyState.Chasing && toState != EnemyState.Chasing)
        {
            // 探索状態に移行
            if (toState != EnemyState.Searching)
            {
                StartSearching();
                return;
            }
        }

        // 探索状態への遷移
        if (toState == EnemyState.Searching)
        {
            searchTimer = searchDuration;
        }
    }

    /// <summary>
    /// 特定の状態のイベントを発火
    /// </summary>
    private void InvokeSpecificStateEvents(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrolling:
                OnStartedPatrolling?.Invoke();
                break;
            case EnemyState.Chasing:
                OnStartedChasing?.Invoke();
                break;
            case EnemyState.Searching:
                OnStartedSearching?.Invoke();
                break;
            case EnemyState.Idle:
                OnBecameIdle?.Invoke();
                break;
        }
    }

    /// <summary>
    /// 探索タイマーの更新
    /// </summary>
    private void UpdateSearchTimer()
    {
        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0)
        {
            // 探索終了、巡回に戻る
            searchTimer = 0;
            StartPatrolling();
        }
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// 現在の状態を取得
    /// </summary>
    public EnemyState CurrentState => currentState;

    /// <summary>
    /// 前の状態を取得
    /// </summary>
    public EnemyState PreviousState => previousState;

    /// <summary>
    /// 現在の状態になってからの経過時間
    /// </summary>
    public float TimeInCurrentState => Time.time - stateChangeTime;

    /// <summary>
    /// 巡回中かどうか
    /// </summary>
    public bool IsPatrolling => currentState == EnemyState.Patrolling;

    /// <summary>
    /// 追跡中かどうか
    /// </summary>
    public bool IsChasing => currentState == EnemyState.Chasing;

    /// <summary>
    /// 探索中かどうか
    /// </summary>
    public bool IsSearching => currentState == EnemyState.Searching;

    /// <summary>
    /// 待機中かどうか
    /// </summary>
    public bool IsIdle => currentState == EnemyState.Idle;

    /// <summary>
    /// プレイヤーを検知しているかどうか
    /// </summary>
    public bool IsDetectingPlayer
    {
        get
        {
            return detectionSystem != null && detectionSystem.IsDetectingPlayer;
        }
    }

    /// <summary>
    /// 現在の追跡対象を取得
    /// </summary>
    public Transform GetCurrentTarget()
    {
        if (chaseSystem != null)
        {
            return chaseSystem.GetCurrentTarget();
        }
        return null;
    }

    /// <summary>
    /// 強制的に巡回状態に変更
    /// </summary>
    public void StartPatrolling()
    {
        if (navigationSystem != null)
        {
            navigationSystem.enabled = true;
        }

        // 探索タイマーをリセット
        searchTimer = 0;
    }

    /// <summary>
    /// 強制的に探索状態に変更
    /// </summary>
    public void StartSearching()
    {
        searchTimer = searchDuration;

        // 必要に応じて追跡を停止
        if (chaseSystem != null && chaseSystem.IsChasing)
        {
            chaseSystem.ClearTarget();
        }
    }

    /// <summary>
    /// 強制的に待機状態に変更
    /// </summary>
    public void BecomeIdle()
    {
        if (navigationSystem != null)
        {
            navigationSystem.enabled = false;
        }

        if (chaseSystem != null && chaseSystem.IsChasing)
        {
            chaseSystem.ClearTarget();
        }

        searchTimer = 0;
    }

    /// <summary>
    /// 状態の詳細情報を取得（デバッグ用）
    /// </summary>
    public string GetStateInfo()
    {
        string info = $"State: {currentState}\n";
        info += $"Time in State: {TimeInCurrentState:F1}s\n";

        if (currentState == EnemyState.Searching)
        {
            info += $"Search Time Remaining: {searchTimer:F1}s\n";
        }

        if (IsDetectingPlayer)
        {
            Transform target = GetCurrentTarget();
            info += $"Detecting: {(target != null ? target.name : "Unknown")}\n";
        }

        return info;
    }

    #endregion

    #region UI Display

    /// <summary>
    /// デバッグUI表示の更新
    /// </summary>
    private void UpdateUI()
    {
        // 実装は必要に応じて
        // 例：Canvas上のTextコンポーネントに状態を表示
    }

    /// <summary>
    /// GUI表示（デバッグ用）
    /// </summary>
    private void OnGUI()
    {
        if (!showStatusUI || !Application.isPlaying) return;

        // 画面左上に状態情報を表示
        GUI.Box(new Rect(10, 10, 200, 100), GetStateInfo());
    }

    #endregion

    #region Debug Visualization

    /// <summary>
    /// デバッグ用：状態に応じた色でオブジェクトを可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // 状態に応じた色でオブジェクトを表示
        Color stateColor = GetStateColor();
        Gizmos.color = stateColor;
        Gizmos.DrawWireSphere(transform.position, 1f);

        // 状態ラベルを表示
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3f,
                                 $"{gameObject.name}\n{currentState}");
#endif
    }

    /// <summary>
    /// 状態に応じた色を取得
    /// </summary>
    private Color GetStateColor()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling: return Color.green;
            case EnemyState.Chasing: return Color.red;
            case EnemyState.Searching: return Color.yellow;
            case EnemyState.Idle: return Color.gray;
            default: return Color.white;
        }
    }

    #endregion
}