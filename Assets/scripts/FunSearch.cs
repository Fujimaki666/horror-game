using UnityEngine;
using UnityEngine.UI;


public class FunSearch : MonoBehaviour
{
    public float angle = 45f;
    private ChaseTarget chaser;
    // スライダーの参照
    public GameObject hpslider;
    public GameObject playerslider;
    private void Start()
    {
        chaser = GetComponentInParent<ChaseTarget>();
        if (chaser == null)
        {
            Debug.LogError("ChaseTarget component not found in parent!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            // 敵がリアクション中ならスキップ
            if (chaser != null && chaser.isReacting)
            {
                return;
            }

            // スケアーフェーズ中は追跡フェーズに移行しない
            if (GameManager.Instance != null && GameManager.Instance.currentPhase == GamePhase.Scare)
            {
                return;
            }
            Vector3 posDelta = other.transform.position - transform.position;
            float target_angle = Vector3.Angle(transform.forward, posDelta);
            if (target_angle < angle)
            {
                if (Physics.Raycast(transform.position, posDelta, out RaycastHit hit))
                {
                    
                    if (hit.collider == other)
                    {
                        GameManager.Instance?.SetPhase(GamePhase.Chase);
                        //hpslider.SetActive(false);
                        //playerslider.SetActive(true);
                        Debug.Log("視界内、追跡開始");
                        chaser?.SetTarget(other.transform);
                        //GameManager.Instance.SetPhase(GamePhase.Chase);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance?.SetPhase(GamePhase.Patrol);
            Debug.Log("視界外、追跡終了");
            //hpslider.SetActive(true);
            //playerslider.SetActive(false);
            chaser?.ClearTarget();
            //GameManager.Instance.SetPhase(GamePhase.Patrol);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 leftDir = Quaternion.Euler(0, -angle, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, angle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftDir * 5f);
        Gizmos.DrawRay(transform.position, rightDir * 5f);
    }

    public bool IsDetectingPlayer => chaser != null && chaser.GetCurrentTarget() != null;

}