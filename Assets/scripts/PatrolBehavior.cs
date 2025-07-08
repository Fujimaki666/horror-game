using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ����V�X�e��
/// �ݒ肳�ꂽ����n�_�Ԃ��ړ�����@�\���
/// �����_������܂��͏��ԏ����I���\
/// </summary>
public class PatrolBehavior : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;        // ����n�_�̔z��
    public float patrolSpeed = 2f;          // ���񎞂̈ړ����x
    public bool randomPatrol = true;        // true�Ȃ烉���_���Afalse�Ȃ珇�ԏ���

    private NavMeshAgent agent;             // NavMeshAgent�R���|�[�l���g
    private int currentPatrolIndex = 0;     // ���݂̏���n�_�̃C���f�b�N�X
    private bool isPatrolling = false;      // ���񒆂��ǂ����̃t���O

    /// <summary>
    /// �������F�K�v�ȃR���|�[�l���g���擾
    /// </summary>
    void Awake() // Start()��Awake()�ɕύX
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
    /// ���t���[���Ă΂��F���񒆂̏��������s
    /// </summary>
    void Update()
    {
        if (isPatrolling)
        {
            HandlePatrolling();
        }
    }

    /// <summary>
    /// ���񒆂̏����F�ړI�n�ɓ��������玟�̒n�_��ݒ�
    /// </summary>
    void HandlePatrolling()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        // �ړI�n�ɋ߂Â��A���ړ����������Ă���ꍇ
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            // �G�[�W�F���g�����ۂɒ�~���Ă���ꍇ�i���x���قڃ[���j
            if (agent.velocity.sqrMagnitude < 0.1f)
            {
                SetNextPatrolPoint(); // ���̏���n�_��ݒ�
            }
        }
    }

    /// <summary>
    /// ������J�n����
    /// �O���X�N���v�g����Ăяo�����
    /// </summary>
    public void StartPatrol()
    {
        Debug.Log("StartPatrol() called"); // �ǉ�

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points set!");
            return;
        }

        Debug.Log($"Starting patrol with {patrolPoints.Length} points"); // �ǉ�
        isPatrolling = true;
        agent.speed = patrolSpeed;
        agent.isStopped = false;

        Debug.Log("Calling SetNextPatrolPoint()"); // �ǉ�
        SetNextPatrolPoint();

        Debug.Log("StartPatrol() completed"); // �ǉ�
    }

    /// <summary>
    /// ������~����
    /// �O���X�N���v�g����Ăяo�����
    /// </summary>
    public void StopPatrol()
    {
        isPatrolling = false;
        agent.isStopped = true;         // �G�[�W�F���g�̈ړ����~
        Debug.Log("Stopped patrolling");
    }

    /// <summary>
    /// ���̏���n�_��ݒ肷��
    /// randomPatrol�̐ݒ�ɉ����ă����_���܂��͏��ԂɑI��
    /// </summary>
    void SetNextPatrolPoint()
    {
        Debug.Log("SetNextPatrolPoint() called"); // �ǉ�

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

        Debug.Log($"Selected patrol index: {currentPatrolIndex}"); // �ǉ�

        if (patrolPoints[currentPatrolIndex] != null)
        {
            Vector3 destination = patrolPoints[currentPatrolIndex].position;
            Debug.Log($"Setting destination to: {destination}"); // �ǉ�

            agent.SetDestination(destination);

            Debug.Log($"Destination set. HasPath: {agent.hasPath}, PathPending: {agent.pathPending}"); // �ǉ�
        }
        else
        {
            Debug.LogError($"Patrol point {currentPatrolIndex} is null!");
        }
    }

    /// <summary>
    /// ���񒆂��ǂ������m�F
    /// </summary>
    /// <returns>���񒆂̏ꍇtrue</returns>
    public bool IsPatrolling()
    {
        return isPatrolling;
    }

    /// <summary>
    /// ���݂̖ڕW�n�_���擾
    /// </summary>
    /// <returns>���݂̖ڕWTransform</returns>
    public Transform GetCurrentTarget()
    {
        if (patrolPoints != null && currentPatrolIndex < patrolPoints.Length)
        {
            return patrolPoints[currentPatrolIndex];
        }
        return null;
    }
}