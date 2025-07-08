using UnityEngine;
public class AutomaticDoor : MonoBehaviour
{
    //�@�h�A�̃A�j���[�^�[
    [SerializeField]
    [Tooltip("�����h�A�̃A�j���[�^�[")]
    private Animator automaticDoorAnimator;
    /// <summary>
    /// �����h�A���m�G���A�ɓ�������
    /// </summary>
    /// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
    {
        // �A�j���[�V�����p�����[�^��true�ɂ���B(�h�A���J��)
        automaticDoorAnimator.SetBool("Open", true);
    }
    /// <summary>
    /// �����h�A���m�G���A���o����
    /// </summary>
    /// <param name="other"></param>
	private void OnTriggerExit(Collider other)
    {
        // �A�j���[�V�����p�����[�^��false�ɂ���B(�h�A���܂�)
        automaticDoorAnimator.SetBool("Open", false);
    }
}