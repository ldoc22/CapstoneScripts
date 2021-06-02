using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class UserProfile :MonoBehaviour
{


    public string publicUserName { get; private set; }
    public string level { get; private set; }
   

    public void SetProfile(UserProfile _up)
    {
        publicUserName = _up.publicUserName;
        level = _up.level;
    }

    public UserProfile(string _name, string _level)
    {
        publicUserName = _name;
        level = _level;
    }
}
