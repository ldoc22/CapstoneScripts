using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Account : MonoBehaviour
{

    public static Account instance;
    
    public int ID { get;  private set; }
    public int dbID { get;  private set; }
    public int charID { get;  private set; }

    public bool Spoofed;

    private void OnEnable()
    {
        if (instance == null) instance = this;
        else if(instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);
        character = new Character();
        if (Spoofed)
        {

        }
    }

    // Start is called before the first frame update

    public Character character;
    public void Initialize(int _id,int _dbID, int _charID)
    {
        ID = _id;
        dbID = _dbID;
        charID = _charID;
    }

    public void SetCharID(int _id)
    {
        charID = _id;
    }



}
