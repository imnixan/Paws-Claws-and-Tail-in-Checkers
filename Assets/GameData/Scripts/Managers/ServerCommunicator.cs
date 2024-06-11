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
        private WebSocket ws;
        private ClientGameManager gameManager;
        public ServerDataSender serverDataSender { get; private set; }
        public ServerDataHandler serverDataHandler { get; private set; }

        public ServerCommunicator(string ip = "localhost")
        {
            this.ip = ip;
        }

        public void ConnectToServer(ClientGameManager gm)
        {
            this.gameManager = gm;
            ws = new WebSocket($"ws://{ip}:8080/checkers");
            serverDataHandler = new ServerDataHandler(ws, gameManager);
            serverDataSender = new ServerDataSender(ws, gameManager);
            ws.OnMessage += serverDataHandler.ProcessServerData;
            ws.OnOpen += OnConnected;
            ws.OnError += OnError;
            ws.OnClose += gameManager.OnServerCloseConnect;
            ws.Connect();
        }

        public void Disconnect()
        {
            if (ws != null)
            {
                ws.Close();
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

        void OnDestroy()
        {
            Disconnect();
        }
    }
}
