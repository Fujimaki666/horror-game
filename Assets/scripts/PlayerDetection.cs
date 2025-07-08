using UnityEngine;

/// <summary>
/// プレイヤー検知システム（Triggerベース）
/// Colliderのトリガー機能を使用してプレイヤーを効率的に検知
/// 視野角と障害物チェックを組み合わせた検知を行う
/// </summary>
public class PlayerDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public float fieldOfViewAngle = 45f;    // 視野角（度）
    public LayerMask obstacleLayer = 1;     // 障害物のレイヤー（視界を遮るオブジェクト）

    // イベント：他のスクリプトに状態変化を通知
    public System.Action OnPlayerDetected;  // プレイヤーを発見した時
    public System.Action OnPlayerLost;      // プレイヤーを見失った時

    private Transform currentPlayer;        // 現在検知中のプレイヤー
    private bool isPlayerInSight = false;   // プレイヤーが視界内にいるか

    /// <summary>
    /// 初期化：必要に応じて設定を確認
    /// トリガーコライダーが必要（Inspectorで設定）
    /// </summary>
    void Start()
    {
        // トリガーコライダーの確認
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogWarning("PlayerDetection requires a Trigger Collider! Please add a Collider and set 'Is Trigger' to true.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"=== TRIGGER ENTER: {other.name} ===");
    }

    /// <summary>
    /// プレイヤーがトリガー範囲内にいる間継続的に呼ばれる
    /// 視野角と障害物チェックを行い、検知状態を更新
    /// </summary>
    /// <param name="other">トリガー内のコライダー</param>
    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"TRIGGER STAY: {other.name}");

        // プレイヤータグのオブジェクトのみ処理
        if (!other.CompareTag("Player"))
        {
            Debug.Log($"Not a player tag: {other.tag}"); // 追加
            return;
        }

        Debug.Log("Player tag confirmed"); // 追加

        // プレイヤーへの方向ベクトルを計算
        Vector3 positionDelta = other.transform.position - transform.position;
        float targetAngle = Vector3.Angle(transform.forward, positionDelta);

        Debug.Log($"Target angle: {targetAngle}, FOV: {fieldOfViewAngle}"); // 追加

        // 視野角チェック：設定角度内にいるか
        if (targetAngle < fieldOfViewAngle)
        {
            Debug.Log("Player within field of view"); // 追加

            // 新しくプレイヤーを発見した場合
            if (!isPlayerInSight)
            {
                Debug.Log("Detecting player for first time"); // 追加
                isPlayerInSight = true;
                currentPlayer = other.transform;

                Debug.Log("Invoking OnPlayerDetected event"); // 追加
                OnPlayerDetected?.Invoke();     // プレイヤー発見イベント
                Debug.Log("視界内、追跡開始");
            }
            else
            {
                Debug.Log("Player already in sight"); // 追加
            }
        }
        else
        {
            Debug.Log("Player outside field of view"); // 追加
                                                       // 視野角外にいる場合
            HandlePlayerLost();
        }
    }

    /// <summary>
    /// プレイヤーがトリガー範囲から出た時に呼ばれる
    /// 検知状態をリセット
    /// </summary>
    /// <param name="other">トリガーから出たコライダー</param>
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"=== TRIGGER EXIT: {other.name} ===");
    }

    /// <summary>
    /// プレイヤーを見失った時の処理
    /// 重複した見失いイベントを防ぐ
    /// </summary>
    void HandlePlayerLost()
    {
        if (isPlayerInSight)
        {
            isPlayerInSight = false;
            currentPlayer = null;
            OnPlayerLost?.Invoke();         // プレイヤー見失いイベント
        }
    }

    /// <summary>
    /// プレイヤーが現在視界内にいるかどうかを確認
    /// </summary>
    /// <returns>プレイヤーが見えている場合true</returns>
    public bool CanSeePlayer()
    {
        return isPlayerInSight;
    }

    /// <summary>
    /// 現在検知中のプレイヤーのTransformを取得
    /// </summary>
    /// <returns>プレイヤーのTransform（見つからない場合はnull）</returns>
    public Transform GetPlayer()
    {
        return currentPlayer;
    }

    /// <summary>
    /// プレイヤーまでの距離を取得
    /// </summary>
    /// <returns>プレイヤーまでの距離</returns>
    public float GetDistanceToPlayer()
    {
        if (currentPlayer == null) return float.MaxValue;
        return Vector3.Distance(transform.position, currentPlayer.position);
    }

    /// <summary>
    /// デバッグ用：Scene viewで視界を可視化
    /// エディタでオブジェクトを選択した時に視界範囲が表示される
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // 視野角を緑の線で表示
        Gizmos.color = Color.green;
        Vector3 leftDirection = Quaternion.Euler(0, -fieldOfViewAngle, 0) * transform.forward;
        Vector3 rightDirection = Quaternion.Euler(0, fieldOfViewAngle, 0) * transform.forward;

        // 視野角の境界線を描画（5メートルの長さ）
        Gizmos.DrawRay(transform.position, leftDirection * 5f);
        Gizmos.DrawRay(transform.position, rightDirection * 5f);

        // 実行中にプレイヤーが見えている場合は赤い線で接続
        if (Application.isPlaying && isPlayerInSight && currentPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentPlayer.position);
        }

        // トリガーコライダーの確認用（黄色で範囲表示）
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = Color.yellow;
            if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(transform.position, sphere.radius);
            }
            else if (col is BoxCollider box)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawWireCube(box.center, box.size);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}