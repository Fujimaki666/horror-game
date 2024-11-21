using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    public AudioClip[] audioClips; // �����̉����N���b�v��ݒ�
    public AudioSource audioSource; // �����Đ����邽�߂̃I�[�f�B�I�\�[�X

    void Start()
    {
        if (audioSource == null)
        {
            // �����I��AudioSource���A�^�b�`
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // ���V�t�g�܂��͉E�V�t�g�������ꂽ�特���Đ�
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            PlaySound();
        }
    }

    public void PlaySound()
    {
        if (audioClips.Length > 0)
        {
            // UnityEngine.Random�𖾎��I�Ɏg�p
            int randomIndex = UnityEngine.Random.Range(0, audioClips.Length);
            AudioClip selectedClip = audioClips[randomIndex];
            audioSource.clip = selectedClip; audioSource.Play();
        }
    }
}
