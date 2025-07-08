using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayRandomSound : MonoBehaviour
{
    public AudioClip[] audioClips; // 複数の音声クリップ
    private AudioSource audioSource;

    void Awake()
    {
        // AudioSource を確実に取得
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
