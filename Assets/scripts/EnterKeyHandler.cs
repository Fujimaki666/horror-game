using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI; // UI関連の機能を使用

public class EnterKeyToPressButton : MonoBehaviour
{
    public Button targetButton; // 対象のボタンをアサイン

    void Update()
    {
        // エンターキー（Return）を検知
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (targetButton != null)
            {
                // ボタンをプログラム的にクリック
                targetButton.onClick.Invoke();
            }
            else
            {
                
            }
        }
    }
}
