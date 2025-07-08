using UnityEngine;

public class DropObjectSoundPlayer : MonoBehaviour
{
    public AudioClip hitSound; // ���ʉ�
    private AudioSource audioSource;

    void Start()
    {
        // AudioSource �������ǉ��i�Ȃ���΁j
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // �Ώۂ��^�[�Q�b�g�i��FPlayer�j�ƂԂ������Ƃ�
        if (collision.transform.CompareTag("Target"))
        {
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
    }
}
