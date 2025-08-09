using UnityEngine;

public class DialogueTriggerZone : MonoBehaviour
{
    public DialogueData dialogueToPlay;      // ���̃G���A�p�̉�b
    public AutoDialogue dialogueManager;     // ���ʂ̉�b�Đ��X�N���v�g

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(dialogueToPlay);
            Destroy(gameObject); // ��x����
        }
    }
}
