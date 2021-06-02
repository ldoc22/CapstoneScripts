using System.Collections.Generic;
using UnityEngine.UI;


namespace Chat
{
    [System.Serializable]
    class ChatWindow
    {
        public int id;
        public List<Message> messages;
        public bool hasUnrealMessages;
        public int MaxMessages;
        public ChatWindow(int _id, int _max)
        {
            id = _id;
            messages = new List<Message>();
            MaxMessages = _max;
        }

        public void AddMessage(string _msg, int _channel, int _clientID)
        {


            Message msg = new Message(_msg, _channel, _clientID);
            messages.Add(msg);

        }

      
    }
    [System.Serializable]
    class Message 
    {
        public string text;
        public int channel;
        public int ClientID;

        public Message(string _text, int _channel, int _ClientID)
        {
            text = _text;
            channel = _channel;
            ClientID = _ClientID;
        }
    
    }



}

