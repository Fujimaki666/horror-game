using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHP : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Start()
    {
        string characterType = "Friend";

        switch(characterType)
        {
            case "Player":
            case "Friend":
                Debug.Log("����");
            break;
            case "Enemy":
            Debug.Log("�G");
            break;
            
            default:
            Debug.Log("���̑�");
            break;
        }
        if(characterType ==  "Friend"|| characterType == "Player")
        {
            Debug.Log("����");
        }
        else
        {
            Debug.Log("�G");
        }

    }
}

    // Update is called once per frame


