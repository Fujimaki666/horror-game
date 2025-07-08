using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerModel player = new PlayerModel("nanana");
       
        PlayerModel player2 = new PlayerModel("anpan");
        Debug.Log(player.Name);
        player.Name = "xxxxx";
        Debug.Log(player.Name);
    }

    // Update is called once per frame
    
}
