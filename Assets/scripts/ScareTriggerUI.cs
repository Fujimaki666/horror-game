using UnityEngine;
using UnityEngine.UI;

public class ScareTriggerUI : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject text; // 表示するボタン
    public RandomEffectManager effectManager;

    [Header("Enemy Detection")]
    [Tooltip("監視する敵のChaseTargetコンポーネント")]
    public ChaseTarget enemyChaseTarget; // 敵の追跡システムへの参照

    private bool canTrigger = false;
    private bool isUIVisible = false;
    private bool scareQueued = false;
    private bool scareInProgress = false;

    void Start()
    {
        text.SetActive(false); // 最初はボタン非表示にする

        // enemyChaseTargetが設定されていない場合、自動で検索
        if (enemyChaseTarget == null)
        {
            enemyChaseTarget = FindObjectOfType<ChaseTarget>();
            if (enemyChaseTarget == null)
            {
                Debug.LogWarning("ChaseTarget component not found! Enemy chase detection will not work.");
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("右クリック検知");
        }
        Debug.Log($"Phase: {GameManager.Instance.currentPhase}");
        if (GameManager.Instance.currentPhase == GamePhase.Chase)
        {
            Debug.Log("フェーズがChaseのため処理中断");
            return;
        }

        // 敵が追跡中かチェック
        bool enemyIsChasing = IsEnemyChasing();
        Debug.Log($"Update - canTrigger:{canTrigger}, RightClick:{Input.GetMouseButtonDown(1)}");

        // 敵が追跡中の場合、UIを強制的に隠す
        if (enemyIsChasing && isUIVisible)
        {
            Debug.Log("敵が追跡中なのでUIを非表示にして中断");
            ForceHideUI();
            return;
        }
        // 発動予約があれば実行
        if (scareQueued)
        {
            Debug.Log("スケア予約発動");
            scareQueued = false;
            GameManager.Instance?.SetPhase(GamePhase.Scare);
            effectManager.TriggerRandomEffect();
            HideScareUI();
            return;
        }

        // 敵が追跡中でない場合のみ、通常の処理を実行
        if (!enemyIsChasing && canTrigger && Input.GetMouseButtonDown(1))
        {
            //GameManager.Instance?.SetPhase(GamePhase.Scare);
            Debug.Log("スケア発動！");
            
            //effectManager.TriggerRandomEffect();
            
            //HideScareUI();
            scareQueued = true;
        }
    }

    /// <summary>
    /// 敵が現在追跡中かどうかを確認
    /// </summary>
    private bool IsEnemyChasing()
    {
        if (enemyChaseTarget == null) return false;
        return enemyChaseTarget.IsChasing;
    }

    /// <summary>
    /// スケアUIを表示（敵が追跡中でない場合のみ）
    /// </summary>
    public void ShowScareUI()
    {
        // 敵が追跡中の場合は表示しない
        if (IsEnemyChasing())
        {
            Debug.Log("敵が追跡中のため、スケアUIは表示されません");
            return;
        }

        canTrigger = true;
        isUIVisible = true;
        text.SetActive(true);
        //Debug.Log("スケアUI表示！");
    }

    /// <summary>
    /// スケアUIを隠す
    /// </summary>
    public void HideScareUI()
    {
        canTrigger = false;
        isUIVisible = false;
        text.SetActive(false);
        Debug.Log("スケアUI非表示");
    }

    /// <summary>
    /// 強制的にUIを隠す（敵の追跡開始時用）
    /// </summary>
    private void ForceHideUI()
    {
        canTrigger = false;
        isUIVisible = false;
        text.SetActive(false);
        Debug.Log("敵の追跡開始により、スケアUIを強制非表示");
    }

    /// <summary>
    /// デバッグ用：現在の状態を表示
    /// </summary>
    public void DebugCurrentState()
    {
        Debug.Log($"UI状態 - 表示可能: {canTrigger}, UI表示中: {isUIVisible}, 敵追跡中: {IsEnemyChasing()}");
    }
}
