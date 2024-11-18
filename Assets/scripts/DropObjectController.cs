using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DropObjectController : MonoBehaviour
{
    public GameObject dropObjectPrefab; // 降らせる物のPrefab
    public Transform targetObject;     // ターゲットとなるオブジェクト
    public float dropHeight = 10f;     // 物を生成する高さ

    // ボタンを押したときに呼び出されるメソッド
    public void DropObject()
    {
        if (dropObjectPrefab != null && targetObject != null)
        {
            // ターゲットの上に物を生成
            Vector3 spawnPosition = targetObject.position + Vector3.up * dropHeight;
            Instantiate(dropObjectPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            //Debug.LogError("Prefabまたはターゲットオブジェクトが設定されていません！");
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //Debug.Log("カーソルが再表示されました");
    }
}
