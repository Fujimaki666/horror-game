using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI; // UI�֘A�̋@�\���g�p

public class EnterKeyToPressButton : MonoBehaviour
{
    public Button targetButton; // �Ώۂ̃{�^�����A�T�C��

    void Update()
    {
        // �G���^�[�L�[�iReturn�j�����m
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (targetButton != null)
            {
                // �{�^�����v���O�����I�ɃN���b�N
                targetButton.onClick.Invoke();
            }
            else
            {
                
            }
        }
    }
}
