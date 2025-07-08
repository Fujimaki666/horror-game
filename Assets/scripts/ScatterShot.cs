using System.Collections;
using UnityEngine;

public class ScatterShot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform target;
    public float stopDistance = 1.5f;     // �^�[�Q�b�g�̎�O�Ŏ~�܂鋗��
    public float bulletSpeed = 10f;       // �e�̈ړ����x
    public float bulletLifetime = 1f;     // �e�̎����i�b�j
    public float heightOffset = 1.0f;     // �����iY�j���ǂꂾ���������邩

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

        // �^�[�Q�b�g���������߂�
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // �^�[�Q�b�g�̎�O�{�������𑫂��� stopPosition
        Vector3 stopPosition = target.position - directionToTarget * stopDistance;
        stopPosition.y += heightOffset;  // �� ������������

        // �e�𐶐�
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

        // �~�܂������ƁA�c�莞�ԕ��҂��Ă������
        yield return new WaitForSeconds(Mathf.Max(0, lifetime - timer));
        Destroy(bullet);
    }
}
