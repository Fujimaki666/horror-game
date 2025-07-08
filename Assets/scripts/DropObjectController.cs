using UnityEngine;

public class DropObjectController : MonoBehaviour
{
    public GameObject dropObjectPrefab;
    public Transform targetObject;
    public float dropHeight = 4f;

    public void DropObject()
    {
        if (dropObjectPrefab != null && targetObject != null)
        {
            Vector3 spawnPosition = targetObject.position + Vector3.up * dropHeight;
            GameObject dropped = Instantiate(dropObjectPrefab, spawnPosition, Quaternion.identity);            // 1�b��ɍ폜
            Destroy(dropped, 2.5f);
        }
        Debug.Log("yobareta");
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
    }
}
