using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject shotObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateShotObject(0);
            CreateShotObject(30f);
            CreateShotObject(-30f);
        }
    }

    private void CreateShotObject(float axis)
    {
        GameObject shot = Instantiate(shotObject, transform.position, Quaternion.identity);
        var shotTestObject = shot.GetComponent<ShotTestObject>();
        shotTestObject.SetCharacterObject(gameObject);
        shotTestObject.SetForwordAxis(Quaternion.AngleAxis(axis, Vector3.up));
    }
}
