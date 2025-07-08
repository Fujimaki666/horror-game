using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayRandomSound : MonoBehaviour
{
    public AudioClip[] audioClips; // �����̉����N���b�v
    private AudioSource audioSource;

    void Awake()
    {
        // AudioSource ���m���Ɏ擾
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomClip()
    {
        if (audioClips != null && audioClips.Length > 0)
        {
            int index = Random.Range(0, audioClips.Length);
            audioSource.PlayOneShot(audioClips[index]);
        }
    }
}
