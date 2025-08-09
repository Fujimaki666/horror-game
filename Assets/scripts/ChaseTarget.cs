using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// �G�̒ǐՃV�X�e��
/// �v���C���[��ǐՂ��A�A�j���[�V�����𐧌䂷��
/// ����V�X�e���Ƃ̘A�g���s��
/// </summary>
public class ChaseTarget : MonoBehaviour
{
    [Header("Chase Settings")]
    [Tooltip("�ǐՎ��̈ړ����x")]
    public float chaseSpeed = 5f;

    [Header("Animation Settings")]
    [Tooltip("�A�j���[�V���������L���ɂ��邩")]
    public bool enableAnimationControl = true;

    [Header("Debug")]
    [Tooltip("�f�o�b�O���O��\�����邩")]
    public bool showDebugLogs = true;

    // �ǐՏ��
    private Transform currentTarget;              // ���݂̒ǐՑΏ�
    private bool isChasing = false;              // �ǐՒ��t���O
    private bool isPaused = false;               // �ꎞ��~���t���O

    // �R���|�[�l���g�Q��
    private NavMeshAgent agent;                  // �ړ�����
    private Animator animator;                   // �A�j���[�V��������
    private Navigation navigationSystem;         // ����V�X�e��

    // �A�j���[�V�����ݒ�
    private readonly string SPEED_PARAMETER = "Speed";
    private readonly string IS_WALKING_PARAMETER = "isWalking";
    private const float MOVEMENT_THRESHOLD = 0.1f;

    private AudioSource audioSource;
    public AudioClip himei;

    public bool isReacting = false;
    public GameObject hpslider;
    public GameObject playerslider;
    #region Unity Lifecycle

    //�G����̃_���[�W
    private bool canDamage = true;
    public float damageCooldown = 2f; // 2�b�N�[���^�C��
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
    /// �K�v�ȃR���|�[�l���g���擾�E����
    /// </summary>
    private void InitializeComponents()
    {
        // NavMeshAgent�擾�i�K�{�j
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"{gameObject.name}: NavMeshAgent component is required!");
            return;
        }

        // Animator�擾�i�I�v�V�����j
        animator = GetComponent<Animator>();
        if (animator == null && enableAnimationControl)
        {
            Debug.LogWarning($"{gameObject.name}: Animator component not found. Animation control disabled.");
            enableAnimationControl = false;
        }

        // Navigation�擾�i�I�v�V�����j
        navigationSystem = GetComponent<Navigation>();
        if (navigationSystem == null)
        {
            Debug.LogWarning($"{gameObject.name}: Navigation component not found. Patrol integration disabled.");
        }
    }

    /// <summary>
    /// NavMeshAgent�̏����ݒ�
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
    /// �ǐՃ��W�b�N�̏���
    /// </summary>
    private void HandleChaseLogic()
    {
        if (!isChasing || currentTarget == null || agent == null) return;

        // �^�[�Q�b�g�̈ʒu��ړI�n�Ƃ��Đݒ�
        agent.SetDestination(currentTarget.position);
    }

    /// <summary>
    /// �ǐՂ��J�n
    /// </summary>
    /// <param name="target">�ǐՑΏۂ�Transform</param>
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
        // ����V�X�e�����~
        StopPatrolSystem();

        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Started chasing {target.name}");
        }
    }

    /// <summary>
    /// �ǐՂ��I��
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
        // ����V�X�e�����ĊJ
        ResumePatrolSystem();
    }

    #endregion

    #region Patrol System Integration

    /// <summary>
    /// ����V�X�e�����~
    /// </summary>
    private void StopPatrolSystem()
    {
        if (navigationSystem != null)
        {
            navigationSystem.enabled = false;
        }
    }

    /// <summary>
    /// ����V�X�e�����ĊJ
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
    /// �A�j���[�V�����̍X�V
    /// </summary>
    private void UpdateAnimations()
    {
        if (!enableAnimationControl || animator == null || agent == null) return;

        float currentSpeed = agent.velocity.magnitude;
        bool isMoving = currentSpeed > MOVEMENT_THRESHOLD;

        // Animator�p�����[�^���X�V
        UpdateAnimatorParameter(SPEED_PARAMETER, currentSpeed);
        UpdateAnimatorParameter(IS_WALKING_PARAMETER, isMoving);
    }

    /// <summary>
    /// Animator�p�����[�^�����S�ɍX�V
    /// </summary>
    private void UpdateAnimatorParameter(string parameterName, float value)
    {
        if (HasAnimatorParameter(parameterName))
        {
            animator.SetFloat(parameterName, value);
        }
    }

    /// <summary>
    /// Animator�p�����[�^�����S�ɍX�V�ibool�Łj
    /// </summary>
    private void UpdateAnimatorParameter(string parameterName, bool value)
    {
        if (HasAnimatorParameter(parameterName))
        {
            animator.SetBool(parameterName, value);
        }
    }

    /// <summary>
    /// Animator�p�����[�^�����݂��邩�`�F�b�N
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
    /// ���̏��180�x��]���Ĉꎞ��~
    /// ����ȃA�N�V�����p�i�K�v�ɉ����Ďg�p�j
    /// </summary>
    public void TurnAround()
    {
        transform.Rotate(0, 180f, 0);
        StartCoroutine(PauseForDuration(5f));
    }

    /// <summary>
    /// �w�莞�Ԃ����ꎞ��~
    /// </summary>
    public IEnumerator PauseForDuration(float duration)
    {
        isPaused = true;
        audioSource.PlayOneShot(himei);
        // �A�j���[�V�������~
        if (enableAnimationControl && animator != null)
        {
            UpdateAnimatorParameter(SPEED_PARAMETER, 0f);
            UpdateAnimatorParameter(IS_WALKING_PARAMETER, false);
            animator.speed = 0f;

        }

        // �ړ����~
        if (agent != null)
        {
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(duration);

        // �ĊJ
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
    /// ���ݒǐՒ����ǂ���
    /// </summary>
    public bool IsChasing => isChasing && currentTarget != null;

    /// <summary>
    /// ���݂̒ǐՑΏۂ��擾
    /// </summary>
    public Transform GetCurrentTarget() => currentTarget;

    /// <summary>
    /// �ꎞ��~�����ǂ���
    /// </summary>
    public bool IsPaused => isPaused;

    /// <summary>
    /// �ǐՑΏۂ��ݒ肳��Ă��邩�ǂ���
    /// </summary>
    public bool HasTarget => currentTarget != null;

    /// <summary>
    /// �ǐՃV�X�e�����L�����ǂ���
    /// </summary>
    public bool IsActive => enabled && agent != null;

    #endregion

    #region Debug Visualization

    /// <summary>
    /// �f�o�b�O�p�F�ǐՏ�Ԃ�����
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // �ǐՑΏۂւ̐���`��
        if (isChasing && currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.position);
            Gizmos.DrawWireSphere(currentTarget.position, 0.5f);
        }

        // NavMeshAgent�̃p�X��`��
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
