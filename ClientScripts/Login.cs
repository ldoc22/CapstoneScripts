using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [Header("Panels")]
    public GameObject Background;
    public GameObject LoadScrene;
    public GameObject login;
    public GameObject Register;
    public GameObject Inputs;
    
    public GameObject CharacterSelection;
    public GameObject CharacterCreation;


    [Header("Inputs and Buttons")]
    public Button backButton;
    public Button createUserButton;
    public Button backToLoginButton;
    public Button LoginButton;
    public Button RegisterButton;
    public Button ToRegisterButton;
    public InputField username;
    public InputField password;
    public InputField confirmpassword;
    // Start is called before the first frame update
    void Start()
    {
        backToLoginButton.onClick.AddListener(() => SetLoginModeActive());
        LoginButton.onClick.AddListener(() => SendLogin());
        RegisterButton.onClick.AddListener(() => SendRegister());
        ToRegisterButton.onClick.AddListener(() => SetRegisterModeActive());
        
     

    }

    public void SetRegisterModeActive()
    {
        Inputs.SetActive(true);
        Register.SetActive(true);
        login.SetActive(false);
    }

    private void SendLogin()
    {
        ClientSend.Login(username.text.Trim(), password.text.Trim());
    }
    private void SendRegister()
    {
        ClientSend.Register(username.text.Trim(), password.text.Trim(), confirmpassword.text.Trim());
    }

    public void SetLoginModeActive()
    {
        Inputs.SetActive(true);
        Register.SetActive(false);
        login.SetActive(true);
    }


    public void EnableUserCreation()
    {

    }

    //enable or disable load 
    public void E_D_Load(bool enable)
    {
        if (enable)
        {
            LoadScrene.SetActive(true);
        }
        else
        {
            LoadScrene.SetActive(false);
            CharacterSelection.SetActive(true);
        }
    }


    public void LoginSuccessful()
    {
        Inputs.SetActive(false);
        CharacterSelection.SetActive(true);
        Register.SetActive(false);
        login.SetActive(false);
        CharacterManager.instance.CreateCharacters();

    }

    public void LoginFailed()
    {
        Debug.Log("Login Failed");
    }

    public void CreationCallback(bool _success)
    {
        if (_success)
        {
            ReturnToCharacterSelection();
        }
        else
        {
            Debug.Log("Failed to Create Character");
        }
    }

    public void ReturnToCharacterSelection()
    {
        Background.SetActive(true);
        CharacterSelection.SetActive(true);
        CharacterManager.instance.Reset();
        CharacterManager.instance.CreateCharacters();
        
        CharacterCreation.GetComponent<UI_CharacterCreation>().enableUI = false;
        CharacterCreation.SetActive(false);
    }

    public void CreateCharacterActivate()
    {
        Background.SetActive(false);
        CharacterSelection.SetActive(false);
        CharacterCreation.SetActive(true);
        CharacterCreation.GetComponent<UI_CharacterCreation>().enableUI = true;
        
    }

    

    
}
