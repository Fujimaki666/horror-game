using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Charahealth : MonoBehaviour
{
    // 敵の最大HPと現在のHP
    public int maxHp = 10;
    private int currentHp;

    // スライダーの参照
    public Slider hpSlider;

    void Start()
    {
        // 初期設定
        currentHp = maxHp; // HPを最大値に設定
        hpSlider.maxValue = maxHp; // スライダーの最大値を設定
        hpSlider.value = currentHp; // 現在のHPを反映
    }

    public void TakeDamage(int damage)
    {
        // HPを減らす処理
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;

        // スライダーに現在のHPを反映
        hpSlider.value = currentHp;

        // HPが0になったときの処理
        if (currentHp == 0)
        {
            Debug.Log("クリア！");
            // ここにゲームオーバーの処理を追加
        }
    }
}
