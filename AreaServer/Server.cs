using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;
using UnityEngine;

public class Server 
{
    public static int MaxPlayers { get; private set; }

    public static int Port { get; private set; }
    public static Dictionary<int, Client> tempClients = new Dictionary<int, Client>();
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    public static List<int> toPing = new List<int>();


    private static TcpListener tcpListener;

    private static UdpClient udpListener;

    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);
        

        Debug.Log($"Server started on {Port}.");
    }

    public static void Stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }

  

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        //Console.Write($"Incoming connection from {_client.Client.RemoteEndPoint} ....");
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint} ....");
        for (int i = 1; i <= MaxPlayers; i++)
        {
            

            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
           
        }

        Console.Write($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }
    public static int NextOpenSpot()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                return i;
            }
        }
        return -1;
    }

    public static int FindHowManyActive()
    {
        int count = 0;
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket != null)
            {
                count++;
            }
        }
        return count;
    }


    public static void ResetClient(int i)
    {
        clients[i] = new Client(i);
    }

    private static void UDPReceiveCallback(IAsyncResult _result)
    {

       
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);
            //Debug.Log(_clientEndPoint + " connected");
            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }

                if (clients[_clientId].udp.endPoint == null)
                {
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    clients[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error recieving UDP Data: {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }

    private static void InitializeServerData()
    {

       
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int) ClientPackets.welcomeReceived, ServerHandle.WelcomeRecieved },
                { (int) ClientPackets.ping, ServerHandle.PingRecieve },
                { (int) ClientPackets.CharacterLoggedIn, ServerHandle.CharacterSpawn },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                { (int)ClientPackets.playerShoot, ServerHandle.PlayerShoot },
                { (int)ClientPackets.playerThrowItem, ServerHandle.PlayerThrowItem },
                { (int)ClientPackets.SetTarget, ServerHandle.SetTarget },
                { (int)ClientPackets.worldChat, ServerHandle.SendWorldChatUDP },
                { (int)ClientPackets.AbilityCast, ServerHandle.AbilityUse},
                { (int)ClientPackets.characterAnimation, ServerHandle.CharacterAnimationUpdate},
                { (int)ClientPackets.dbRequest, ServerHandle.DatabaseRequest},
                { (int)ClientPackets.Equip, ServerHandle.Equip}

            };
    }
}
