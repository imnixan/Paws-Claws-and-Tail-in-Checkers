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
        public int playerId;

        protected override void OnOpen()
        {
            base.OnOpen();
            RoomCreator.OnPlayerConnect(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            RoomCreator.OnPlayerDisconnect(this);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            GlobalMessageHandler.OnMessage(e, roomNumber);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Debug.Log($"Error: {e.Exception.Message}");
        }

        public void SendPlayerMessage(string message)
        {
            Debug.Log($"send data {message} to player");

            Send(message);
        }
    }
}
