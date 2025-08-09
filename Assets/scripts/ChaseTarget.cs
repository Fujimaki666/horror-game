using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// 敵の追跡システム
/// プレイヤーを追跡し、アニメーションを制御する
/// 巡回システムとの連携も行う
/// </summary>
public class ChaseTarget : MonoBehaviour
{
    [Header("Chase Settings")]
    [Tooltip("追跡時の移動速度")]
    public float chaseSpeed = 5f;

    [Header("Animation Settings")]
    [Tooltip("アニメーション制御を有効にするか")]
    public bool enableAnimationControl = true;

    [Header("Debug")]
    [Tooltip("デバッグログを表示するか")]
    public bool showDebugLogs = true;

    // 追跡状態
    private Transform currentTarget;              // 現在の追跡対象
    private bool isChasing = false;              // 追跡中フラグ
    private bool isPaused = false;               // 一時停止中フラグ

    // コンポーネント参照
    private NavMeshAgent agent;                  // 移動制御
    private Animator animator;                   // アニメーション制御
    private Navigation navigationSystem;         // 巡回システム

    // アニメーション設定
    private readonly string SPEED_PARAMETER = "Speed";
    private readonly string IS_WALKING_PARAMETER = "isWalking";
    private const float MOVEMENT_THRESHOLD = 0.1f;

    private AudioSource audioSource;
    public AudioClip himei;

    public bool isReacting = false;
    public GameObject hpslider;
    public GameObject playerslider;
    #region Unity Lifecycle

    //敵からのダメージ
    private bool canDamage = true;
    public float damageCooldown = 2f; // 2秒クールタイム
    void Start()
    {
        InitializeComponents();
        ConfigureAgent();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPaused) return;

