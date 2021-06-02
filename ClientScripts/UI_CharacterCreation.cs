using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPGCharacters;

public class UI_CharacterCreation : MonoBehaviour
{



    public bool enableUI;
    public DemoItemEquipper itemEquipper;
    public DemoCamera CreationCamera;
    [SerializeField]
    private string username;
    public GUIStyle username_box;
    public GUIStyle Create_Button;
    private void OnGUI()
    {
        if (enableUI)
        {
            itemEquipper.enableUI = true;
            CreationCamera.enableUI = true;
            
            username = GUI.TextField(new Rect(Screen.width - 400, Screen.height - 100, 200, 40), username, 16, username_box);
            username_box.fontSize = 20;

            if (GUI.Button(new Rect(Screen.width - 150, Screen.height - 150, 150, 150), "Create Character"))
            {
                CreateCharacter();
            }


            if (GUI.Button(new Rect((Screen.width / 2) - 100, 0, 100, 50), "Exit Creation"))
            {
                Main.instance.login.ReturnToCharacterSelection();
            }

        }
        else
        {
            itemEquipper.enableUI = false;
            CreationCamera.enableUI = false;
        }

    }

    public void CreateCharacter()
    {
        if (username.Equals("") || username == null || username.Contains("character") || username.Contains("name")) return;
        //StartCoroutine(Main.instance.web.CreateCharacter(Main.instance.userInfo.UserID, username, Callback_Creation));
        string list = username + "," + itemEquipper.selectedCharacter.race.ToString() + "," + itemEquipper.selectedCharacter.gender.ToString()
            + "," + itemEquipper.selectedCharacter.hairStyle.ToString()
            + "," + itemEquipper.selectedCharacter.beardStyle.ToString()
            + "," + itemEquipper.selectedCharacter.eyebrowStyle.ToString()
            + "," + ColorUtility.ToHtmlStringRGB(itemEquipper.selectedCharacter.skinColor)
            + "," + ColorUtility.ToHtmlStringRGB(itemEquipper.selectedCharacter.eyeColor)
            + "," + ColorUtility.ToHtmlStringRGB(itemEquipper.selectedCharacter.hairColor)
             + "," + ColorUtility.ToHtmlStringRGB(itemEquipper.selectedCharacter.mouthColor);

        ClientSend.CreateCharacter(list);
    }

    public void Callback_Creation(string _result)
    {
        if (_result.Contains("Exists"))
        {
            Debug.Log("Name Already Exists");
        }
        else if(_result.Contains("Created"))
        {
            Main.instance.login.ReturnToCharacterSelection();
        }
        else
        {
            Debug.Log(_result);
        }
    }

    
}
