using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using PJTC.Enums;
using PJTC.Structs;
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
        private const float MAX_PING_TIME_FOR_CONNECT = 3;
        public float lastPingTime { get; private set; }
        public float timeSinceLastPing
        {
            get
            {
                float currentTime = Time.time;
                return currentTime - lastPingTime;
            }
        }

        public bool isConnected
        {
            get { return timeSinceLastPing <= MAX_PING_TIME_FOR_CONNECT; }
        }

        private int messageCount = 0;
        private Dictionary<
            int,
            (string message, DateTime timestamp, int retryCount)
        > pendingMessages = new Dictionary<int, (string, DateTime, int)>();
        private const int RETRY_INTERVAL = 5; // Интервал повторной отправки сообщения в секундах
        private const int MAX_RETRIES = 3; // Максимальное количество попыток повторной отправки
        private const float MAX_PING_TIME_SECONDS = 7.5f;
        private const int PING_CHECK_TIME = 2500;
        private Thread retryThread;
        private bool running = true;
        private readonly object lockObject = new object();
        private System.Timers.Timer timer;

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck)
        {
            ClientServerMessage csm = BuildMessage(type, body);
            csm.messageID = messageCount;
            messageCount++;
            if (type == CSMRequest.Type.MAKE_MOVE)
            {
                Debug.Log($"Process move {csm.data}");
            }
            string message = JsonUtility.ToJson(csm);
            SendAsync(message, MessageSended);

            if (needAck)
            {
                lock (lockObject)
                {
                    pendingMessages[csm.messageID] = (message, DateTime.UtcNow, 0);
                }
            }
        }

        public void MessageSended(bool succes) { }

        protected override void OnOpen()
        {
            base.OnOpen();
            EmitOnPing = true;
            active = true;
            RoomCreator.OnPlayerConnect(this);
            retryThread = new Thread(CheckForAcknowledgements);
            retryThread.Start();
            timer = new System.Timers.Timer(PING_CHECK_TIME);

            timer.Elapsed += CheckPing;
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
            running = false;
            retryThread.Join();
        }

        private void OnPlayerDisconnect()
        {
            timer.Stop();
            Debug.Log($"PLAYER {playerID} DISCONNECTED");
            if (active)
            {
                active = false;
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
            if (e.IsPing)
            {
                OnPing();
                return;
            }

            ClientServerMessage csm = JsonUtility.FromJson<ClientServerMessage>(e.Data);

            HandleMessage(csm);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Debug.Log($"Error: {e.Exception.Message} {e.Exception.StackTrace}");
        }

        private void OnPing()
        {
            Debug.Log($"Got ping from player {playerID}");
            lastPingTime = Time.time;
        }

        private void CheckPing(object source, ElapsedEventArgs e)
        {
            Debug.Log(
                $"Player {playerID} ping check... last {timeSinceLastPing} - {MAX_PING_TIME_SECONDS}"
            );
            if (timeSinceLastPing > MAX_PING_TIME_SECONDS)
            {
                active = false;
                Debug.Log($"PLAYER {playerID} DISCONNECTED");

                OnPlayerDisconnect();

                running = false;
                retryThread.Join();
            }
            else
            {
                Debug.Log($"Player {playerID} ping check ok");
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
                Thread.Sleep(RETRY_INTERVAL * 1000);
                List<int> toRemove = new List<int>();

                lock (pendingMessages)
                {
                    foreach (var kvp in pendingMessages)
                    {
                        var (message, timestamp, retryCount) = kvp.Value;

                        if ((DateTime.UtcNow - timestamp).TotalSeconds >= RETRY_INTERVAL)
                        {
                            if (retryCount < MAX_RETRIES)
                            {
                                Debug.Log(
                                    $"Retrying message {kvp.Key} to player {playerID} (Attempt {retryCount + 1}/{MAX_RETRIES})"
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
                                    $"Failed to receive acknowledgement for message {kvp.Key} to player {playerID} after {MAX_RETRIES} attempts"
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
