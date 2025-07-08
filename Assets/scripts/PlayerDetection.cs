using UnityEngine;

/// <summary>
/// �v���C���[���m�V�X�e���iTrigger�x�[�X�j
/// Collider�̃g���K�[�@�\���g�p���ăv���C���[�������I�Ɍ��m
/// ����p�Ə�Q���`�F�b�N��g�ݍ��킹�����m���s��
/// </summary>
public class PlayerDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public float fieldOfViewAngle = 45f;    // ����p�i�x�j
    public LayerMask obstacleLayer = 1;     // ��Q���̃��C���[�i���E���Ղ�I�u�W�F�N�g�j

    // �C�x���g�F���̃X�N���v�g�ɏ�ԕω���ʒm
    public System.Action OnPlayerDetected;  // �v���C���[�𔭌�������
    public System.Action OnPlayerLost;      // �v���C���[������������

    private Transform currentPlayer;        // ���݌��m���̃v���C���[
    private bool isPlayerInSight = false;   // �v���C���[�����E���ɂ��邩

    /// <summary>
    /// �������F�K�v�ɉ����Đݒ���m�F
    /// �g���K�[�R���C�_�[���K�v�iInspector�Őݒ�j
    /// </summary>
    void Start()
    {
        // �g���K�[�R���C�_�[�̊m�F
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
    /// �v���C���[���g���K�[�͈͓��ɂ���Ԍp���I�ɌĂ΂��
    /// ����p�Ə�Q���`�F�b�N���s���A���m��Ԃ��X�V
    /// </summary>
    /// <param name="other">�g���K�[���̃R���C�_�[</param>
    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"TRIGGER STAY: {other.name}");

        // �v���C���[�^�O�̃I�u�W�F�N�g�̂ݏ���
        if (!other.CompareTag("Player"))
        {
            Debug.Log($"Not a player tag: {other.tag}"); // �ǉ�
            return;
        }

        Debug.Log("Player tag confirmed"); // �ǉ�

        // �v���C���[�ւ̕����x�N�g�����v�Z
        Vector3 positionDelta = other.transform.position - transform.position;
        float targetAngle = Vector3.Angle(transform.forward, positionDelta);

        Debug.Log($"Target angle: {targetAngle}, FOV: {fieldOfViewAngle}"); // �ǉ�

        // ����p�`�F�b�N�F�ݒ�p�x���ɂ��邩
        if (targetAngle < fieldOfViewAngle)
        {
            Debug.Log("Player within field of view"); // �ǉ�

            // �V�����v���C���[�𔭌������ꍇ
            if (!isPlayerInSight)
            {
                Debug.Log("Detecting player for first time"); // �ǉ�
                isPlayerInSight = true;
                currentPlayer = other.transform;

                Debug.Log("Invoking OnPlayerDetected event"); // �ǉ�
                OnPlayerDetected?.Invoke();     // �v���C���[�����C�x���g
                Debug.Log("���E���A�ǐՊJ�n");
            }
            else
            {
                Debug.Log("Player already in sight"); // �ǉ�
            }
        }
        else
        {
            Debug.Log("Player outside field of view"); // �ǉ�
                                                       // ����p�O�ɂ���ꍇ
            HandlePlayerLost();
        }
    }

    /// <summary>
    /// �v���C���[���g���K�[�͈͂���o�����ɌĂ΂��
    /// ���m��Ԃ����Z�b�g
    /// </summary>
    /// <param name="other">�g���K�[����o���R���C�_�[</param>
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"=== TRIGGER EXIT: {other.name} ===");
    }

    /// <summary>
    /// �v���C���[�������������̏���
    /// �d�������������C�x���g��h��
    /// </summary>
    void HandlePlayerLost()
    {
        if (isPlayerInSight)
        {
            isPlayerInSight = false;
            currentPlayer = null;
            OnPlayerLost?.Invoke();         // �v���C���[�������C�x���g
        }
    }

    /// <summary>
    /// �v���C���[�����ݎ��E���ɂ��邩�ǂ������m�F
    /// </summary>
    /// <returns>�v���C���[�������Ă���ꍇtrue</returns>
    public bool CanSeePlayer()
    {
        return isPlayerInSight;
    }

    /// <summary>
    /// ���݌��m���̃v���C���[��Transform���擾
    /// </summary>
    /// <returns>�v���C���[��Transform�i������Ȃ��ꍇ��null�j</returns>
    public Transform GetPlayer()
    {
        return currentPlayer;
    }

    /// <summary>
    /// �v���C���[�܂ł̋������擾
    /// </summary>
    /// <returns>�v���C���[�܂ł̋���</returns>
    public float GetDistanceToPlayer()
    {
        if (currentPlayer == null) return float.MaxValue;
        return Vector3.Distance(transform.position, currentPlayer.position);
    }

    /// <summary>
    /// �f�o�b�O�p�FScene view�Ŏ��E������
    /// �G�f�B�^�ŃI�u�W�F�N�g��I���������Ɏ��E�͈͂��\�������
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // ����p��΂̐��ŕ\��
        Gizmos.color = Color.green;
        Vector3 leftDirection = Quaternion.Euler(0, -fieldOfViewAngle, 0) * transform.forward;
        Vector3 rightDirection = Quaternion.Euler(0, fieldOfViewAngle, 0) * transform.forward;

        // ����p�̋��E����`��i5���[�g���̒����j
        Gizmos.DrawRay(transform.position, leftDirection * 5f);
        Gizmos.DrawRay(transform.position, rightDirection * 5f);

        // ���s���Ƀv���C���[�������Ă���ꍇ�͐Ԃ����Őڑ�
        if (Application.isPlaying && isPlayerInSight && currentPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentPlayer.position);
        }

        // �g���K�[�R���C�_�[�̊m�F�p�i���F�Ŕ͈͕\���j
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