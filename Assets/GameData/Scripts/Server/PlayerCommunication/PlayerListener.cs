using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using PJTC.Enums;
using PJTC.Structs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PJTC.Server
{
    public class PlayerListener : WebSocketBehavior
    {
        public Guid roomNumber;
        public int playerID;
        public bool active;
        private const int RETRY_ACK_INTERVAL = 5000;
        private const int MAX_RETRIES = 3; // Максимальное количество попыток повторной отправки
        private const int ALIVE_CHECK_TIME = 5000;
        private int retries;
        public bool isConnected
        {
            get { return IsAlive; }
        }

        private int messageCount = 0;
        private Dictionary<
            int,
            (string message, DateTime timestamp, int retryCount)
        > pendingMessages = new Dictionary<int, (string, DateTime, int)>();
        private System.Timers.Timer timer;

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck)
        {
            ClientServerMessage csm = BuildMessage(type, body);
            csm.messageID = messageCount;
            messageCount++;
            string message = JsonUtility.ToJson(csm);
            SendAsync(message, MessageSended);

            if (needAck)
            {
                pendingMessages[csm.messageID] = (message, DateTime.UtcNow, 0);
            }
        }

        public void MessageSended(bool succes) { }

        protected override void OnOpen()
        {
            base.OnOpen();
            active = true;
            RoomCreator.OnPlayerConnect(this);
            timer = new System.Timers.Timer(ALIVE_CHECK_TIME);

            timer.Elapsed += CheckAlive;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public void CloseListener()
        {
            CloseAsync();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            OnPlayerDisconnect();
        }

        private void OnPlayerDisconnect()
        {
            timer.Stop();
            Debug.Log($"PLAYER {playerID} DISCONNECTED");
            if (active && roomNumber != Guid.Empty)
            {
                active = false;
                Debug.Log(roomNumber);
                GlobalMessageHandler.OnPlayerDisconnect(roomNumber, playerID);
            }
            else
            {
                RoomCreator.OnPlayerDisconnect(this);
            }
        }

        private void HandleMessage(ClientServerMessage csm)
        {
            if (csm.type == (int)CSMRequest.Type.ACK)
            {
                pendingMessages.Remove(csm.messageID);
            }
            else
            {
                GlobalMessageHandler.OnMessage(csm, roomNumber, playerID);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            ClientServerMessage csm = JsonUtility.FromJson<ClientServerMessage>(e.Data);

            HandleMessage(csm);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Debug.Log($"Error: {e.Exception} {e.Exception.StackTrace}");
        }

        private void CheckAlive(object source, ElapsedEventArgs e)
        {
            if (!IsAlive)
            {
                retries++;
                if (retries >= MAX_RETRIES)
                {
                    active = false;

                    OnPlayerDisconnect();
                }
            }
            else
            {
                retries = 0;
            }
        }

        private ClientServerMessage BuildMessage<T>(CSMRequest.Type type, T body)
        {
            string data = JsonUtility.ToJson(body);
            ClientServerMessage csm = new ClientServerMessage((int)type, data);

            return csm;
        }
    }
}
