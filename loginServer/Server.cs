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

    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;


    private static TcpListener tcpListener;


    

    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);


        Debug.Log($"Server started on {Port}.");
    }

    public static void Stop()
    {
        tcpListener.Stop();
       
    }

    public static void DisconnectAll()
    {
        for (int i = 1; i <= clients.Count; i++)
        {
            if(clients[i].tcp.socket != null)
            {
                clients[i].Disconnect();
            }
        }
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Console.Write($"Incoming connection from {_client.Client.RemoteEndPoint} ....");
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                Debug.Log($"{ _client.Client.RemoteEndPoint} assigned to {i}");
                return;
            }
        }

        Console.Write($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }


   

    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
         {
                { (int) ClientPackets.RegisterUser,RegisterUser },
                { (int) ClientPackets.Login, Login },
                { (int) ClientPackets.Characters, CharacterRequest },
                { (int) ClientPackets.RequestCreation, RequestCreation },
                { (int) ClientPackets.RequestToLaunch, LaunchToGame }

                


         };
    }

    public static void RequestCreation(int _from, Packet _packet)
    {
        ServerHandle.instance.RequestCreation(_from, _packet);
    }

    public static  void RegisterUser(int _from, Packet _packet)
    {
        ServerHandle.instance.RegisterUser(_from, _packet);
    }

    public static void Login(int _from, Packet _packet)
    {
        ServerHandle.instance.Login(_from, _packet);
    }

    public static void CharacterRequest(int _from, Packet _packet)
    {
        ServerHandle.instance.CharacterRequest(_from);
    }

    public static void LaunchToGame(int _from, Packet _packet)
    {
        ServerSend.LaunchToGame(_from, _packet.ReadInt());
    }
}

