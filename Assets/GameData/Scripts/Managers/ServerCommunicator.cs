using System;
using GameData.Managers;
using PCTC.Enums;
using PCTC.Managers;
using PCTC.Structs;
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

        public ServerCommunicator(ClientGameManager gm, string ip = "localhost")
        {
            this.ip = ip;
            this.gameManager = gm;
        }

        public void ConnectToServer()
        {
            ws = new WebSocket($"ws://{ip}:8080/checkers");
            serverDataHandler = new ServerDataHandler(ws, gameManager);
            serverDataSender = new ServerDataSender(ws, gameManager);
            ws.OnMessage += serverDataHandler.ProcessServerData;
            ws.OnOpen += OnConnected;
            ws.OnError += OnError;
            ws.OnClose += OnConnectionClosed;
            ws.Connect();
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
            Debug.Log("SERVER END CONNECTION");
            gameManager.OnServerEndConnection();
        }

        void OnDestroy()
        {
            Disconnect();
        }
    }
}
