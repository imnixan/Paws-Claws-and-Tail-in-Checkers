using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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
        private const int ACK_TIMOUT = 1000;
        private const int ACK_CHECK_INTERVAL = 3;
        private const int MAX_STORED_MESSAGES = 10;
        private List<AckWaiter> myMessageHistory = new List<ClientServerMessage>();

        private int currentMessageId;
        private int expectedMessageId;

        protected override void OnOpen()
        {
            base.OnOpen();
            active = true;
            RoomCreator.OnPlayerConnect(this);
        }

        private IEnumerator ResendMessageNoAck()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(ACK_CHECK_INTERVAL);
            while (true)
            {
                yield return waitForSeconds;
                float currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                foreach (var message in myMessageHistory)
                {
                    if (currentTime - message.timeStamp >= ACK_TIMOUT)
                    {
                        string repeatedMessage = JsonUtility.ToJson(message);
                        Send(repeatedMessage);
                    }
                }
            }
        }

        public void RequestMessage(int messageID)
        {
            string data = JsonUtility.ToJson(new StringData(""));

            ClientServerMessage csm = new ClientServerMessage(
                (int)CSMRequest.Type.MESSAGE_REQUEST,
                data
            );
            csm.messageID = messageID;
            string message = JsonUtility.ToJson(csm);
            Send(message);
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

        private void HandleMessage(ClientServerMessage csm)
        {
            GlobalMessageHandler.OnMessage(csm, roomNumber, playerID);
        }

        private void RecieveAck(int messageID)
        {
            foreach (var waiter in myMessageHistory)
            {
                if (waiter.csm.messageID == messageID)
                {
                    myMessageHistory.Remove(waiter);
                    return;
                }
            }
        }

        private void SendAck(int messageID)
        {
            string data = JsonUtility.ToJson(new StringData(""));

            ClientServerMessage csm = new ClientServerMessage((int)CSMRequest.Type.ACK, data);
            string message = JsonUtility.ToJson(csm);
            Send(message);
        }

        private void ResendMessage(int messageID)
        {
            foreach (var waiter in myMessageHistory)
            {
                if (waiter.csm.messageID == messageID)
                {
                    string repeatedMessage = JsonUtility.ToJson(waiter);
                    Send(repeatedMessage);
                    ResendMessage(messageID++);
                    return;
                }
            }
        }

        private void CheckMessage(ClientServerMessage csm)
        {
            CSMRequest.Type type = (CSMRequest.Type)csm.type;
            switch (type)
            {
                case CSMRequest.Type.ACK:
                    RecieveAck(csm.messageID);
                    break;
                case CSMRequest.Type.MESSAGE_REQUEST:
                    ResendMessage(csm.messageID);
                    break;
            }
            if (csm.messageID == expectedMessageId)
            {
                HandleMessage(csm);
                SendAck(csm.messageID);
                expectedMessageId++;
            }
            else
            {
                RequestMessage(expectedMessageId);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            ClientServerMessage csm = JsonUtility.FromJson<ClientServerMessage>(e.Data);
            CheckMessage(csm);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Debug.Log($"Error: {e.Exception.Message} {e.Exception.StackTrace}");
        }

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck)
        {
            ClientServerMessage csm = BuildMessage(type, body);
            string message = JsonUtility.ToJson(csm);
            Send(message);
            if (needAck)
            {
                myMessageHistory.Add(
                    new AckWaiter(csm, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
                );
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
