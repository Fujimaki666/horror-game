using UnityEngine;
public class AutomaticDoor : MonoBehaviour
{
    //　ドアのアニメーター
    [SerializeField]
    [Tooltip("自動ドアのアニメーター")]
    private Animator automaticDoorAnimator;
    /// <summary>
    /// 自動ドア検知エリアに入った時
    /// </summary>
    /// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
    {
        // アニメーションパラメータをtrueにする。(ドアが開く)
        automaticDoorAnimator.SetBool("Open", true);
    }
    /// <summary>
    /// 自動ドア検知エリアを出た時
    /// </summary>
    /// <param name="other"></param>
	private void OnTriggerExit(Collider other)
    {
        // アニメーションパラメータをfalseにする。(ドアが閉まる)
        automaticDoorAnimator.SetBool("Open", false);
    }
}