        HandleChaseLogic();
        UpdateAnimations();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// 必要なコンポーネントを取得・検証
    /// </summary>
    private void InitializeComponents()
    {
        // NavMeshAgent取得（必須）
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"{gameObject.name}: NavMeshAgent component is required!");
            return;
        }

        // Animator取得（オプション）
        animator = GetComponent<Animator>();
        if (animator == null && enableAnimationControl)
        {
            Debug.LogWarning($"{gameObject.name}: Animator component not found. Animation control disabled.");
            enableAnimationControl = false;
        }

        // Navigation取得（オプション）
        navigationSystem = GetComponent<Navigation>();
        if (navigationSystem == null)
        {
            Debug.LogWarning($"{gameObject.name}: Navigation component not found. Patrol integration disabled.");
        }
    }

    /// <summary>
    /// NavMeshAgentの初期設定
    /// </summary>
    private void ConfigureAgent()
    {
        if (agent != null)
        {
            agent.speed = chaseSpeed;
        }
    }

    #endregion

    #region Chase Logic

    /// <summary>
    /// 追跡ロジックの処理
    /// </summary>
    private void HandleChaseLogic()
    {
        if (!isChasing || currentTarget == null || agent == null) return;

        // ターゲットの位置を目的地として設定
        agent.SetDestination(currentTarget.position);
    }

    /// <summary>
    /// 追跡を開始
    /// </summary>
    /// <param name="target">追跡対象のTransform</param>
    public void SetTarget(Transform target)
    {
        
        if (target == null)
        {
            Debug.LogWarning($"{gameObject.name}: Attempted to set null target for chase!");
            return;
        }

        currentTarget = target;
        isChasing = true;
        if (isChasing)
        {
            //hpslider.SetActive(false);
            
            //playerslider.SetActive(true);
        }
        // 巡回システムを停止
        StopPatrolSystem();

        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Started chasing {target.name}");
        }
    }

    /// <summary>
    /// 追跡を終了
    /// </summary>
    public void ClearTarget()
    {
        if (currentTarget != null && showDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Stopped chasing {currentTarget.name}, returning to patrol");
        }

        currentTarget = null;
        isChasing = false;
        //hpslider.SetActive(true);
        //playerslider.SetActive(false);
        // 巡回システムを再開
        ResumePatrolSystem();
    }

    #endregion

    #region Patrol System Integration

    /// <summary>
    /// 巡回システムを停止
    /// </summary>
    private void StopPatrolSystem()
    {
        if (navigationSystem != null)
        {
            navigationSystem.enabled = false;
        }
    }

    /// <summary>
    /// 巡回システムを再開
    /// </summary>
    private void ResumePatrolSystem()
    {
         //hpslider.SetActive(true);
            //playerslider.SetActive(false);
        
        if (navigationSystem != null)
        {
            navigationSystem.enabled = true;
        }
    }

    #endregion




    #region PlayerDamage
    private void OnTriggerEnter(Collider other)
    {
        if (isChasing && canDamage && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
                StartCoroutine(DamageCooldown());
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }

    #endregion



    #region Animation Control

    /// <summary>
    /// アニメーションの更新
    /// </summary>
    private void UpdateAnimations()
    {
        if (!enableAnimationControl || animator == null || agent == null) return;

        float currentSpeed = agent.velocity.magnitude;
        bool isMoving = currentSpeed > MOVEMENT_THRESHOLD;

        // Animatorパラメータを更新
        UpdateAnimatorParameter(SPEED_PARAMETER, currentSpeed);
        UpdateAnimatorParameter(IS_WALKING_PARAMETER, isMoving);
    }

    /// <summary>
    /// Animatorパラメータを安全に更新
    /// </summary>
    private void UpdateAnimatorParameter(string parameterName, float value)
    {
        if (HasAnimatorParameter(parameterName))
        {
            animator.SetFloat(parameterName, value);
        }
    }

    /// <summary>
    /// Animatorパラメータを安全に更新（bool版）
    /// </summary>
    private void UpdateAnimatorParameter(string parameterName, bool value)
    {
        if (HasAnimatorParameter(parameterName))
        {
            animator.SetBool(parameterName, value);
        }
    }

    /// <summary>
    /// Animatorパラメータが存在するかチェック
    /// </summary>
    private bool HasAnimatorParameter(string parameterName)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName)
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Special Actions

    /// <summary>
    /// その場で180度回転して一時停止
    /// 特殊なアクション用（必要に応じて使用）
    /// </summary>
    public void TurnAround()
    {
        transform.Rotate(0, 180f, 0);
        StartCoroutine(PauseForDuration(5f));
    }

    /// <summary>
    /// 指定時間だけ一時停止
    /// </summary>
    public IEnumerator PauseForDuration(float duration)
    {
        isPaused = true;
        audioSource.PlayOneShot(himei);
        // アニメーションを停止
        if (enableAnimationControl && animator != null)
        {
            UpdateAnimatorParameter(SPEED_PARAMETER, 0f);
            UpdateAnimatorParameter(IS_WALKING_PARAMETER, false);
            animator.speed = 0f;

        }

        // 移動を停止
        if (agent != null)
        {
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(duration);

        // 再開
        if (enableAnimationControl && animator != null)
        {
            animator.speed = 1f;
        }

        if (agent != null)
        {
            agent.isStopped = false;
        }

        isPaused = false;
        GameManager.Instance?.SetPhase(GamePhase.Patrol);
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// 現在追跡中かどうか
    /// </summary>
    public bool IsChasing => isChasing && currentTarget != null;

    /// <summary>
    /// 現在の追跡対象を取得
    /// </summary>
    public Transform GetCurrentTarget() => currentTarget;

    /// <summary>
    /// 一時停止中かどうか
    /// </summary>
    public bool IsPaused => isPaused;

    /// <summary>
    /// 追跡対象が設定されているかどうか
    /// </summary>
    public bool HasTarget => currentTarget != null;

    /// <summary>
    /// 追跡システムが有効かどうか
    /// </summary>
    public bool IsActive => enabled && agent != null;

    #endregion

    #region Debug Visualization

    /// <summary>
    /// デバッグ用：追跡状態を可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // 追跡対象への線を描画
        if (isChasing && currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.position);
            Gizmos.DrawWireSphere(currentTarget.position, 0.5f);
        }

        // NavMeshAgentのパスを描画
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.green;
            Vector3[] pathCorners = agent.path.corners;
            for (int i = 0; i < pathCorners.Length - 1; i++)
            {
                Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
            }
        }
    }

    #endregion
}
