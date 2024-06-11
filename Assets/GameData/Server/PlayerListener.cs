using System;
using DG.Tweening.Core.Easing;
using PCTC.Enums;
using PCTC.Structs;
using PTCP.Scripts;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PCTC.Server
{
    public class PlayerListener : WebSocketBehavior, MessageSender
    {
        public Guid roomNumber;
        public int playerID;
        public bool active;
        private int messageCount = 0;

        protected override void OnOpen()
        {
            base.OnOpen();
            active = true;
            RoomCreator.OnPlayerConnect(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            active = false;
            Debug.Log($"PLAYER {playerID} DISCONNECTED");
            OnPlayerDisconnect();
        }

        private void OnPlayerDisconnect()
        {
            GlobalMessageHandler.OnPlayerDisconnect(roomNumber, playerID);
            RoomCreator.OnListenerRemove(this);
        }

        private void HandleMessage(ClientServerMessage csm)
        {
            Debug.Log($"Server got message {csm.messageID} from player {playerID}");
            GlobalMessageHandler.OnMessage(csm, roomNumber, playerID);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            ClientServerMessage csm = JsonUtility.FromJson<ClientServerMessage>(e.Data);
            HandleMessage(csm);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Debug.Log($"Error: {e.Exception.Message} {e.Exception.StackTrace}");
        }

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck)
        {
            ClientServerMessage csm = BuildMessage(type, body);
            csm.messageID = messageCount;
            messageCount++;
            Debug.Log($"Server send message {csm.messageID} to player {playerID}");
            string message = JsonUtility.ToJson(csm);
            Send(message);
        }

        private ClientServerMessage BuildMessage<T>(CSMRequest.Type type, T body)
        {
            string data = JsonUtility.ToJson(body);
            ClientServerMessage csm = new ClientServerMessage((int)type, data);
            return csm;
        }
    }
}
