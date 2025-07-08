using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Text hpText;
    public Slider hpSlider;
    // Start is called before the first frame update
    void Start()
    {
        hpText.text = "50";
        hpSlider.value = 50;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0.1f, 0, 0);
        
    }
}
