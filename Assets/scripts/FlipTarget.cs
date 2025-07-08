using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTarget : MonoBehaviour
{
    public Transform target;
    public float modelHeight = 2.0f;
    public void Turn()
    {
       
            // 現在の回転角を取得
            Vector3 euler = target.rotation.eulerAngles;

            // 上下反転（X軸に180度）
            target.rotation = Quaternion.Euler(180f, euler.y, euler.z);

            // モデルの高さ分だけY座標を下げる（上下が反転してるので頭位置に足が来る）
            target.position -= new Vector3(0f, modelHeight, 0f);
    }
    
}
