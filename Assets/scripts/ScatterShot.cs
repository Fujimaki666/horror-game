using System.Collections;
using UnityEngine;

public class ScatterShot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform target;
    public float stopDistance = 1.5f;     // ターゲットの手前で止まる距離
    public float bulletSpeed = 10f;       // 弾の移動速度
    public float bulletLifetime = 1f;     // 弾の寿命（秒）
    public float heightOffset = 1.0f;     // 高さ（Y）をどれだけ浮かせるか

    /*void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Fire();
        }
    }*/

    public void Fire()
    {
        if (target == null) return;

        // ターゲット方向を求める
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // ターゲットの手前＋高さ分を足した stopPosition
        Vector3 stopPosition = target.position - directionToTarget * stopDistance;
        stopPosition.y += heightOffset;  // ← 高さを加える

        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        StartCoroutine(MoveBullet(bullet, stopPosition, bulletLifetime));
    }

    IEnumerator MoveBullet(GameObject bullet, Vector3 stopPosition, float lifetime)
    {
        float timer = 0f;

        while (timer < lifetime && Vector3.Distance(bullet.transform.position, stopPosition) > 0.05f)
        {
            bullet.transform.position = Vector3.MoveTowards(
                bullet.transform.position,
                stopPosition,
                bulletSpeed * Time.deltaTime
            );
            timer += Time.deltaTime;
            yield return null;
        }

        // 止まったあと、残り時間分待ってから消す
        yield return new WaitForSeconds(Mathf.Max(0, lifetime - timer));
        Destroy(bullet);
    }
}
