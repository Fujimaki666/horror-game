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
                Debug.Log("–¡•û");
            break;
            case "Enemy":
            Debug.Log("“G");
            break;
            
            default:
            Debug.Log("‚»‚Ì‘¼");
            break;
        }
        if(characterType ==  "Friend"|| characterType == "Player")
        {
            Debug.Log("–¡•û");
        }
        else
        {
            Debug.Log("“G");
        }

    }
}

    // Update is called once per frame


