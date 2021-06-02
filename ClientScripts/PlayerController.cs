using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    

    public bool ableToMove;
    public CameraController cam;
    public  float rotateSpeed = 150.0f;
    private Vector3 moveDirection = Vector3.zero;
    public bool TestMode;
    public float time;

    private void Start()
    {
        time = 0;
    }
    public void Update()
    {
        
       
        if (Input.GetKeyDown(KeyCode.L))
        {
            Inventory.instance.CreateItems();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            UIManager.instance.ToggleInventory();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            UIManager.instance.ToggleCharacterEquipmentPanel();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!GameManager.players[Client.instance.myId].role.isBusy)
            {
                bool b = GameManager.players[Client.instance.myId].role.anim.GetBool("isArmed");
                GameManager.players[Client.instance.myId].role.anim.SetBool("isArmed", !b);
            }
        }

        if (Input.GetKeyDown(KeyCode.N)){
            ableToMove = true;
        }
       


        if (cam.inFirstPerson == false)
        {
            if (Input.GetMouseButton(1))
            {
                transform.rotation = Quaternion.Euler(0,  cam.gameObject.GetComponent<Camera>().transform.eulerAngles.y, 0);
            }
            else
            {
                transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);

            }
        }
        else
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            }

        }

        if (TestMode)
        {
            if(time > 4)
            {
                RandomizeInput();
                time = 0;
            }
            time += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestMode = !TestMode;
        }

    }


    public void RandomizeInput()
    {
        bool[] _inputs;
        _inputs = new bool[] { false, false, false, false, false };
        int _randomBool = Random.Range(0, 3);
        _inputs[_randomBool] = true;

        ClientSend.PlayerMovement(_inputs);
        if(_randomBool == 2)
        {
            ClientSend.SendSetTarget(1, true);
            ClientSend.SendAbilityToServer(100);
        }
        ClientSend.SendWorldChat(0,RandomString(Random.Range(2, 20)));
    }


    public static string RandomString(int length)
    {
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string _msg = string.Empty;
        for (int i = 0; i < length; i++)
        {
            _msg += chars[Random.Range(0, chars.Length)];
        }
        return _msg;
        
        
    }


    public void FixedUpdate()
    {
        if(ableToMove)
            SendInputToServer();

    }


    private void SendInputToServer()
    {

        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)

        };
        /*
        if (GameManager.players[Client.instance.myId].role.isBusy)
        {
            _inputs = new bool[] { false, false, false, false, false };
            GameManager.players[Client.instance.myId].role.SetHorizontalRun(0);
        }
        */
        try
        {
            if (!GameManager.players[Client.instance.myId].role.isBusy)
            {
                float x = 0, y = 0;
                if (_inputs[0])
                {
                    x = 1f;
                }
                if (_inputs[1])
                {
                    x = -1f;
                }
                if (_inputs[2])
                {
                    y = -1f;
                }
                if (_inputs[3])
                {
                    y = 1f;
                }

                
            }
        }
        catch
        {

        }


        ClientSend.PlayerMovement(_inputs);
        if(_inputs[0] == true)
            GameManager.players[Client.instance.myId].role.SetHorizontalRun(1);
        else if(_inputs[1] == true)
            GameManager.players[Client.instance.myId].role.SetHorizontalRun(-1);
        for (int i = 0; i < _inputs.Length; i++)
        {
            if (!_inputs[i])
            {
                if(i == _inputs.Length-1)
                    GameManager.players[Client.instance.myId].role.SetHorizontalRun(0);
                continue;
            }
            else
            {
                break;
            }

        }


    }
}
