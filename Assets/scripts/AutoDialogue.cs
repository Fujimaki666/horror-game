using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutoDialogue : MonoBehaviour
{
    public Text dialogueText;
    public AudioSource audioSource;
    public float lineDelay = 2.5f;

    public void StartDialogue(DialogueData data)
    {
        StartCoroutine(PlayDialogue(data));
    }

    IEnumerator PlayDialogue(DialogueData data)
    {
        dialogueText.gameObject.SetActive(true);

        for (int i = 0; i < data.lines.Length; i++)
        {
            dialogueText.text = data.lines[i];

            if (i < data.voiceClips.Length && data.voiceClips[i] != null)
            {
                audioSource.PlayOneShot(data.voiceClips[i]);
            }

            yield return new WaitForSeconds(lineDelay);
        }

        dialogueText.text = "";
        dialogueText.gameObject.SetActive(false);
    }
}
