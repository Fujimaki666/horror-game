using UnityEngine;
using System;

/// <summary>
/// �G�̌y�ʏ�ԊǗ��V�X�e��
/// ������Navigation�AChaseTarget�AFunSearch�ƘA�g���đS�̂̏�Ԃ��Ǘ�
/// �e�R���|�[�l���g�̓Ɨ�����ۂ��Ȃ���A���ꂳ�ꂽ��Ԋm�F���
/// </summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// �G�̍s�����
    /// </summary>
    public enum EnemyState
    {
        Patrolling,    // ����
        Chasing,       // �ǐՒ�
        Searching,     // �T�����i�v���C���[��������������j
        Idle           // �ҋ@��
    }

    [Header("State Management")]
    [Tooltip("���݂̓G�̏�ԁi�ǂݎ���p�j")]
    [SerializeField] private EnemyState currentState = EnemyState.Patrolling;

    [Header("State Transition Settings")]
    [Tooltip("�ǐՏI����̒T�����ԁi�b�j")]
    public float searchDuration = 3f;

    [Header("Debug")]
    [Tooltip("��ԕω��̃��O��\�����邩")]
    public bool showStateChangeLogs = true;

    [Tooltip("���݂̏�Ԃ�UI�ɕ\�����邩")]
    public bool showStatusUI = true;

    // �R���|�[�l���g�Q��
    private Navigation navigationSystem;
    private ChaseTarget chaseSystem;
    private FunSearch detectionSystem;

    // ��ԊǗ�
    private EnemyState previousState;
    private float stateChangeTime;
    private float searchTimer;

    // �C�x���g�i���̃V�X�e�����w�ǉ\�j
    public event Action<EnemyState, EnemyState> OnStateChanged; // (�V���, �����)
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
    /// �R���g���[���[�̏�����
    /// </summary>
    private void InitializeController()
    {
        // �e�V�X�e���R���|�[�l���g���擾
        navigationSystem = GetComponent<Navigation>();
        chaseSystem = GetComponent<ChaseTarget>();
        detectionSystem = GetComponentInChildren<FunSearch>();

        // �R���|�[�l���g�̑��݊m�F
        ValidateComponents();

        // ������Ԃ�ݒ�
        previousState = currentState;
        stateChangeTime = Time.time;

        if (showStateChangeLogs)
        {
            Debug.Log($"{gameObject.name}: EnemyController initialized. Initial state: {currentState}");
        }
    }

    /// <summary>
    /// �K�v�ȃR���|�[�l���g�����݂��邩�`�F�b�N
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
    /// ��ԃ��W�b�N�̍X�V
    /// </summary>
    private void UpdateStateLogic()
    {
        // �e�V�X�e���̏�Ԃ��m�F���āA�K�؂ȏ�Ԃ�����
        EnemyState newState = DetermineCurrentState();

        // ��Ԃ��ω������ꍇ�̏���
        if (newState != currentState)
        {
            ChangeState(newState);
        }

        // �T����Ԃ̃^�C�}�[�Ǘ�
        if (currentState == EnemyState.Searching)
        {
            UpdateSearchTimer();
        }
    }

    /// <summary>
    /// ���݂̏�Ԃ��e�V�X�e���̏�Ԃ��画��
    /// </summary>
    private EnemyState DetermineCurrentState()
    {
        // �ǐՒ��̏ꍇ
        if (chaseSystem != null && chaseSystem.IsChasing)
        {
            return EnemyState.Chasing;
        }

        // �T�����̏ꍇ�i�T���^�C�}�[���c���Ă���j
        if (currentState == EnemyState.Searching && searchTimer > 0)
        {
            return EnemyState.Searching;
        }

        // ����\�ȏꍇ
        if (navigationSystem != null && navigationSystem.IsPatrolling())
        {
            return EnemyState.Patrolling;
        }

        // ���̑��̏ꍇ�͑ҋ@
        return EnemyState.Idle;
    }

    /// <summary>
    /// ��Ԃ�ύX
    /// </summary>
    private void ChangeState(EnemyState newState)
    {
        EnemyState oldState = currentState;
        previousState = currentState;
        currentState = newState;
        stateChangeTime = Time.time;

        // ��ԕύX���̓��ʂȏ���
        HandleStateTransition(oldState, newState);

        // �C�x���g����
        OnStateChanged?.Invoke(newState, oldState);
        InvokeSpecificStateEvents(newState);

        if (showStateChangeLogs)
        {
            Debug.Log($"{gameObject.name}: State changed from {oldState} to {newState}");
        }
    }

    /// <summary>
    /// ��ԑJ�ڎ��̓��ʂȏ���
    /// </summary>
    private void HandleStateTransition(EnemyState fromState, EnemyState toState)
    {
        // �ǐՂ��瑼�̏�Ԃւ̑J��
        if (fromState == EnemyState.Chasing && toState != EnemyState.Chasing)
        {
            // �T����ԂɈڍs
            if (toState != EnemyState.Searching)
            {
                StartSearching();
                return;
            }
        }

        // �T����Ԃւ̑J��
        if (toState == EnemyState.Searching)
        {
            searchTimer = searchDuration;
        }
    }

    /// <summary>
    /// ����̏�Ԃ̃C�x���g�𔭉�
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
    /// �T���^�C�}�[�̍X�V
    /// </summary>
    private void UpdateSearchTimer()
    {
        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0)
        {
            // �T���I���A����ɖ߂�
            searchTimer = 0;
            StartPatrolling();
        }
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// ���݂̏�Ԃ��擾
    /// </summary>
    public EnemyState CurrentState => currentState;

    /// <summary>
    /// �O�̏�Ԃ��擾
    /// </summary>
    public EnemyState PreviousState => previousState;

    /// <summary>
    /// ���݂̏�ԂɂȂ��Ă���̌o�ߎ���
    /// </summary>
    public float TimeInCurrentState => Time.time - stateChangeTime;

    /// <summary>
    /// ���񒆂��ǂ���
    /// </summary>
    public bool IsPatrolling => currentState == EnemyState.Patrolling;

    /// <summary>
    /// �ǐՒ����ǂ���
    /// </summary>
    public bool IsChasing => currentState == EnemyState.Chasing;

    /// <summary>
    /// �T�������ǂ���
    /// </summary>
    public bool IsSearching => currentState == EnemyState.Searching;

    /// <summary>
    /// �ҋ@�����ǂ���
    /// </summary>
    public bool IsIdle => currentState == EnemyState.Idle;

    /// <summary>
    /// �v���C���[�����m���Ă��邩�ǂ���
    /// </summary>
    public bool IsDetectingPlayer
    {
        get
        {
            return detectionSystem != null && detectionSystem.IsDetectingPlayer;
        }
    }

    /// <summary>
    /// ���݂̒ǐՑΏۂ��擾
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
    /// �����I�ɏ����ԂɕύX
    /// </summary>
    public void StartPatrolling()
    {
        if (navigationSystem != null)
        {
            navigationSystem.enabled = true;
        }

        // �T���^�C�}�[�����Z�b�g
        searchTimer = 0;
    }

    /// <summary>
    /// �����I�ɒT����ԂɕύX
    /// </summary>
    public void StartSearching()
    {
        searchTimer = searchDuration;

        // �K�v�ɉ����ĒǐՂ��~
        if (chaseSystem != null && chaseSystem.IsChasing)
        {
            chaseSystem.ClearTarget();
        }
    }

    /// <summary>
    /// �����I�ɑҋ@��ԂɕύX
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
    /// ��Ԃ̏ڍ׏����擾�i�f�o�b�O�p�j
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
    /// �f�o�b�OUI�\���̍X�V
    /// </summary>
    private void UpdateUI()
    {
        // �����͕K�v�ɉ�����
        // ��FCanvas���Text�R���|�[�l���g�ɏ�Ԃ�\��
    }

    /// <summary>
    /// GUI�\���i�f�o�b�O�p�j
    /// </summary>
    private void OnGUI()
    {
        if (!showStatusUI || !Application.isPlaying) return;

        // ��ʍ���ɏ�ԏ���\��
        GUI.Box(new Rect(10, 10, 200, 100), GetStateInfo());
    }

    #endregion

    #region Debug Visualization

    /// <summary>
    /// �f�o�b�O�p�F��Ԃɉ������F�ŃI�u�W�F�N�g������
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // ��Ԃɉ������F�ŃI�u�W�F�N�g��\��
        Color stateColor = GetStateColor();
        Gizmos.color = stateColor;
        Gizmos.DrawWireSphere(transform.position, 1f);

        // ��ԃ��x����\��
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3f,
                                 $"{gameObject.name}\n{currentState}");
#endif
    }

    /// <summary>
    /// ��Ԃɉ������F���擾
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