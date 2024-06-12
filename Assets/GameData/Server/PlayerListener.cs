using System;
using System.Collections.Generic;
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
        private int messageCount = 0;
        private Dictionary<
            int,
            (string message, DateTime timestamp, int retryCount)
        > pendingMessages = new Dictionary<int, (string, DateTime, int)>();
        private const int retryIntervalSeconds = 5; // Интервал повторной отправки сообщения в секундах
        private const int maxRetries = 3; // Максимальное количество попыток повторной отправки
        private Thread retryThread;
        private bool running = true;
        private readonly object lockObject = new object();

        protected override void OnOpen()
        {
            base.OnOpen();
            active = true;
            RoomCreator.OnPlayerConnect(this);
            retryThread = new Thread(CheckForAcknowledgements);
            retryThread.Start();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            active = false;
            Debug.Log($"PLAYER {playerID} DISCONNECTED");
            OnPlayerDisconnect();
            running = false;
            retryThread.Join();
        }

        private void OnPlayerDisconnect()
        {
            GlobalMessageHandler.OnPlayerDisconnect(roomNumber, playerID);
        }

        private void HandleMessage(ClientServerMessage csm)
        {
            Debug.Log($"Server got message {csm.messageID} from player {playerID}");

            if (csm.type == (int)CSMRequest.Type.ACK)
            {
                // Подтверждение получено, удаляем сообщение из списка ожидающих подтверждения
                lock (lockObject)
                {
                    pendingMessages.Remove(csm.messageID);
                }
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
            Debug.Log($"Error: {e.Exception.Message} {e.Exception.StackTrace}");
        }

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck)
        {
            ClientServerMessage csm = BuildMessage(type, body);
            csm.messageID = messageCount;
            messageCount++;
            Debug.Log($"Server send message {csm.messageID} to player {playerID} ack {needAck}");
            string message = JsonUtility.ToJson(csm);
            Send(message);

            if (needAck)
            {
                lock (lockObject)
                {
                    pendingMessages[csm.messageID] = (message, DateTime.UtcNow, 0);
                }
            }
        }

        private ClientServerMessage BuildMessage<T>(CSMRequest.Type type, T body)
        {
            string data = JsonUtility.ToJson(body);
            ClientServerMessage csm = new ClientServerMessage((int)type, data);
            return csm;
        }

        private void CheckForAcknowledgements()
        {
            while (running)
            {
                Thread.Sleep(retryIntervalSeconds * 1000);
                List<int> toRemove = new List<int>();

                lock (pendingMessages)
                {
                    foreach (var kvp in pendingMessages)
                    {
                        var (message, timestamp, retryCount) = kvp.Value;

                        if ((DateTime.UtcNow - timestamp).TotalSeconds >= retryIntervalSeconds)
                        {
                            if (retryCount < maxRetries)
                            {
                                Debug.Log(
                                    $"Retrying message {kvp.Key} to player {playerID} (Attempt {retryCount + 1}/{maxRetries})"
                                );
                                Send(message);
                                pendingMessages[kvp.Key] = (
                                    message,
                                    DateTime.UtcNow,
                                    retryCount + 1
                                );
                            }
                            else
                            {
                                Debug.LogWarning(
                                    $"Failed to receive acknowledgement for message {kvp.Key} to player {playerID} after {maxRetries} attempts"
                                );
                                toRemove.Add(kvp.Key);
                            }
                        }
                    }

                    // Удаление элементов после завершения перечисления
                    foreach (int key in toRemove)
                    {
                        pendingMessages.Remove(key);
                    }
                }
            }
        }
    }
}
