using UnityEngine;
using UnityEngine.UI;

public class ScareTriggerUI : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject text; // �\������{�^���iCanvas��ɐݒu�j
    public RandomEffectManager effectManager;

    [Header("Enemy Detection")]
    [Tooltip("�Ď�����G��ChaseTarget�R���|�[�l���g")]
    public ChaseTarget enemyChaseTarget; // �G�̒ǐՃV�X�e���ւ̎Q��

    private bool canTrigger = false;
    private bool isUIVisible = false;

    void Start()
    {
        text.SetActive(false); // �ŏ��̓{�^����\���ɂ���

        // enemyChaseTarget���ݒ肳��Ă��Ȃ��ꍇ�A�����Ō���
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
        // �G���ǐՒ����`�F�b�N
        bool enemyIsChasing = IsEnemyChasing();

        // �G���ǐՒ��̏ꍇ�AUI�������I�ɉB��
        if (enemyIsChasing && isUIVisible)
        {
            ForceHideUI();
            return;
        }

        // �G���ǐՒ��łȂ��ꍇ�̂݁A�ʏ�̏��������s
        if (!enemyIsChasing && canTrigger && Input.GetMouseButtonDown(1))
        {
            effectManager.TriggerRandomEffect();
            HideScareUI();
        }
    }

    /// <summary>
    /// �G�����ݒǐՒ����ǂ������m�F
    /// </summary>
    private bool IsEnemyChasing()
    {
        if (enemyChaseTarget == null) return false;
        return enemyChaseTarget.IsChasing;
    }

    /// <summary>
    /// �X�P�AUI��\���i�G���ǐՒ��łȂ��ꍇ�̂݁j
    /// </summary>
    public void ShowScareUI()
    {
        // �G���ǐՒ��̏ꍇ�͕\�����Ȃ�
        if (IsEnemyChasing())
        {
            Debug.Log("�G���ǐՒ��̂��߁A�X�P�AUI�͕\������܂���");
            return;
        }

        canTrigger = true;
        isUIVisible = true;
        text.SetActive(true);
        Debug.Log("�X�P�AUI�\���I");
    }

    /// <summary>
    /// �X�P�AUI���B��
    /// </summary>
    public void HideScareUI()
    {
        canTrigger = false;
        isUIVisible = false;
        text.SetActive(false);
        Debug.Log("�X�P�AUI��\��");
    }

    /// <summary>
    /// �����I��UI���B���i�G�̒ǐՊJ�n���p�j
    /// </summary>
    private void ForceHideUI()
    {
        canTrigger = false;
        isUIVisible = false;
        text.SetActive(false);
        Debug.Log("�G�̒ǐՊJ�n�ɂ��A�X�P�AUI��������\��");
    }

    /// <summary>
    /// �f�o�b�O�p�F���݂̏�Ԃ�\��
    /// </summary>
    public void DebugCurrentState()
    {
        Debug.Log($"UI��� - �\���\: {canTrigger}, UI�\����: {isUIVisible}, �G�ǐՒ�: {IsEnemyChasing()}");
    }
}
