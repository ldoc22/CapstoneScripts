using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPrefab_UI : MonoBehaviour
{
    

   public void SetText(string _name, string _level)
    {
        transform.Find("Name").GetComponent<Text>().text = _name;
        transform.Find("Level").GetComponent<Text>().text = _level;
    }
}
