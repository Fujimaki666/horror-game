using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sakuteki : MonoBehaviour
{
    public Transform player;

    void Start()
    {

    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.name == "PlayerCapsule")
        {
            transform.LookAt(player);
            transform.Translate(0, 0, 0.1f);
        }

    }
}
