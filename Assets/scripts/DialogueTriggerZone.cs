using UnityEngine;

public class DialogueTriggerZone : MonoBehaviour
{
    public DialogueData dialogueToPlay;      // このエリア用の会話
    public AutoDialogue dialogueManager;     // 共通の会話再生スクリプト

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(dialogueToPlay);
            Destroy(gameObject); // 一度きり
        }
    }
}
