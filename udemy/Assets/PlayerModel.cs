
using UnityEngine;

public class PlayerModel
{
    // Start is called before the first frame update
    string name;
    int hp;
    int at;

    public string Name
     {
       get{ return name; }
       set{ name = value; }
      }

    public int HP
    {
        get { return hp; }
        set { hp = value; }
    }

    public PlayerModel()
    {
        name = "haruko";
    }

    public PlayerModel(string name)
    {
        this.name = name;
        hp = 100;
        at = 50;
    }
    public void SayName()
    {
        Debug.Log(name);
    }
   
}
