using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character 
{
   public string publicUserName { get; private set; }
    public string level { get; private set; }


    public string dbID { get; private set; }
    public void SetProfile(string _username, string _level, string _id)
    {
        publicUserName = _username;
        level = _level;
        dbID = _id;
    }
    public Character()
    {
       
    }

    public bool isEmpty()
    {
        if (publicUserName == null || level == null) return true;
        return false;
    }

    
}
