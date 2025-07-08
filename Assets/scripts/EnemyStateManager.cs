using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �G�̏�ԊǗ��V�X�e��
/// �e�R���|�[�l���g�Ԃ̘A�g���s���A�S�̂̓���𐧌�
/// ���� �� �ǐ� �� �ꎞ��~ �� ���� �̏�ԑJ�ڂ��Ǘ�
/// </summary>
public class EnemyStateManager : MonoBehaviour
{
    /// <summary>
    /// �G�̏�Ԃ��`����񋓌^
    /// </summary>
    public enum EnemyState
    {
        Patrolling,     // ����
        Chasing,        // �ǐՒ�
        Paused          // �ꎞ��~��
    }

    [Header("Behavior Settings")]
    public float pauseTimeAfterChase = 2f;  // �ǐՏI����̈ꎞ��~����

    private EnemyState currentState = EnemyState.Patrolling;   // ���݂̏��

    // �e�@�\�R���|�[�l���g�̎Q��
    private PlayerDetection playerDetection;       // �v���C���[���m�V�X�e��
    private PatrolBehavior patrolBehavior;         // ����V�X�e��
    private ChaseBehavior chaseBehavior;           // �ǐՃV�X�e��
    private EnemyAnimationController animationController;  // �A�j���[�V��������

