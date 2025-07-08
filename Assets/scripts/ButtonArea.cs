using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonArea : MonoBehaviour
{
    
    private ScareTriggerUI scareUI;
    private void Start()
    {
       
        scareUI = FindObjectOfType<ScareTriggerUI>();
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            scareUI?.ShowScareUI();
            
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            scareUI?.HideScareUI();
            
        }
    }

}
