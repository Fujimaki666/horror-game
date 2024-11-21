using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    public AudioClip[] audioClips; // 複数の音声クリップを設定
    public AudioSource audioSource; // 音を再生するためのオーディオソース

    void Start()
    {
        if (audioSource == null)
        {
            // 自動的にAudioSourceをアタッチ
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // 左シフトまたは右シフトが押されたら音を再生
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            PlaySound();
        }
    }

    public void PlaySound()
    {
        if (audioClips.Length > 0)
        {
            // UnityEngine.Randomを明示的に使用
            int randomIndex = UnityEngine.Random.Range(0, audioClips.Length);
            AudioClip selectedClip = audioClips[randomIndex];
            audioSource.clip = selectedClip; audioSource.Play();
        }
    }
}