    /// <summary>
    /// �������F�e�R���|�[�l���g���擾���ăC�x���g��o�^
    /// </summary>
    void Start()
    {
        Debug.Log("=== EnemyStateManager Start() ===");

        // �܂������I�u�W�F�N�g�Ō���
        //playerDetection = GetComponent<PlayerDetection>();

        // ������Ȃ��ꍇ�͎q�I�u�W�F�N�g�ł�����
        
            Debug.Log("PlayerDetection not found on this object, searching children...");
            playerDetection = GetComponentInChildren<PlayerDetection>();
        

        // ���̃R���|�[�l���g���擾
        patrolBehavior = GetComponent<PatrolBehavior>();
        chaseBehavior = GetComponent<ChaseBehavior>();
        animationController = GetComponent<EnemyAnimationController>();

        Debug.Log($"PlayerDetection found: {playerDetection != null}");
        if (playerDetection != null)
        {
            Debug.Log($"PlayerDetection found on: {playerDetection.gameObject.name}");
        }

        // �C�x���g�o�^
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
    /// ���t���[���Ă΂��F�ǐՒ��̓��ʂȏ���
    /// </summary>
    void Update()
    {
        if (currentState == EnemyState.Chasing)
        {
            HandleChasingUpdate();
        }
    }
    /// <summary>
    /// �ǐՒ��̍X�V����
    /// �v���C���[�̉���Ԃ��Ď����A���������Ԃ��Ǘ�
    /// </summary>
    void HandleChasingUpdate()
    {
        if (chaseBehavior == null || playerDetection == null) return;

        if (playerDetection.CanSeePlayer())
        {
            // �v���C���[�������Ă���Ԃ͍Ō�Ɍ������Ԃ��X�V
            chaseBehavior.UpdateLastSeenTime();
        }
        else if (chaseBehavior.ShouldLoseTarget())
        {
            // ��莞�Ԍ���������ǐՂ��I��
            chaseBehavior.StopChasing();
        }
    }

    /// <summary>
    /// �ǐՂ��J�n
    /// �v���C���[���m�V�X�e������Ăяo�����
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
    /// �v���C���[�������������̏���
    /// ���ۂ̌���������� HandleChasingUpdate �ōs��
    /// </summary>
    void OnPlayerLost()
    {
        Debug.Log("Player lost from sight");
        // ������������ HandleChasingUpdate �Ŏ��ԊǗ������
    }

    /// <summary>
    /// �ǐՏI�����̏���
    /// ��莞�ԑҋ@���Ă��珄��ɖ߂�
    /// </summary>
    void OnChaseEnded()
    {
        StartCoroutine(ReturnToPatrolAfterDelay());
    }

    /// <summary>
    /// ��莞�ԑҋ@���Ă��珄��ɖ߂�R���[�`��
    /// </summary>
    /// <returns>�R���[�`��</returns>
    IEnumerator ReturnToPatrolAfterDelay()
    {
        SetState(EnemyState.Paused);    // �܂��ꎞ��~��Ԃ�

        // �ݒ莞�Ԃ����ҋ@
        yield return new WaitForSeconds(pauseTimeAfterChase);

        SetState(EnemyState.Patrolling); // �����Ԃɖ߂�
    }

    /// <summary>
    /// ��Ԃ�ύX
    /// �O�̏�Ԃ̏I�������ƐV������Ԃ̊J�n�������s��
    /// </summary>
    /// <param name="newState">�V�������</param>
    void SetState(EnemyState newState)
    {
        Debug.Log($"=== SetState called: {currentState} -> {newState} ==="); // �ǉ�

        if (currentState == newState)
        {
            Debug.Log("Same state, returning"); // �ǉ�
            return;   // ������Ԃ̏ꍇ�͉������Ȃ�
        }

        // �O�̏�Ԃ��I��
        Debug.Log($"Exiting state: {currentState}"); // �ǉ�
        ExitState(currentState);

        // �V������ԂɕύX
        currentState = newState;

        // �V������Ԃ��J�n
        Debug.Log($"Entering state: {currentState}"); // �ǉ�
        EnterState(currentState);

        Debug.Log($"State changed to: {currentState}");
    }

    /// <summary>
    /// �V������Ԃɓ��鎞�̏���
    /// </summary>
    /// <param name="state">������</param>
    void EnterState(EnemyState state)
    {
        Debug.Log($"=== EnterState called with: {state} ==="); // �ǉ�

        switch (state)
        {
            case EnemyState.Patrolling:
                Debug.Log("Starting patrol"); // �ǉ�
                patrolBehavior?.StartPatrol();
                break;

            case EnemyState.Chasing:
                Debug.Log("Stopping patrol for chase"); // �ǉ�
                patrolBehavior?.StopPatrol();
                break;

            case EnemyState.Paused:
                Debug.Log("Entering paused state"); // �ǉ�
                patrolBehavior?.StopPatrol();
                animationController?.SetIdleState();
                break;
        }

        Debug.Log($"EnterState completed for: {state}"); // �ǉ�
    }

    /// <summary>
    /// ��Ԃ���o�鎞�̏���
    /// </summary>
    /// <param name="state">�o����</param>
    void ExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrolling:
                // �����~
                patrolBehavior?.StopPatrol();
                break;

            case EnemyState.Chasing:
                // �ǐՏI�������� ChaseBehavior �ōs����
                break;

            case EnemyState.Paused:
                // �A�j���[�V�������ĊJ
                animationController?.ResumeAnimation();
                break;
        }
    }

    /// <summary>
    /// �O�����猻�݂̏�Ԃ��m�F�ł���v���p�e�B�Q
    /// ���̃X�N���v�g����G�̏�Ԃ��m�F����ۂɎg�p
    /// </summary>
    public EnemyState CurrentState => currentState;     // ���݂̏��
    public bool IsChasing => currentState == EnemyState.Chasing;        // �ǐՒ���
    public bool IsPatrolling => currentState == EnemyState.Patrolling;  // ���񒆂�
    public bool IsPaused => currentState == EnemyState.Paused;          // �ꎞ��~����

    /// <summary>
    /// �I�u�W�F�N�g�j�����̏���
    /// �C�x���g�̓o�^���������ă��������[�N��h��
    /// </summary>
    void OnDestroy()
    {
        // �v���C���[���m�V�X�e���̃C�x���g�o�^����
        if (playerDetection != null)
        {
            playerDetection.OnPlayerDetected -= StartChasing;
            playerDetection.OnPlayerLost -= OnPlayerLost;
        }

        // �ǐՃV�X�e���̃C�x���g�o�^����
        if (chaseBehavior != null)
        {
            chaseBehavior.OnChaseEnded -= OnChaseEnded;
        }
    }
}