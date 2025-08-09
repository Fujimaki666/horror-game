using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] lines;          // �Z���t
    public AudioClip[] voiceClips; // �e�Z���t�̃{�C�X�i�������Ɂj
}
