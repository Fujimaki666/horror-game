using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTarget : MonoBehaviour
{
    public Transform target;
    public float modelHeight = 2.0f;
    public void Turn()
    {
       
            // ���݂̉�]�p���擾
            Vector3 euler = target.rotation.eulerAngles;

            // �㉺���]�iX����180�x�j
            target.rotation = Quaternion.Euler(180f, euler.y, euler.z);

            // ���f���̍���������Y���W��������i�㉺�����]���Ă�̂œ��ʒu�ɑ�������j
            target.position -= new Vector3(0f, modelHeight, 0f);
    }
    
}
