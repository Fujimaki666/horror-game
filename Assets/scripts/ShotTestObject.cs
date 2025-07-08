using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTestObject : MonoBehaviour
{
    public float shot_speed;

    protected Vector3 forward;
    protected Quaternion forwardAxis;
    protected Rigidbody rb;
    protected GameObject characterObject;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        forward = characterObject.transform.forward;
    }

    void Update()
    {
        rb.velocity = forwardAxis * forward * shot_speed;
    }

    public void SetCharacterObject(GameObject characterObject)
    {
        this.characterObject = characterObject;
    }

    public void SetForwordAxis(Quaternion axis)
    {
        this.forwardAxis = axis;
    }
}
