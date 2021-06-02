using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;


    public HealthBar CharacterHealthPanel;
    public HealthBar TargetHealthBar;
    public bool InventoryEnabled;
    public bool CharacterEquipmentPanelEnabled;
    public Slider IntroductionBar;

    public GameObject StartMenu;
    public InputField usernameField;

    public GameObject LoadingScreen;

    public Chatbox chatbox;

    Coroutine introduction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance of UIManager alreadyt exists");
            Destroy(this);
        }

        // ConnectToServer();
        LoadingScreen.SetActive(true);
        IntroductionBar.gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(Screen.width/2, Screen.height-50, 50, 50), "Slash"))
        {
            ClientSend.SendAbilityToServer(0);
        }
        if (GUI.Button(new Rect(Screen.width / 2 + 50, Screen.height-50, 50, 50), "Fireball"))
        {
            ClientSend.SendAbilityToServer(100);
        }
        if (GUI.Button(new Rect(Screen.width / 2 + 100, Screen.height-50, 50, 50), "Shock"))
        {
            ClientSend.SendAbilityToServer(101);
        }
    }

    public void StartIntroduction(float _time, Action<bool> _callback)
    {
        
        if (introduction == null)
        {
            introduction = StartCoroutine(Introduction(_time, _callback));
            Debug.Log("Start Introduction");
        }
    }

    public IEnumerator Introduction(float _time, Action<bool> _callback)
    {
        IntroductionBar.gameObject.SetActive(true);
        IntroductionBar.value = 0;
        float currentTime = 0;
        while(currentTime <= _time)
        {
            currentTime += Time.deltaTime;
            IntroductionBar.value = currentTime / _time;
            yield return null;
        }
        introduction = null;
        _callback(true);
        IntroductionBar.gameObject.SetActive(false);

    }

    public void SetCharacterPanelStats(float  _maxHealth,float  _currentHealth, float _maxMana, float _currentMana )
    {
        CharacterHealthPanel.InitHealthBar(false, _maxHealth, _currentHealth); 
        CharacterHealthPanel.InitManaBar(_maxMana, _currentMana);
    }

    public void SetTargetPanel(Interactable _target)
    {
        TargetHealthBar.gameObject.SetActive(true);
        TargetHealthBar.InitHealthBar(_target.isNPC, _target.MaxHealth, _target.Health);
        TargetHealthBar.InitManaBar(_target.MaxMana, _target.Mana);

    }
    public void DisableTargetPanel()
    {
        TargetHealthBar.gameObject.SetActive(false);
    }

    void Start()
    {
        ConnectToServer();
        
    }

    public void ConnectToServer()
    {
        StartMenu.SetActive(false);
        usernameField.interactable = false;
        //Client.instance.ConnectToServer();
    }

    

    public void ReceiveWorldChat(int _id,int _channel, string _msg)
    {
        chatbox.RecieveChat(_id, _channel, _msg);
    
    }


    public void ToggleInventory()
    {
        InventoryEnabled = !InventoryEnabled;
        Inventory.instance.isEnabled = InventoryEnabled;
    }



    public void ToggleCharacterEquipmentPanel()
    {
        CharacterEquipmentPanelEnabled = !CharacterEquipmentPanelEnabled;
        CharacterEquipmentPanel.instance.enableUI = CharacterEquipmentPanelEnabled;
    }

    public void SetLoading(bool _isLoading)
    {
        LoadingScreen.SetActive(_isLoading);
    }






    ////////

   
  
}
