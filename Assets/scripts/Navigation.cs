using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �G�̏���V�X�e��
/// �ݒ肳�ꂽ����n�_�Ԃ������_���Ɉړ�����
/// </summary>
public class Navigation : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("���񂷂�n�_�̔z��")]
    public Transform[] goals;

    [Header("Movement Settings")]
    [Tooltip("�ړI�n���B�̔��苗��")]
    public float arrivalDistance = 0.5f;

    [Header("Debug")]
    [Tooltip("�f�o�b�O���O��\�����邩")]
    public bool showDebugLogs = true;

    // �v���C�x�[�g�ϐ�
    private int currentDestinationIndex = 0;     // ���݂̖ړI�n�ԍ�
    private NavMeshAgent agent;                  // NavMeshAgent�R���|�[�l���g

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
    /// ���񒆂��ǂ������m�F
    /// </summary>
    public bool IsPatrolling()
    {
        return enabled && agent != null && !agent.isStopped && goals != null && goals.Length > 0;
    }

    /// <summary>
    /// ����V�X�e�����L�����ǂ���
    /// </summary>
    public bool IsActive()
    {
        return enabled && agent != null;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// ����V�X�e���̏�����
    /// </summary>
    private void InitializeNavigation()
    {
        // NavMeshAgent�R���|�[�l���g���擾
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"{gameObject.name}: NavMeshAgent component not found!");
            return;
        }

        // ����n�_�̌���
        if (!ValidateGoals())
        {
            return;
        }

        // �ŏ��̖ړI�n��ݒ�
        MoveToNextGoal();
    }

    /// <summary>
    /// ����n�_�̐ݒ肪���������`�F�b�N
    /// </summary>
    private bool ValidateGoals()
    {
        if (goals == null || goals.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: No patrol goals set! Please assign patrol points.");
            return false;
        }

        // null�n�_���`�F�b�N
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
    /// �ړI�n�ւ̓��B���`�F�b�N
    /// </summary>
    private void CheckArrival()
    {
        if (agent == null || goals.Length == 0) return;

        // �ړI�n�ɋ߂Â��A�ړ����������Ă���ꍇ
        bool nearDestination = agent.remainingDistance < arrivalDistance;
        bool pathComplete = !agent.pathPending;
        bool hasStoppedMoving = agent.velocity.sqrMagnitude < 0.1f;

        if (nearDestination && pathComplete && hasStoppedMoving)
        {
            MoveToNextGoal();
        }
    }

    /// <summary>
    /// ���̏���n�_�Ɉړ�
    /// </summary>
    private void MoveToNextGoal()
    {
        if (goals.Length == 0) return;

        // �����_���Ɏ��̒n�_��I���i���ݒn�_�ȊO�j
        int nextIndex = SelectNextGoal();
        currentDestinationIndex = nextIndex;

        // �I�����ꂽ�n�_���L�����`�F�b�N
        if (goals[currentDestinationIndex] == null)
        {
            Debug.LogWarning($"{gameObject.name}: Selected patrol goal {currentDestinationIndex} is null!");
            return;
        }

        // �ړI�n��ݒ�
        Vector3 destination = goals[currentDestinationIndex].position;
        agent.SetDestination(destination);

        // �f�o�b�O���O
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Moving to patrol goal {currentDestinationIndex} " +
                     $"({goals[currentDestinationIndex].name}) at {destination}");
        }
    }

    /// <summary>
    /// ���̏���n�_�������_���ɑI��
    /// ���݂̒n�_�Ɠ����n�_�͔�����
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
    /// ����V�X�e�����ꎞ��~
    /// �ǐՃ��[�h���ɌĂ΂��
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
    /// ����V�X�e�����ĊJ
    /// �ǐՏI�����ɌĂ΂��
    /// </summary>
    public void ResumePatrol()
    {
        if (agent != null)
        {
            agent.isStopped = false;
            // ���݂̖ړI�n�Ɍ������čĊJ
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
    /// ���݂̖ړI�n���擾
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
    /// �f�o�b�O�p�F����n�_�ƌo�H������
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (goals == null || goals.Length == 0) return;

        // ����n�_��`��
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i] == null) continue;

            // ���݂̖ړI�n�͐ԁA���̑��͐ŕ\��
            Gizmos.color = (i == currentDestinationIndex) ? Color.red : Color.blue;
            Gizmos.DrawWireSphere(goals[i].position, 1f);

            // �n�_�ԍ���\���i�G�f�B�^�ł̂݁j
#if UNITY_EDITOR
            UnityEditor.Handles.Label(goals[i].position + Vector3.up * 2f, $"Goal {i}");
#endif
        }

        // ���݂̃p�X��`��
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