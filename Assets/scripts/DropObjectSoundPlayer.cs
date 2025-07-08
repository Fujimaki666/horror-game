using UnityEngine;

public class DropObjectSoundPlayer : MonoBehaviour
{
    public AudioClip hitSound; // 効果音
    private AudioSource audioSource;

    void Start()
    {
        // AudioSource を自動追加（なければ）
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 対象がターゲット（例：Player）とぶつかったとき
        if (collision.transform.CompareTag("Target"))
        {
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
    }
}
