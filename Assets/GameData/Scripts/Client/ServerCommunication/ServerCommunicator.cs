using System;
using System.Collections;
using System.Collections.Generic;
using GameData.Managers;
using PJTC.Enums;
using PJTC.Managers;
using PJTC.Server;
using PJTC.Structs;
using UnityEngine;
using WebSocketSharp;

namespace GameData.Scripts
{
    public class ServerCommunicator
    {
        private string ip;
        public WebSocket ws { get; private set; }
        private ClientGameManager gameManager;
        public ServerDataSender serverDataSender { get; private set; }
        public ServerDataHandler serverDataHandler { get; private set; }
        public int messageCount = 0;

        public ServerCommunicator(ClientGameManager gm, string ip = "localhost")
        {
            this.ip = ip;
            this.gameManager = gm;
        }

        public void ConnectToServer()
        {
            ws = new WebSocket($"ws://{ip}:8080/checkers");
            serverDataHandler = new ServerDataHandler(ws);
            serverDataSender = new ServerDataSender(ws, this);
            ws.OnMessage += OnMessage;
            ws.OnOpen += OnConnected;
            ws.OnError += OnError;
            ws.OnClose += OnConnectionClosed;
            ws.ConnectAsync();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            ClientServerMessage csm = JsonUtility.FromJson<ClientServerMessage>(e.Data);
            SendAck(csm.messageID);
            HandleMessage(csm);
        }

        private void SendAck(int messageID)
        {
            CSMRequest.Type type = CSMRequest.Type.ACK;
            ClientServerMessage csm = new ClientServerMessage((int)type, "Ack");
            csm.messageID = messageID;
            string message = JsonUtility.ToJson(csm);
            Send(message);
        }

        public void Disconnect()
        {
            if (ws != null)
            {
                ws.Close();
            }
            else
            {
                gameManager.RestartScene();
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Debug.Log("error" + e);
        }

        private void OnConnected(object sender, System.EventArgs e)
        {
            gameManager.OnConnect();
        }

        private void OnConnectionClosed(object sender, System.EventArgs e)
        {
            gameManager.OnServerEndConnection();
        }

        void OnDestroy()
        {
            Disconnect();
        }

        private void Send(string message)
        {
            ws.Send(message);
        }

        private void HandleMessage(ClientServerMessage csm)
        {
            serverDataHandler.ProcessServerData(csm);
        }

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck)
        {
            ClientServerMessage csm = BuildMessage(type, body);
            csm.messageID = messageCount;
            messageCount++;
            Debug.Log($"Client {gameManager.playerID} send message {csm.messageID}");
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
