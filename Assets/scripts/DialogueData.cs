using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] lines;          // セリフ
    public AudioClip[] voiceClips; // 各セリフのボイス（同じ数に）
}
