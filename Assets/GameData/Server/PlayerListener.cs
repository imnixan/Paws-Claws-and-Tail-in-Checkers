using System;
using PCTC.Structs;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PCTC.Server
{
    public class PlayerListener : WebSocketBehavior
    {
        public Guid roomNumber;
        public int playerID;
        public bool active;

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
            Debug.Log("PLAYER DISCONNECTED");
            OnPlayerDisconnect();
        }

        private void OnPlayerDisconnect()
        {
            GlobalMessageHandler.OnPlayerDisconnect(roomNumber, playerID);
            RoomCreator.OnListenerRemove(this);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            GlobalMessageHandler.OnMessage(e, roomNumber, playerID);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Debug.Log($"Error: {e.Exception.Message} {e.Exception.StackTrace}");
        }

        public void SendPlayerMessage(string message)
        {
            Send(message);
        }
    }
}
