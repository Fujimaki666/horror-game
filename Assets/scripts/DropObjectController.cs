using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DropObjectController : MonoBehaviour
{
    public GameObject dropObjectPrefab; // �~�点�镨��Prefab
    public Transform targetObject;     // �^�[�Q�b�g�ƂȂ�I�u�W�F�N�g
    public float dropHeight = 10f;     // ���𐶐����鍂��

    // �{�^�����������Ƃ��ɌĂяo����郁�\�b�h
    public void DropObject()
    {
        if (dropObjectPrefab != null && targetObject != null)
        {
            // �^�[�Q�b�g�̏�ɕ��𐶐�
            Vector3 spawnPosition = targetObject.position + Vector3.up * dropHeight;
            Instantiate(dropObjectPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            //Debug.LogError("Prefab�܂��̓^�[�Q�b�g�I�u�W�F�N�g���ݒ肳��Ă��܂���I");
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //Debug.Log("�J�[�\�����ĕ\������܂���");
    }
}
