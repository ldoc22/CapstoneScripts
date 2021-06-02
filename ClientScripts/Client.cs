using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Net.Sockets;
using UnityEngine.SceneManagement;



public class Client : MonoBehaviour
{


    public static Client instance;

    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    public bool loggedIn;

    public Account account;
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        if (scene.name.Contains("Menu")) return;
        loggedIn = true;
        OnSceneLoad();
    }

    void OnSceneLoad()
    {
        if (loggedIn)
        {
            port = 2456;
            ConnectToServer();
            
            
        }
        else
        {
            Debug.Log("Not Logged in");
            StartCoroutine(Wait(2f));
        }

    }

    IEnumerator Wait(float _time)
    {
        yield return new WaitForSeconds(_time);
        OnSceneLoad();
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Debug.Log("Instance of Client alreadyt exists");
            Destroy(gameObject);
            return;
        }
        instance.isConnected = false;
        DontDestroyOnLoad(this.gameObject);
        account = Account.instance;
        loggedIn = false;
        
        
       
        
    }

    

    

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        if (loggedIn)
        {
            Debug.Log("Initialize package handlers");
            InitializeClientData();
        }
        else
            InitializeLoginClientData();

        tcp = new TCP();
        udp = new UDP();
        
        isConnected = true;
        tcp.Connect();
    }

    public bool IsConnected()
    {
        return isConnected;
    }

    public class TCP 
    {
        public TcpClient socket;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receiveBuffer = null;
            receivedData = null;
            socket = null;
        }
    
        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();
            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if(socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }catch(Exception _ex)
            {
                Debug.Log($"Error send data to server: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {

                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);
                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Disconnect();
            }
        }


        private bool HandleData(byte [] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if(receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if(_packetLength <= 0)
                {
                    return true;
                }
            }

            while(_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if(_packetLength <= 1)
            {
                return true;
            }
            return false;
        }

    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;


        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);
                if(socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch(Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using(Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        private void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if(_data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                Disconnect();
            }
        }

        private void HandleData(byte [] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
            using (Packet _packet = new Packet(_data))
            {
                int _packetId = _packet.ReadInt();
                packetHandlers[_packetId](_packet);
                }
            });
        }
    }

    private void InitializeLoginClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int) LoginServerPackets.Connected, Welcome},
            {(int) LoginServerPackets.Disconnected, Disconnect},
            {(int) LoginServerPackets.LoginResponse, ClientHandle.HandleLogin},
            {(int) LoginServerPackets.Characters, ClientHandle.HandleCharacters},
            {(int) LoginServerPackets.CreateCharacter, ClientHandle.HandleCharacterCreation},
            {(int) LoginServerPackets.Launch, ClientHandle.LaunchGranted}
        };
    }

    private static void Welcome(Packet _packet)
    {
        Debug.Log("Connected to Login Server");
    }


    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int) ServerPackets.welcome, ClientHandle.Welcome},
            {(int) ServerPackets.ping, ClientHandle.PingHandle},
            {(int) ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            {(int) ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            {(int) ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            {(int) ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            {(int) ServerPackets.playerHealth, ClientHandle.PlayerHealth },
            {(int) ServerPackets.playerDeath, ClientHandle.PlayerRespawned },
            {(int) ServerPackets.createItemSpawner, ClientHandle.CreateItemSpawner },
            {(int) ServerPackets.itemSpawned, ClientHandle.ItemSpawned },
            {(int) ServerPackets.itemPickedUp, ClientHandle.ItemPickedUp },
            {(int) ServerPackets.spawnProjectile, ClientHandle.SpawnProjectile },
            {(int) ServerPackets.projectilePosition, ClientHandle.ProjectilePosition },
            {(int) ServerPackets.projectileExplosion, ClientHandle.ProjectileExploded },
            {(int) ServerPackets.updateNPC, ClientHandle.NPCHandle },
            {(int) ServerPackets.NPCposition, ClientHandle.NPCPosition },
            {(int) ServerPackets.SetTarget, ClientHandle.SetTarget },
            {(int) ServerPackets.worldChatRecieve, ClientHandle.ReceieveWorldChat },
             {(int) ServerPackets.AbilityUse, ClientHandle.RecieveAbility },
             {(int) ServerPackets.StartIntroduction, ClientHandle.StartIntroduction },
             {(int) ServerPackets.UpdateAnimation, ClientHandle.UpdatePlayerAnimation},
             {(int) ServerPackets.dbReturn, ClientHandle.DBReturn },
             {(int) ServerPackets.LoadScene, ClientHandle.LoadScene }


        };
        
    }

    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from server");
        }
    }

    private void Disconnect(Packet _packet)
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            if(udp.socket != null)
                udp.socket.Close();

            Debug.Log("Disconnected from server");
            
            
           
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }



}
